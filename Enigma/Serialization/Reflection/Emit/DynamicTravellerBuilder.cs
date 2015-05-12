using System;
using System.Collections.Generic;
using System.Reflection;
using Enigma.Reflection.Emit;
using ConstructorBuilder = Enigma.Reflection.Emit.ConstructorBuilder;

namespace Enigma.Serialization.Reflection.Emit
{

    public class DynamicTravellerBuilder
    {
        private readonly Type _type;
        private readonly DynamicTravellerContext _dtContext;
        private readonly ClassBuilder _classBuilder;
        private readonly SerializableTypeProvider _typeProvider;
        private readonly DynamicTraveller _dynamicTraveller;
        private readonly ConstructorBuilder _constructorBuilder;
        private readonly MethodBuilder _travelWriteMethod;
        private readonly MethodBuilder _travelReadMethod;

        public DynamicTravellerBuilder(DynamicTravellerContext dtContext, ClassBuilder classBuilder, SerializableTypeProvider typeProvider, Type type)
        {
            _dtContext = dtContext;
            _classBuilder = classBuilder;
            _typeProvider = typeProvider;
            _type = type;
            _constructorBuilder = _classBuilder.DefineConstructor(typeof(IVisitArgsFactory));
            _constructorBuilder.IL.Var.Load(_constructorBuilder.IL.Var.This());
            _constructorBuilder.IL.CallBaseConstructor(typeof(object).GetConstructor(new Type[] { }));
            //_constructorBuilder.IL.Return();

            _travelWriteMethod = _classBuilder.DefineOverloadMethod("Travel", typeof(void), new[] { typeof(IWriteVisitor), _type });
            _travelReadMethod = _classBuilder.DefineOverloadMethod("Travel", typeof(void), new[] { typeof(IReadVisitor), _type });

            var factory = new VisitArgsFactory(typeProvider, type);
            _dynamicTraveller = new DynamicTraveller(_classBuilder.Type, factory, _constructorBuilder.Reference, _travelWriteMethod.Method, _travelReadMethod.Method, dtContext.Members);
        }

        public DynamicTraveller DynamicTraveller { get { return _dynamicTraveller; } }
       
        public void BuildTraveller()
        {
            if (_classBuilder.IsSealed) throw new InvalidOperationException("Class builder is sealed");
            var target = _typeProvider.GetOrCreate(_type);
            var members = _dtContext.Members;
            var factoryArgument = new MethodArgILCodeVariable(1, members.VisitArgsFactoryType);


            var childTravellers = new Dictionary<Type, ChildTravellerInfo>();
            var argFields = new Dictionary<SerializableProperty, FieldInfo>();

            var travellerIndex = 0;
            foreach (var property in target.Properties) {
                var argField = _classBuilder.DefinePrivateField("_arg" + property.Ref.Name, members.VisitArgsType);
                var visitArgsCode = new CallMethodILCode(factoryArgument, members.ConstructVisitArgsMethod, property.Ref.Name);
                _constructorBuilder.IL.SetField(argField, visitArgsCode);
                argFields.Add(property, argField);

                Type[] types;
                if (!ReflectionAnalyzer.TryGetComplexTypes(property.Ext, out types)) continue;
                
                foreach (var type in types) {
                    if (childTravellers.ContainsKey(type)) continue;

                    var dynamicTraveller = _dtContext.Get(type);
                    var interfaceType = typeof (IGraphTraveller<>).MakeGenericType(type);
                    var fieldBuilder = _classBuilder.DefinePrivateField(string.Concat("_traveller", type.Name, ++travellerIndex), interfaceType);
                    childTravellers.Add(type, new ChildTravellerInfo {
                        Field = fieldBuilder,
                        TravelWriteMethod = dynamicTraveller.TravelWriteMethod,
                        TravelReadMethod = dynamicTraveller.TravelReadMethod
                    });
                    
                    var getFactoryCode = new CallMethodILCode(factoryArgument, members.ConstructVisitArgsWithTypeMethod, type);
                    var callConstructorCode = new CallConstructorILCode(dynamicTraveller.Constructor, getFactoryCode);
                    _constructorBuilder.IL.SetField(fieldBuilder, callConstructorCode);
                    
                    //_constructorBuilder.IL.SetField(fieldBuilder, dynamicTraveller.Constructor);
                }
            }
            _constructorBuilder.IL.Return();

            var context = new TravellerContext(childTravellers, argFields);
            BuildWriteMethods(target, context);
            BuildReadMethods(target, context);

            _classBuilder.Seal();
            _dynamicTraveller.Complete(_classBuilder.Type);
        }

        private void BuildWriteMethods(SerializableType target, TravellerContext context)
        {
            var typedMethodBuilder = _travelWriteMethod;
            var writeBuilder = new DynamicWriteTravellerBuilder(typedMethodBuilder, target, context);
            writeBuilder.BuildTravelWriteMethod();

            var untypedMethodBuilder = _classBuilder.DefineOverloadMethod("Travel", typeof(void), new[] { typeof(IWriteVisitor), typeof(object) });
            var il = untypedMethodBuilder.IL;
            il.LoadThis();
            il.Var.Load(new MethodArgILCodeVariable(1, typeof(IWriteVisitor)));
            il.Var.Load(new MethodArgILCodeVariable(2, typeof(object)));
            il.Cast(target.Type);
            il.Call(typedMethodBuilder.Method);
            il.Return();
        }

        private void BuildReadMethods(SerializableType target, TravellerContext context)
        {
            var typedMethodBuilder = _travelReadMethod;
            var readBuilder = new DynamicReadTravellerBuilder(typedMethodBuilder, target, context);
            readBuilder.BuildTravelReadMethod();

            var untypedMethodBuilder = _classBuilder.DefineOverloadMethod("Travel", typeof(void), new[] { typeof(IReadVisitor), typeof(object) });
            var il = untypedMethodBuilder.IL;
            il.LoadThis();
            il.Var.Load(new MethodArgILCodeVariable(1, typeof(IReadVisitor)));
            il.Var.Load(new MethodArgILCodeVariable(2, typeof(object)));
            il.Cast(target.Type);
            il.Call(typedMethodBuilder.Method);
            il.Return();
        }

    }
}
