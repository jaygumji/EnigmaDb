using System;
using System.Collections.Generic;
using Enigma.Reflection.Emit;
using ConstructorBuilder = Enigma.Reflection.Emit.ConstructorBuilder;

namespace Enigma.Serialization.Reflection.Emit
{

    public class DynamicTravellerBuilder
    {
        private readonly Type _type;
        private readonly DynamicTravellerContext _dynamicTravellerContext;
        private readonly ClassBuilder _classBuilder;
        private readonly DynamicTraveller _dynamicTraveller;
        private readonly ConstructorBuilder _constructorBuilder;
        private readonly MethodBuilder _travelWriteMethod;
        private readonly MethodBuilder _travelReadMethod;

        public DynamicTravellerBuilder(DynamicTravellerContext dynamicTravellerContext, ClassBuilder classBuilder, Type type)
        {
            _dynamicTravellerContext = dynamicTravellerContext;
            _classBuilder = classBuilder;
            _type = type;
            _constructorBuilder = _classBuilder.DefineDefaultConstructor();
            _travelWriteMethod = _classBuilder.DefineOverloadMethod("Travel", typeof(void), new[] { typeof(IWriteVisitor), _type });
            _travelReadMethod = _classBuilder.DefineOverloadMethod("Travel", typeof(void), new[] { typeof(IReadVisitor), _type });
            _dynamicTraveller = new DynamicTraveller(_classBuilder.Type, _constructorBuilder.Reference, _travelWriteMethod.Method, _travelReadMethod.Method);
        }

        public DynamicTraveller DynamicTraveller { get { return _dynamicTraveller; } }
       
        public void BuildTraveller()
        {
            if (_classBuilder.IsSealed) throw new InvalidOperationException("Class builder is sealed");
            var context = TypeReflectionContext.Get(_type);

            var childTravellers = new Dictionary<Type, ChildTravellerInfo>();
            var fieldIndex = 0;
            foreach (var property in context.SerializableProperties) {
                Type[] types;
                if (!ReflectionAnalyzer.TryGetComplexTypes(property.PropertyTypeContext.Extended, out types)) continue;
                
                foreach (var type in types) {
                    var dynamicTraveller = _dynamicTravellerContext.Get(type);
                    var interfaceType = typeof (IGraphTraveller<>).MakeGenericType(type);
                    var fieldBuilder = _classBuilder.DefinePrivateField(string.Concat("_childTraveller", type.Name, fieldIndex), interfaceType);
                    childTravellers.Add(type, new ChildTravellerInfo {
                        Field = fieldBuilder,
                        TravelWriteMethod = dynamicTraveller.TravelWriteMethod,
                        TravelReadMethod = dynamicTraveller.TravelReadMethod
                    });
                    _constructorBuilder.IL.SetFieldWithDefaultConstructor(fieldBuilder, dynamicTraveller.Constructor);
                    fieldIndex++;
                }
            }
            _constructorBuilder.IL.Return();

            BuildWriteMethods(context, childTravellers);
            BuildReadMethods(context, childTravellers);

            _classBuilder.Seal();
            _dynamicTraveller.Complete(_classBuilder.Type);
        }

        private void BuildWriteMethods(TypeReflectionContext context, Dictionary<Type, ChildTravellerInfo> childTravellers)
        {
            var typedMethodBuilder = _travelWriteMethod;
            var writeBuilder = new DynamicWriteTravellerBuilder(typedMethodBuilder, context, childTravellers);
            writeBuilder.BuildTravelWriteMethod();

            var untypedMethodBuilder = _classBuilder.DefineOverloadMethod("Travel", typeof(void), new[] { typeof(IWriteVisitor), typeof(object) });
            var il = untypedMethodBuilder.IL;
            il.LoadArgs(0, 1, 2);
            il.Cast(context.Type);
            il.Call(typedMethodBuilder.Method);
            il.Return();
        }

        private void BuildReadMethods(TypeReflectionContext context, Dictionary<Type, ChildTravellerInfo> childTravellers)
        {
            var typedMethodBuilder = _travelReadMethod;
            var readBuilder = new DynamicReadTravellerBuilder(typedMethodBuilder, context, childTravellers);
            readBuilder.BuildTravelReadMethod();

            var untypedMethodBuilder = _classBuilder.DefineOverloadMethod("Travel", typeof(void), new[] { typeof(IReadVisitor), typeof(object) });
            var il = untypedMethodBuilder.IL;
            il.LoadArgs(0, 1, 2);
            il.Cast(context.Type);
            il.Call(typedMethodBuilder.Method);
            il.Return();
        }

    }
}
