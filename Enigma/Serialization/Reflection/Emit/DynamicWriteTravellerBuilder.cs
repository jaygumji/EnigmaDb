using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using Enigma.Reflection;
using Enigma.Reflection.Emit;
using MethodBuilder = Enigma.Reflection.Emit.MethodBuilder;

namespace Enigma.Serialization.Reflection.Emit
{
    public class DynamicWriteTravellerBuilder
    {
        private static readonly DynamicWriteTravellerMembers Members = new DynamicWriteTravellerMembers();

        private readonly TypeReflectionContext _context;
        private readonly IVariable _visitorVariable;
        private readonly Dictionary<Type, ChildTravellerInfo> _childTravellers;
        private readonly ILExpressed _il;

        public DynamicWriteTravellerBuilder(MethodBuilder builder, TypeReflectionContext context, Dictionary<Type, ChildTravellerInfo> childTravellers)
        {
            _context = context;
            _childTravellers = childTravellers;
            _il = builder.IL;
            _visitorVariable = new MethodArgVariable(1, typeof(IWriteVisitor));
        }

        public void BuildTravelWriteMethod()
        {
            var graphArgument = new MethodArgVariable(2, _context.Type);
            foreach (var property in _context.SerializableProperties) {
                GeneratePropertyCode(graphArgument, property);
            }
            _il.Return();
        }

        private void GeneratePropertyCode(IVariable graphVariable, PropertyReflectionContext context)
        {
            var extPropertyType = context.PropertyTypeContext.Extended;
            //if (extPropertyType.Class == TypeClass.Dictionary) return;
            //if (extPropertyType.Class == TypeClass.Complex) return;
            //if (extPropertyType.Class == TypeClass.Collection) return;
            //if (extPropertyType.Class == TypeClass.Value) return;
            if (extPropertyType.IsValueOrNullableOfValue()) {
                var isNullable = extPropertyType.Class == TypeClass.Nullable;
                var isEnum = extPropertyType.IsEnum();

                var valueType = extPropertyType.Inner;
                _il.LoadVar(_visitorVariable);
                _il.Snippets.GetPropertyValue(graphVariable, context.Property);

                if (isEnum) {
                    valueType = extPropertyType.GetUnderlyingEnumType();
                    if (extPropertyType.Class != TypeClass.Nullable)
                        _il.Construct(Members.NullableConstructors[valueType]);
                }
                else if (!isNullable && context.Property.PropertyType.IsValueType)
                    _il.Snippets.AsNullable(valueType);

                _il.LoadValue(context.Property.Name);
                _il.LoadValue(context.Index);

                if (isEnum) {
                    _il.LoadVar(graphVariable);
                    _il.CallVirt(context.Property.GetGetMethod());
                    _il.Call(Members.VisitArgsEnumValue.MakeGenericMethod(valueType));
                }
                else if (isNullable)
                    _il.Call(Members.VisitArgsNullableValue);
                else
                    _il.Call(Members.VisitArgsValue);

                var visitValueMethod = Members.VisitorVisitValue[valueType];
                _il.CallVirt(visitValueMethod);
            }
            else if (extPropertyType.Class == TypeClass.Dictionary) {
                var container = extPropertyType.Container.AsDictionary();
                var keyType = container.KeyType;
                var valueType = container.ValueType;

                _il.LoadVar(graphVariable);
                _il.CallVirt(context.Property.GetGetMethod());

                var dictionaryType = typeof (IDictionary<,>).MakeGenericType(keyType, valueType);
                var cLocal = _il.DeclareLocal("dictionary", dictionaryType);
                if (dictionaryType != extPropertyType.Inner)
                    _il.Cast(dictionaryType);
                _il.SetLocal(cLocal);
                _il.LoadVar(_visitorVariable);
                _il.LoadValue(context.Property.Name);
                _il.LoadValue(context.Index);
                _il.LoadLocal(cLocal);

                var visitArgsDictionaryMethod = Members.VisitArgsDictionary.MakeGenericMethod(keyType, valueType);
                _il.Call(visitArgsDictionaryMethod);
                _il.CallVirt(Members.VisitorVisit);

                GenerateDictionaryCode(new LocalVariable(cLocal), container.ElementType);

                _il.LoadVar(_visitorVariable);
                _il.CallVirt(Members.VisitorLeave);
            }
            else if (extPropertyType.Class == TypeClass.Collection) {
                var elementType = extPropertyType.Container.AsCollection().ElementType;
                _il.LoadVar(graphVariable);
                _il.CallVirt(context.Property.GetGetMethod());

                var collectionType = typeof (ICollection<>).MakeGenericType(elementType);
                var cLocal = _il.DeclareLocal("collection", collectionType);
                if (collectionType != extPropertyType.Inner)
                    _il.Cast(collectionType);
                _il.SetLocal(cLocal);
                _il.LoadVar(_visitorVariable);
                _il.LoadValue(context.Property.Name);
                _il.LoadValue(context.Index);
                _il.LoadLocal(cLocal);

                var visitArgsCollectionMethod = Members.VisitArgsCollection.MakeGenericMethod(elementType);
                _il.Call(visitArgsCollectionMethod);
                _il.CallVirt(Members.VisitorVisit);

                GenerateCollectionCode(new LocalVariable(cLocal));

                _il.LoadVar(_visitorVariable);
                _il.CallVirt(Members.VisitorLeave);
            }
            else {
                var singleLocal = _il.DeclareLocal("single", context.Property.PropertyType);
                _il.LoadVar(graphVariable);
                _il.CallVirt(context.Property.GetGetMethod());
                _il.SetLocal(singleLocal);
                _il.LoadVar(_visitorVariable);
                _il.LoadValue(context.Property.Name);
                _il.LoadValue(context.Index);
                _il.LoadLocal(singleLocal);
                _il.Call(Members.VisitArgsSingle);
                _il.CallVirt(Members.VisitorVisit);

                var checkIfNullLabel = _il.DefineLabel();
                var checkLocal = _il.DeclareLocal("check", typeof(bool));
                _il.LoadLocal(singleLocal);
                _il.LoadNull();
                _il.CompareEquals();
                _il.SetLocal(checkLocal);
                _il.LoadLocal(checkLocal);
                _il.TransferLongIfTrue(checkIfNullLabel);

                GenerateChildCall(new LocalVariable(singleLocal));

                _il.MarkLabel(checkIfNullLabel);

                _il.LoadVar(_visitorVariable);
                _il.CallVirt(Members.VisitorLeave);
            }
        }

        private void GenerateDictionaryCode(IVariable dictionary, Type elementType)
        {
            var transferIfNullLabel = _il.DefineLabel();
            _il.TransferIfNull(dictionary, transferIfNullLabel);

            var enumerateCode = new ILEnumerateCode(dictionary, (il, it) => {
                GenerateEnumerateContentCode(new InstancePropertyVariable(it, elementType.GetProperty("Key")), Members.VisitArgsDictionaryKey);
                GenerateEnumerateContentCode(new InstancePropertyVariable(it, elementType.GetProperty("Value")), Members.VisitArgsDictionaryValue);
            });
            _il.Generate(enumerateCode);

            _il.MarkLabel(transferIfNullLabel);
        }

        private void GenerateCollectionCode(IVariable collection)
        {
            var transferIfNullLabel = _il.DefineLabel();
            _il.TransferIfNull(collection, transferIfNullLabel);

            var enumerateCode = new ILEnumerateCode(collection, (il, it) => GenerateEnumerateContentCode(it, Members.VisitArgsCollectionItem));
            _il.Generate(enumerateCode);

            _il.MarkLabel(transferIfNullLabel);
        }

        private void GenerateEnumerateContentCode(IVariable variable, FieldInfo staticVisitArgsField)
        {
            var type = variable.VariableType;
            var extType = _il.TypeCache.Extend(type);

            if (extType.IsValueOrNullableOfValue()) {
                var isNullable = extType.Class == TypeClass.Nullable;
                _il.LoadVar(_visitorVariable);
                _il.LoadVar(variable);
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
                        GenerateEnumerateContentCode(new InstancePropertyVariable(it, elementType.GetProperty("Key")), Members.VisitArgsDictionaryKey);
                        GenerateEnumerateContentCode(new InstancePropertyVariable(it, elementType.GetProperty("Value")), Members.VisitArgsDictionaryValue);
                    });
                    _il.Generate(enumerableCode);
                }
                else {
                    _il.LoadVar(variable);
                    _il.Cast(dictionaryType);
                    var dictionaryLocal = _il.DeclareLocal("dictionary", dictionaryType);
                    _il.SetLocal(dictionaryLocal);

                    var part = new ILEnumerateCode(new LocalVariable(dictionaryLocal), (il, it) => {
                        GenerateEnumerateContentCode(new InstancePropertyVariable(it, elementType.GetProperty("Key")), Members.VisitArgsDictionaryKey);
                        GenerateEnumerateContentCode(new InstancePropertyVariable(it, elementType.GetProperty("Value")), Members.VisitArgsDictionaryValue);
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
                    _il.LoadVar(variable);
                    _il.Cast(collectionType);
                    var collectionLocal = _il.DeclareLocal("collection", collectionType);
                    _il.SetLocal(collectionLocal);

                    var part = new ILEnumerateCode(new LocalVariable(collectionLocal), (il, it) => GenerateEnumerateContentCode(it, Members.VisitArgsCollectionItem));
                    _il.Generate(part);
                }
            }
            else {
                _il.LoadVar(_visitorVariable);
                _il.LoadStaticField(staticVisitArgsField);
                _il.CallVirt(Members.VisitorVisit);

                GenerateChildCall(variable);

                _il.LoadVar(_visitorVariable);
                _il.CallVirt(Members.VisitorLeave);
            }
        }

        private void GenerateChildCall(IVariable child)
        {
            ChildTravellerInfo childTravellerInfo;
            if (!_childTravellers.TryGetValue(child.VariableType, out childTravellerInfo))
                throw InvalidGraphException.ComplexTypeWithoutTravellerDefined(child.VariableType);

            _il.LoadThis();
            _il.LoadField(childTravellerInfo.Field);
            _il.LoadVar(_visitorVariable);
            _il.LoadVar(child);
            _il.CallVirt(childTravellerInfo.TravelWriteMethod);
        }

        
    }
}