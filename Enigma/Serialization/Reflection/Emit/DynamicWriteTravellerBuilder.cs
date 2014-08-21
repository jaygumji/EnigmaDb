using System;
using System.Collections.Generic;
using System.Reflection;
using Enigma.Reflection;
using Enigma.Reflection.Emit;
using MethodBuilder = Enigma.Reflection.Emit.MethodBuilder;

namespace Enigma.Serialization.Reflection.Emit
{
    public class DynamicWriteTravellerBuilder
    {
        private static readonly DynamicWriteTravellerMembers Members = new DynamicWriteTravellerMembers();

        private readonly TypeReflectionContext _context;
        private readonly ILCodeVariable _visitorVariable;
        private readonly Dictionary<Type, ChildTravellerInfo> _childTravellers;
        private readonly ILExpressed _il;

        public DynamicWriteTravellerBuilder(MethodBuilder builder, TypeReflectionContext context, Dictionary<Type, ChildTravellerInfo> childTravellers)
        {
            _context = context;
            _childTravellers = childTravellers;
            _il = builder.IL;
            _visitorVariable = new MethodArgILCodeVariable(1, typeof(IWriteVisitor));
        }

        public void BuildTravelWriteMethod()
        {
            var graphArgument = new MethodArgILCodeVariable(2, _context.Type);
            foreach (var property in _context.SerializableProperties) {
                GeneratePropertyCode(graphArgument, property);
            }
            _il.Return();
        }

        private void GeneratePropertyCode(ILCodeVariable graphVariable, PropertyReflectionContext context)
        {
            var extPropertyType = context.PropertyTypeContext.Extended;
            //if (extPropertyType.Class == TypeClass.Dictionary) return;
            //if (extPropertyType.Class == TypeClass.Complex) return;
            //if (extPropertyType.Class == TypeClass.Collection) return;
            //if (extPropertyType.Class == TypeClass.Nullable) return;
            //if (extPropertyType.Class == TypeClass.Value) return;
            if (extPropertyType.IsValueOrNullableOfValue()) {
                var isNullable = extPropertyType.Class == TypeClass.Nullable;
                var isEnum = extPropertyType.IsEnum();

                var valueType = extPropertyType.Inner;

                _il.Var.Load(_visitorVariable);
                _il.Snippets.GetPropertyValue(graphVariable, context.Property);

                if (isEnum) {
                    valueType = extPropertyType.GetUnderlyingEnumType();
                    if (!isNullable) _il.Snippets.AsNullable(valueType);
                }
                else if (!isNullable && valueType.IsValueType)
                    _il.Snippets.AsNullable(valueType);

                _il.LoadValue(context.Property.Name);
                _il.LoadValue(context.Index);

                if (isEnum) {
                    _il.Snippets.GetPropertyValue(graphVariable, context.Property);
                    _il.Call(Members.VisitArgsEnumValue.MakeGenericMethod(valueType));
                }
                else if (isNullable) {
                    _il.Snippets.GetPropertyValue(graphVariable, context.Property);
                    var local = _il.DeclareLocal("nullableValue", valueType);
                    _il.Var.Set(local);
                    _il.Var.LoadAddress(local);
                    _il.Call(extPropertyType.Container.AsNullable().GetHasValueMethod);
                    _il.Call(Members.VisitArgsNullableValue);
                }
                else
                    _il.Call(Members.VisitArgsValue);

                var visitValueMethod = Members.VisitorVisitValue[valueType];
                _il.CallVirt(visitValueMethod);
            }
            else if (extPropertyType.Class == TypeClass.Dictionary) {
                var container = extPropertyType.Container.AsDictionary();
                _il.Snippets.GetPropertyValue(graphVariable, context.Property);

                var dictionaryType = container.DictionaryInterfaceType;
                var cLocal = _il.DeclareLocal("dictionary", dictionaryType);
                if (dictionaryType != extPropertyType.Inner)
                    _il.Cast(dictionaryType);
                _il.Var.Set(cLocal);

                var visitArgsDictionaryMethod = Members.VisitArgsDictionary.MakeGenericMethod(container.KeyType, container.ValueType);
                var callVisitArgsDictionaryMethod = new CallMethodILCode(visitArgsDictionaryMethod, context.Property.Name, context.Index, cLocal);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, callVisitArgsDictionaryMethod);

                GenerateDictionaryCode(cLocal, container.ElementType);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave);
            }
            else if (extPropertyType.Class == TypeClass.Collection) {
                var container = extPropertyType.Container.AsCollection();
                _il.Snippets.GetPropertyValue(graphVariable, context.Property);

                var collectionType = container.CollectionInterfaceType;
                var cLocal = _il.DeclareLocal("collection", collectionType);
                if (collectionType != extPropertyType.Inner)
                    _il.Cast(collectionType);
                _il.Var.Set(cLocal);

                var visitArgsCollectionMethod = Members.VisitArgsCollection.MakeGenericMethod(container.ElementType);
                var callVisitArgsCollectionMethod = new CallMethodILCode(visitArgsCollectionMethod, context.Property.Name, context.Index, cLocal);
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, callVisitArgsCollectionMethod);

                GenerateCollectionCode(cLocal);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave);
            }
            else {
                var singleLocal = _il.DeclareLocal("single", context.Property.PropertyType);
                _il.Snippets.GetPropertyValue(graphVariable, context.Property);
                _il.Var.Set(singleLocal);

                var callVisitArgsMethod = new CallMethodILCode(Members.VisitArgsSingle, context.Property.Name, context.Index, singleLocal);
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorVisit, callVisitArgsMethod);

                var checkIfNullLabel = _il.DefineLabel();
                _il.TransferIfNull(singleLocal, checkIfNullLabel);

                GenerateChildCall(singleLocal);

                _il.MarkLabel(checkIfNullLabel);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave);
            }
        }

        private void GenerateDictionaryCode(ILCodeVariable dictionary, Type elementType)
        {
            var transferIfNullLabel = _il.DefineLabel();
            _il.TransferIfNull(dictionary, transferIfNullLabel);

            var enumerateCode = new ILEnumerateCode(dictionary, (il, it) => {
                GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Key")), Members.VisitArgsDictionaryKey);
                GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Value")), Members.VisitArgsDictionaryValue);
            });
            _il.Generate(enumerateCode);

            _il.MarkLabel(transferIfNullLabel);
        }

        private void GenerateCollectionCode(ILCodeVariable collection)
        {
            var transferIfNullLabel = _il.DefineLabel();
            _il.TransferIfNull(collection, transferIfNullLabel);

            var enumerateCode = new ILEnumerateCode(collection, (il, it) => GenerateEnumerateContentCode(it, Members.VisitArgsCollectionItem));
            _il.Generate(enumerateCode);

            _il.MarkLabel(transferIfNullLabel);
        }

        private void GenerateEnumerateContentCode(ILCodeVariable variable, FieldInfo staticVisitArgsField)
        {
            var type = variable.VariableType;
            var extType = _il.TypeCache.Extend(type);

            if (extType.IsValueOrNullableOfValue()) {
                var isNullable = extType.Class == TypeClass.Nullable;
                _il.Var.Load(_visitorVariable);
                _il.Var.Load(variable);
                if (!isNullable && type.IsValueType) {
                    var container = _il.TypeCache.Extend(type.AsNullable())
                        .Container.AsNullable();

                    _il.Construct(container.Constructor);
                }
                _il.LoadStaticField(staticVisitArgsField);
                var visitValueMethod = Members.VisitorVisitValue[type];
                _il.CallVirt(visitValueMethod);
            }
            else if (extType.Class == TypeClass.Dictionary) {
                var container = extType.Container.AsDictionary();
                var elementType = container.ElementType;

                var dictionaryType = container.DictionaryInterfaceType;

                if (type == dictionaryType) {
                    var enumerableCode = new ILEnumerateCode(variable, (il, it) => {
                        GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Key")), Members.VisitArgsDictionaryKey);
                        GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Value")), Members.VisitArgsDictionaryValue);
                    });
                    _il.Generate(enumerableCode);
                }
                else {
                    _il.Var.Load(variable);
                    _il.Cast(dictionaryType);
                    var dictionaryLocal = _il.DeclareLocal("dictionary", dictionaryType);
                    _il.Var.Set(dictionaryLocal);

                    var part = new ILEnumerateCode(dictionaryLocal, (il, it) => {
                        GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Key")), Members.VisitArgsDictionaryKey);
                        GenerateEnumerateContentCode(new InstancePropertyILCodeVariable(it, elementType.GetProperty("Value")), Members.VisitArgsDictionaryValue);
                    });
                    _il.Generate(part);
                }
            }
            else if (extType.Class == TypeClass.Collection) {
                var container = extType.Container.AsCollection();
                var collectionType = container.CollectionInterfaceType;
                if (type == collectionType) {
                    var part = new ILEnumerateCode(variable, (il, it) => GenerateEnumerateContentCode(it, Members.VisitArgsCollectionItem));
                    _il.Generate(part);
                }
                else {
                    _il.Var.Load(variable);
                    _il.Cast(collectionType);
                    var collectionLocal = _il.DeclareLocal("collection", collectionType);
                    _il.Var.Set(collectionLocal);

                    var part = new ILEnumerateCode(collectionLocal, (il, it) => GenerateEnumerateContentCode(it, Members.VisitArgsCollectionItem));
                    _il.Generate(part);
                }
            }
            else {
                _il.Var.Load(_visitorVariable);
                _il.LoadStaticField(staticVisitArgsField);
                _il.CallVirt(Members.VisitorVisit);

                GenerateChildCall(variable);

                _il.Var.Load(_visitorVariable);
                _il.CallVirt(Members.VisitorLeave);
            }
        }

        private void GenerateChildCall(ILCodeVariable child)
        {
            ChildTravellerInfo childTravellerInfo;
            if (!_childTravellers.TryGetValue(child.VariableType, out childTravellerInfo))
                throw InvalidGraphException.ComplexTypeWithoutTravellerDefined(child.VariableType);

            var field = _il.Var.Field(_il.Var.This(), childTravellerInfo.Field);
            _il.Snippets.InvokeMethod(field, childTravellerInfo.TravelWriteMethod, _visitorVariable, child);
        }

        
    }
}