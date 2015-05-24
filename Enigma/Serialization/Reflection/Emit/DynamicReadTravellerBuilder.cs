using System;
using Enigma.Reflection;
using Enigma.Reflection.Emit;
using MethodBuilder = Enigma.Reflection.Emit.MethodBuilder;

namespace Enigma.Serialization.Reflection.Emit
{
    public class DynamicReadTravellerBuilder
    {

        private static readonly DynamicReadTravellerMembers Members = new DynamicReadTravellerMembers();

        private readonly SerializableType _target;
        private readonly TravellerContext _context;
        private readonly ILExpressed _il;
        private readonly ILCodeVariable _visitorVariable;

        public DynamicReadTravellerBuilder(MethodBuilder builder, SerializableType target, TravellerContext context)
        {
            _target = target;
            _context = context;
            _il = builder.IL;
            _visitorVariable = new MethodArgILCodeVariable(1, typeof(IReadVisitor));
        }

        public void BuildTravelReadMethod()
        {
            var graphArgument = new MethodArgILCodeVariable(2, _target.Type);
            foreach (var property in _target.Properties) {
                GeneratePropertyCode(graphArgument, property);
            }
            _il.Return();
        }

        private void GeneratePropertyCode(MethodArgILCodeVariable graphVariable, SerializableProperty target)
        {
            var extPropertyType = target.Ext;
            var argsField = _context.GetArgsField(target);
            var argsFieldVariable = _il.Var.Field(_il.Var.This(), argsField);
            if (extPropertyType.IsValueOrNullableOfValue()) {
                var isNullable = extPropertyType.Class == TypeClass.Nullable;
                var isEnum = extPropertyType.IsEnum();
                var isValueType = extPropertyType.Ref.IsValueType;

                var mediatorPropertyType = isEnum ? extPropertyType.GetUnderlyingEnumType() : extPropertyType.Ref;
                var valueType = !isNullable && isValueType ? Members.Nullable[mediatorPropertyType].NullableType : mediatorPropertyType;

                var valueLocal = _il.DeclareLocal("value", valueType);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorTryVisitValue[valueType], argsFieldVariable, valueLocal);

                if (isValueType && !isNullable) {
                    var labelValueNotFound = _il.DefineLabel();
                    _il.TransferLongIfFalse(labelValueNotFound);

                    _il.Snippets.InvokeMethod(valueLocal, Members.Nullable[mediatorPropertyType].GetHasValue);
                    _il.Negate();

                    var nullableHasValueLabel = _il.DefineLabel();
                    _il.TransferLong(nullableHasValueLabel);

                    _il.MarkLabel(labelValueNotFound);
                    _il.LoadValue(true);
                    _il.MarkLabel(nullableHasValueLabel);
                }
                else {
                    _il.Negate();
                }

                var skipSetValueLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(skipSetValueLabel);

                var valueToAdd = (isValueType && !isNullable)
                    ? new CallMethodILCode(valueLocal, Members.Nullable[mediatorPropertyType].GetValue)
                    : (ILCodeParameter)valueLocal;

                _il.Snippets.SetPropertyValue(graphVariable, target.Ref, valueToAdd);

                _il.MarkLabel(skipSetValueLabel);
            }
            else if (extPropertyType.Class == TypeClass.Dictionary) {
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorTryVisit, argsFieldVariable);
                var stateLocal = _il.DeclareLocal("state", typeof(ValueState));
                _il.Var.Set(stateLocal);
                _il.Snippets.AreEqual(stateLocal, (int)ValueState.NotFound);

                var endLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(endLabel);

                _il.Snippets.AreEqual(stateLocal, (int)ValueState.Found);

                var nullLabel = _il.DefineLabel();
                _il.TransferLongIfFalse(nullLabel);

                var dictionaryLocal = GenerateDictionaryEnumerateCode(target.Ref.PropertyType, target.Ref.Name);

                _il.Snippets.SetPropertyValue(graphVariable, target.Ref, dictionaryLocal.Cast(target.Ref.PropertyType));

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, argsFieldVariable);

                _il.TransferLong(endLabel);

                _il.MarkLabel(nullLabel);
                _il.Snippets.SetPropertyValue(graphVariable, target.Ref, null);

                _il.MarkLabel(endLabel);
            }
            else if (extPropertyType.Class == TypeClass.Collection) {
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorTryVisit, argsFieldVariable);
                var stateLocal = _il.DeclareLocal("state", typeof(ValueState));
                _il.Var.Set(stateLocal);

                _il.Snippets.AreEqual(stateLocal, (int)ValueState.NotFound);

                var endLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(endLabel);

                _il.Snippets.AreEqual(stateLocal, (int)ValueState.Found);

                var nullLabel = _il.DefineLabel();
                _il.TransferLongIfFalse(nullLabel);

                var collectionParam = GenerateCollectionContent(extPropertyType, target.Ref.Name);

                _il.Snippets.SetPropertyValue(graphVariable, target.Ref, collectionParam);
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, argsFieldVariable);
                _il.TransferLong(endLabel);

                _il.MarkLabel(nullLabel);
                _il.Snippets.SetPropertyValue(graphVariable, target.Ref, null);

                _il.MarkLabel(endLabel);
            }
            else {
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorTryVisit, argsFieldVariable);
                var stateLocal = _il.DeclareLocal("state", typeof(ValueState));
                _il.Var.Set(stateLocal);

                _il.IfNotEqual(stateLocal, (int)ValueState.NotFound)
                    .Then(() => {
                        _il.IfEqual(stateLocal, (int)ValueState.Found)
                            .Then(() => {
                                var singleLocal = _il.DeclareLocal("single", extPropertyType.Ref);
                                GenerateCreateAndChildCallCode(singleLocal);

                                _il.Snippets.SetPropertyValue(graphVariable, target.Ref, singleLocal);
                                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, argsFieldVariable);
                            }).Else(() => {
                                _il.Snippets.SetPropertyValue(graphVariable, target.Ref, null);
                            }).End();
                    }).End();
            }
        }

        private ILCodeParameter GenerateCollectionContent(ExtendedType target, string refName)
        {
            var collectionMembers = new CollectionMembers(target);
            var isValueType = collectionMembers.ElementType.IsValueType;

            var collectionLocal = _il.DeclareLocal("collection", collectionMembers.VariableType);
            _il.Construct(collectionMembers.Constructor);
            _il.Cast(collectionMembers.VariableType);
            _il.Var.Set(collectionLocal);

            var valueLocal = DeclareCollectionItemLocal("cv", collectionMembers.ElementType); ;

            if (collectionMembers.ElementTypeExt.IsValueOrNullableOfValue()) {
                _il.Snippets.WhileLoop(il => { // While condition
                    _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorTryVisitValue[collectionMembers.ElementType], Members.VisitArgsCollectionItem, valueLocal);

                    var valueNotFoundLabel = _il.DefineLabel();
                    _il.TransferLongIfFalse(valueNotFoundLabel);

                    if (isValueType) {
                        _il.Snippets.InvokeMethod(valueLocal, Members.Nullable[collectionMembers.ElementType].GetHasValue);
                    }
                    else {
                        _il.Snippets.AreEqual(valueLocal, ILCodeParameter.Null);
                        _il.Negate();
                    }

                    var isNullLabel = _il.DefineLabel();
                    _il.TransferLong(isNullLabel);

                    _il.MarkLabel(valueNotFoundLabel);
                    _il.LoadValue(0);
                    _il.MarkLabel(isNullLabel);
                }, il => {
                    _il.Var.Load(collectionLocal);

                    GenerateLoadParamValueCode(valueLocal);

                    _il.CallVirt(collectionMembers.Add);
                });
            }
            else if (collectionMembers.ElementTypeExt.Class == TypeClass.Dictionary) {
                _il.Snippets.WhileLoop(il => { // Condition
                    var callTryVisit = new CallMethodILCode(_visitorVariable, Members.VisitorTryVisit, Members.VisitArgsDictionaryInCollection);
                    _il.Snippets.AreEqual(callTryVisit, (int)ValueState.Found);
                }, il => {
                    var contentParam = GenerateDictionaryEnumerateCode(collectionMembers.ElementType, refName);
                    _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, Members.VisitArgsDictionaryInCollection);
                    _il.Snippets.InvokeMethod(collectionLocal, collectionMembers.Add, contentParam);
                });
            }
            else if (collectionMembers.ElementTypeExt.Class == TypeClass.Collection) {
                _il.Snippets.WhileLoop(il => { // Condition
                    var callTryVisit = new CallMethodILCode(_visitorVariable, Members.VisitorTryVisit, Members.VisitArgsCollectionInCollection);
                    _il.Snippets.AreEqual(callTryVisit, (int)ValueState.Found);
                }, il => {
                    var contentParam = GenerateCollectionContent(collectionMembers.ElementTypeExt, refName);
                    _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, Members.VisitArgsCollectionInCollection);
                    _il.Snippets.InvokeMethod(collectionLocal, collectionMembers.Add, contentParam);
                });
            }
            else {
                _il.Snippets.WhileLoop(il => {
                    var callTryVisit = new CallMethodILCode(_visitorVariable, Members.VisitorTryVisit, Members.VisitArgsCollectionItem);
                    _il.Snippets.AreEqual(callTryVisit, (int)ValueState.Found);
                }, il => {
                    GenerateCreateAndChildCallCode(valueLocal);
                    _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, Members.VisitArgsCollectionItem);
                    _il.Snippets.InvokeMethod(collectionLocal, collectionMembers.Add, valueLocal);
                });
            }
            if (target.Ref.IsArray)
                return new CallMethodILCode(collectionMembers.ToArray, collectionLocal);

            return collectionLocal;
        }

        private LocalILCodeVariable GenerateDictionaryEnumerateCode(Type type, string refName)
        {
            var extType = _il.TypeCache.Extend(type);
            var dictionaryMembers = new DictionaryMembers(extType);

            var local = _il.DeclareLocal("dictionary", dictionaryMembers.VariableType);
            _il.Construct(dictionaryMembers.Constructor);
            _il.Var.Set(local);

            NullableMembers keyNullableMembers;
            ILCodeParameter keyParam;

            var conditionLabel = _il.DefineLabel();
            var bodyLabel = _il.DefineLabel();
            var keyType = Members.Nullable.TryGetValue(dictionaryMembers.KeyType, out keyNullableMembers) ? keyNullableMembers.NullableType : dictionaryMembers.KeyType;
            var keyTypeExt = _il.TypeCache.Extend(keyType);

            // First of all, transfer to the condition part
            _il.TransferLong(conditionLabel);

            // Mark that we enter the body of the loop
            _il.MarkLabel(bodyLabel);
            if (keyTypeExt.IsValueOrNullableOfValue()) {
                keyParam = _il.DeclareLocal("ck", keyType);
            }
            else if (keyTypeExt.Class == TypeClass.Dictionary) {
                keyParam = GenerateDictionaryEnumerateCode(keyType, refName);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, Members.VisitArgsDictionaryInDictionaryKey);
            }
            else if (keyTypeExt.Class == TypeClass.Collection) {
                keyParam = GenerateCollectionContent(keyTypeExt, refName);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, Members.VisitArgsCollectionInDictionaryKey);
            }
            else {
                var keyLocal = _il.DeclareLocal("ck", keyType);
                GenerateCreateAndChildCallCode(keyLocal);
                keyParam = keyLocal;
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, Members.VisitArgsDictionaryKey);
            }

            var throwExceptionLabel = _il.DefineLabel();

            ILCodeParameter valueParam;

            NullableMembers valueNullableMembers;
            var hasNullableMembers = Members.Nullable.TryGetValue(dictionaryMembers.ValueType, out valueNullableMembers);
            var valueType = dictionaryMembers.ValueType;
            var extValueType = valueType.Extend();

            if (extValueType.IsValueOrNullableOfValue()) {
                var loadTrueLabel = _il.DefineLabel();
                var checkIfErrorLabel = _il.DefineLabel();

                valueParam = _il.DeclareLocal("cv", hasNullableMembers ? valueNullableMembers.NullableType : dictionaryMembers.ValueType);
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorTryVisitValue[dictionaryMembers.ValueType], Members.VisitArgsDictionaryValue, valueParam);
                _il.TransferLongIfFalse(loadTrueLabel);

                if (hasNullableMembers) {
                    _il.Snippets.InvokeMethod(valueParam, valueNullableMembers.GetHasValue);
                    _il.Negate();
                }
                else {
                    _il.Snippets.AreEqual(valueParam, null);
                }
                _il.TransferLong(checkIfErrorLabel);
                _il.MarkLabel(loadTrueLabel);
                _il.LoadValue(true);
                _il.MarkLabel(checkIfErrorLabel);
                _il.TransferLongIfTrue(throwExceptionLabel);
            }
            else if (extValueType.Class == TypeClass.Dictionary) {
                var callTryVisitValue = new CallMethodILCode(_visitorVariable, Members.VisitorTryVisit, Members.VisitArgsDictionaryInDictionaryValue);
                _il.Snippets.AreEqual(callTryVisitValue, (int)ValueState.Found);
                _il.TransferLongIfFalse(throwExceptionLabel);

                valueParam = GenerateDictionaryEnumerateCode(valueType, refName);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, Members.VisitArgsDictionaryInDictionaryValue);
            }
            else if (extValueType.Class == TypeClass.Collection) {
                var callTryVisit = new CallMethodILCode(_visitorVariable, Members.VisitorTryVisit, Members.VisitArgsCollectionInDictionaryValue);
                _il.Snippets.AreEqual(callTryVisit, (int)ValueState.Found);
                _il.TransferLongIfFalse(throwExceptionLabel);

                valueParam = GenerateCollectionContent(_il.TypeCache.Extend(valueType), refName);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, Members.VisitArgsCollectionInDictionaryValue);
            }
            else {
                var callTryVisit = new CallMethodILCode(_visitorVariable, Members.VisitorTryVisit, Members.VisitArgsDictionaryValue);
                _il.Snippets.AreEqual(callTryVisit, (int)ValueState.Found);
                _il.TransferLongIfFalse(throwExceptionLabel);

                var valueLocal = _il.DeclareLocal("cv", hasNullableMembers ? valueNullableMembers.NullableType : dictionaryMembers.ValueType);
                GenerateCreateAndChildCallCode(valueLocal);
                valueParam = valueLocal;

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, Members.VisitArgsDictionaryValue);
            }

            _il.Var.Load(local);
            GenerateLoadParamValueCode(keyParam);
            GenerateLoadParamValueCode(valueParam);

            _il.CallVirt(dictionaryMembers.Add);
            _il.TransferLong(conditionLabel);

            _il.MarkLabel(throwExceptionLabel);
            _il.Snippets.Throw(new CallMethodILCode(Members.ExceptionNoDictionaryValue, refName));

            _il.MarkLabel(conditionLabel);

            if (keyTypeExt.IsValueOrNullableOfValue()) {
                var loadZeroLabel = _il.DefineLabel();
                var checkConditionLabel = _il.DefineLabel();

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorTryVisitValue[keyType], Members.VisitArgsDictionaryKey, keyParam);
                _il.TransferLongIfFalse(loadZeroLabel);

                if (keyType.IsValueType) {
                    _il.Snippets.InvokeMethod(keyParam, Members.Nullable[keyType].GetHasValue);
                    _il.TransferLong(checkConditionLabel);
                }
                else {
                    _il.Snippets.AreEqual(keyParam, ILCodeParameter.Null);
                    _il.Negate();
                    _il.TransferLong(checkConditionLabel);
                }

                _il.MarkLabel(loadZeroLabel);
                _il.LoadValue(false);
                _il.MarkLabel(checkConditionLabel);
                _il.TransferLongIfTrue(bodyLabel);

            }
            else if (keyTypeExt.Class == TypeClass.Dictionary) {
                var callTryVisitKey = new CallMethodILCode(_visitorVariable, Members.VisitorTryVisit, Members.VisitArgsDictionaryInDictionaryKey);
                _il.Snippets.AreEqual(callTryVisitKey, (int)ValueState.Found);
                _il.TransferLongIfTrue(bodyLabel);
            }
            else if (keyTypeExt.Class == TypeClass.Collection) {
                var callTryVisitKey = new CallMethodILCode(_visitorVariable, Members.VisitorTryVisit, Members.VisitArgsCollectionInDictionaryKey);
                _il.Snippets.AreEqual(callTryVisitKey, (int)ValueState.Found);
                _il.TransferLongIfTrue(bodyLabel);
            }
            else {
                var callTryVisit = new CallMethodILCode(_visitorVariable, Members.VisitorTryVisit, Members.VisitArgsDictionaryKey);
                _il.Snippets.AreEqual(callTryVisit, (int)ValueState.Found);
                _il.TransferLongIfTrue(bodyLabel);
            }


            return local;
        }

        private void GenerateInnerValue()
        {

        }

        private ILCodeVariable DeclareCollectionItemLocal(string name, Type type)
        {
            var isValueType = type.IsValueType;
            var valueLocal = _il.DeclareLocal(name, isValueType ? Members.Nullable[type].NullableType : type);
            return valueLocal;
        }

        private void GenerateLoadParamValueCode(ILCodeParameter param)
        {
            var type = param.ParameterType;
            var extType = type.Extend();
            if (extType.Class == TypeClass.Nullable)
                type = extType.Container.AsNullable().ElementType;

            if (type.IsValueType)
                _il.Snippets.InvokeMethod(param, Members.Nullable[type].GetValue);
            else
                _il.Var.Load(param);
        }

        private void GenerateCreateAndChildCallCode(ILCodeVariable local)
        {
            var type = local.VariableType;

            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw InvalidGraphException.NoParameterLessConstructor(type);

            _il.Construct(constructor);
            _il.Var.Set(local);

            var childTravellerInfo = _context.GetTraveller(type);

            var field = _il.Var.Field(_il.Var.This(), childTravellerInfo.Field);
            _il.Snippets.InvokeMethod(field, childTravellerInfo.TravelReadMethod, _visitorVariable, local);
        }
    }
}
