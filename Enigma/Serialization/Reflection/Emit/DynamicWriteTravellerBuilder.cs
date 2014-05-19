using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly Dictionary<Type, ChildTravellerInfo> _childTravellers;
        private readonly ILExpressed _il;

        public DynamicWriteTravellerBuilder(MethodBuilder builder, TypeReflectionContext context, Dictionary<Type, ChildTravellerInfo> childTravellers)
        {
            _context = context;
            _childTravellers = childTravellers;
            _il = builder.IL;
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
                _il.LoadArgs(1);
                _il.LoadVar(graphVariable);
                _il.CallVirt(context.Property.GetGetMethod());
                if (isEnum) {
                    valueType = extPropertyType.GetUnderlyingEnumType();
                    if (extPropertyType.Class != TypeClass.Nullable)
                        _il.Construct(Members.NullableConstructors[valueType]);
                }
                else if (!isNullable && context.Property.PropertyType.IsValueType)
                    _il.Construct(Members.NullableConstructors[extPropertyType.Inner]);

                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);

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
                _il.LoadArgs(1);
                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);
                _il.LoadLocal(cLocal);

                var visitArgsDictionaryMethod = Members.VisitArgsDictionary.MakeGenericMethod(keyType, valueType);
                _il.Call(visitArgsDictionaryMethod);
                _il.CallVirt(Members.VisitorVisit);

                GenerateDictionaryCode(new LocalVariable(cLocal), container.ElementType, keyType, valueType);

                _il.LoadArgs(1);
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
                _il.LoadArgs(1);
                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);
                _il.LoadLocal(cLocal);

                var visitArgsCollectionMethod = Members.VisitArgsCollection.MakeGenericMethod(elementType);
                _il.Call(visitArgsCollectionMethod);
                _il.CallVirt(Members.VisitorVisit);

                GenerateCollectionCode(new LocalVariable(cLocal), elementType);

                _il.LoadArgs(1);
                _il.CallVirt(Members.VisitorLeave);
            }
            else {
                var singleLocal = _il.DeclareLocal("single", context.Property.PropertyType);
                _il.LoadVar(graphVariable);
                _il.CallVirt(context.Property.GetGetMethod());
                _il.SetLocal(singleLocal);
                _il.LoadArgs(1);
                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);
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

                _il.LoadArgs(1);
                _il.CallVirt(Members.VisitorLeave);
            }
        }

        private void GenerateDictionaryCode(IVariable dictionary, Type elementType, Type keyType, Type valueType)
        {
            var checkLocal = _il.DeclareLocal("check", typeof(bool));

            var checkIfNullLabel = GenerateReferenceNullCheck(dictionary, checkLocal);

            GenerateEnumerateCode(checkLocal, dictionary, elementType, itVarLocal => {
                GenerateEnumerateContentCode(new InstancePropertyVariable(itVarLocal, elementType.GetProperty("Key")));
                GenerateEnumerateContentCode(new InstancePropertyVariable(itVarLocal, elementType.GetProperty("Value")));
            });

            _il.MarkLabel(checkIfNullLabel);
        }

        private void GenerateCollectionCode(IVariable collection, Type elementType)
        {
            var checkLocal = _il.DeclareLocal("check", typeof(bool));

            var checkIfNullLabel = GenerateReferenceNullCheck(collection, checkLocal);

            GenerateEnumerateCode(checkLocal, collection, elementType, GenerateEnumerateContentCode);

            _il.MarkLabel(checkIfNullLabel);
        }

        private void GenerateEnumerateContentCode(IVariable variable)
        {
            var type = variable.VariableType;
            var extType = type.Extend();

            if (extType.IsValueOrNullableOfValue()) {
                var isNullable = extType.Class == TypeClass.Nullable;
                _il.LoadArgs(1);
                _il.LoadVar(variable);
                if (!isNullable && type.IsValueType)
                    _il.Construct(Members.NullableConstructors[type]);
                _il.LoadStaticField(Members.VisitArgsCollectionItem);
                var visitValueMethod = Members.VisitorVisitValue[type];
                _il.CallVirt(visitValueMethod);
            }
            else if (extType.Class == TypeClass.Dictionary) {
                var container = extType.Container.AsDictionary();
                var elementType = container.ElementType;
                var keyType = container.KeyType;
                var valueType = container.ValueType;

                var dictionaryType = typeof(IDictionary<,>).MakeGenericType(keyType, valueType);
                var checkLocal = _il.DeclareLocal("check", typeof(bool));

                if (type == dictionaryType)
                    GenerateEnumerateCode(checkLocal, variable, elementType, itVarLocal => {
                        GenerateEnumerateContentCode(new InstancePropertyVariable(itVarLocal, elementType.GetProperty("Key")));
                        GenerateEnumerateContentCode(new InstancePropertyVariable(itVarLocal, elementType.GetProperty("Value")));
                    });
                else {
                    _il.LoadVar(variable);
                    _il.Cast(dictionaryType);
                    var dictionaryLocal = _il.DeclareLocal("dictionary", dictionaryType);
                    _il.SetLocal(dictionaryLocal);

                    GenerateEnumerateCode(checkLocal, new LocalVariable(dictionaryLocal), elementType, itVarLocal => {
                        GenerateEnumerateContentCode(new InstancePropertyVariable(itVarLocal, elementType.GetProperty("Key")));
                        GenerateEnumerateContentCode(new InstancePropertyVariable(itVarLocal, elementType.GetProperty("Value")));
                    });
                }
            }
            else if (extType.Class == TypeClass.Collection) {
                var elementType = extType.Container.AsCollection().ElementType;
                var collectionType = typeof (ICollection<>).MakeGenericType(elementType);
                var checkLocal = _il.DeclareLocal("check", typeof(bool));
                if (type == collectionType)
                    GenerateEnumerateCode(checkLocal, variable, elementType, GenerateEnumerateContentCode);
                else {
                    _il.LoadVar(variable);
                    _il.Cast(collectionType);
                    var collectionLocal = _il.DeclareLocal("collection", collectionType);
                    _il.SetLocal(collectionLocal);
                    GenerateEnumerateCode(checkLocal, new LocalVariable(collectionLocal), elementType, GenerateEnumerateContentCode);
                }
            }
            else {
                _il.LoadArgs(1);
                _il.LoadStaticField(Members.VisitArgsCollectionItem);
                _il.CallVirt(Members.VisitorVisit);

                GenerateChildCall(variable);

                _il.LoadArgs(1);
                _il.CallVirt(Members.VisitorLeave);
            }
        }

        private Label GenerateReferenceNullCheck(IVariable reference, LocalBuilder checkLocal)
        {
            var checkIfNullLabel = _il.DefineLabel();
            _il.LoadVar(reference);
            _il.LoadNull();
            _il.CompareEquals();
            _il.SetLocal(checkLocal);
            _il.LoadLocal(checkLocal);
            _il.TransferLongIfTrue(checkIfNullLabel);
            return checkIfNullLabel;
        }

        private void GenerateEnumerateCode(LocalBuilder checkLocal, IVariable enumerable, Type elementType, Action<IVariable> enumerateBody)
        {
            _il.LoadVar(enumerable);

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var getEnumeratorMethod = enumerableType.GetMethod("GetEnumerator");
            _il.CallVirt(getEnumeratorMethod);

            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);
            var itLocal = _il.DeclareLocal("it", enumeratorType);
            _il.SetLocal(itLocal);

            _il.Try();

            var itHeadLabel = _il.DefineLabel();
            var itBodyLabel = _il.DefineLabel();
            _il.TransferShort(itHeadLabel);
            var itVarLocal = _il.DeclareLocal("cv", elementType);
            var getCurrentMethod = enumeratorType.GetProperty("Current").GetGetMethod();
            _il.MarkLabel(itBodyLabel);
            _il.LoadLocal(itLocal);
            _il.Call(getCurrentMethod);
            _il.SetLocal(itVarLocal);

            enumerateBody.Invoke(new LocalVariable(itVarLocal));

            _il.MarkLabel(itHeadLabel);
            _il.LoadLocal(itLocal);
            _il.CallVirt(Members.EnumeratorMoveNext);

            _il.SetLocal(checkLocal);
            _il.LoadLocal(checkLocal);
            _il.TransferShortIfTrue(itBodyLabel);

            _il.Finally();
            _il.LoadLocal(itLocal);
            _il.LoadNull();
            _il.CompareEquals();
            _il.SetLocal(checkLocal);
            _il.LoadLocal(checkLocal);

            var endLabel = _il.DefineLabel();
            _il.TransferShortIfTrue(endLabel);

            _il.LoadLocal(itLocal);
            _il.CallVirt(Members.DisposableDispose);

            _il.MarkLabel(endLabel);
            _il.EndTry();
        }

        private void GenerateChildCall(IVariable child)
        {
            ChildTravellerInfo childTravellerInfo;
            if (!_childTravellers.TryGetValue(child.VariableType, out childTravellerInfo))
                throw InvalidGraphException.ComplexTypeWithoutTravellerDefined(child.VariableType);

            _il.LoadThis();
            _il.LoadField(childTravellerInfo.Field);
            _il.LoadArgs(1);
            _il.LoadVar(child);
            _il.CallVirt(childTravellerInfo.TravelWriteMethod);
        }

        
    }
}