using System;
using System.Collections.Generic;
using System.Reflection;
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
            //if (extPropertyType.Class == TypeClass.Dictionary) return;
            //if (extPropertyType.Class == TypeClass.Complex) return;
            //if (extPropertyType.Class == TypeClass.Collection) return;
            //if (extPropertyType.Class == TypeClass.Nullable) return;
            //if (extPropertyType.Class == TypeClass.Value) return;
            if (extPropertyType.IsValueOrNullableOfValue()) {
                var isNullable = extPropertyType.Class == TypeClass.Nullable;
                var isEnum = extPropertyType.IsEnum();
                var isValueType = extPropertyType.Inner.IsValueType;
                _il.Var.Load(_visitorVariable);
                _il.Var.Load(argsFieldVariable);

                var mediatorPropertyType = isEnum ? extPropertyType.GetUnderlyingEnumType() : extPropertyType.Inner;
                var valueType = !isNullable && isValueType ? Members.Nullable[mediatorPropertyType].NullableType : mediatorPropertyType;

                var valueLocal = _il.DeclareLocal("value", valueType);
                _il.Var.LoadAddress(valueLocal);
                _il.CallVirt(Members.VisitorTryVisitValue[valueType]);

                if (isValueType && !isNullable) {
                    var labelValueNotFound = _il.DefineLabel();
                    _il.TransferLongIfFalse(labelValueNotFound);

                    _il.Snippets.InvokeMethod(valueLocal, Members.Nullable[mediatorPropertyType].GetHasValue);
                    _il.Negate();

                    var nullableHasValueLabel = _il.DefineLabel();
                    _il.TransferLong(nullableHasValueLabel);

                    _il.MarkLabel(labelValueNotFound);
                    _il.LoadValue(1);
                    _il.MarkLabel(nullableHasValueLabel);
                }
                else {
                    _il.Negate();
                }

                var skipSetValueLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(skipSetValueLabel);

                _il.Var.Load(graphVariable);
                if (isValueType && !isNullable)
                    _il.Snippets.InvokeMethod(valueLocal, Members.Nullable[mediatorPropertyType].GetValue);
                else
                    _il.Var.Load(valueLocal);

                _il.CallVirt(target.Ref.GetSetMethod());

                _il.MarkLabel(skipSetValueLabel);
            }
            else if (extPropertyType.Class == TypeClass.Dictionary) {
                var dictionaryMembers = new DictionaryMembers(extPropertyType);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorTryVisit, argsFieldVariable);
                var stateLocal = _il.DeclareLocal("state", typeof(ValueState));
                _il.Var.Set(stateLocal);
                _il.Snippets.AreEqual(stateLocal, (int) ValueState.NotFound);

                var endLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(endLabel);

                _il.Snippets.AreEqual(stateLocal, (int) ValueState.Found);

                var nullLabel = _il.DefineLabel();
                _il.TransferLongIfFalse(nullLabel);

                var dictionaryLocal = _il.DeclareLocal("dictionary", dictionaryMembers.VariableType);
                _il.Construct(dictionaryMembers.Constructor);
                _il.Var.Set(dictionaryLocal);

                NullableMembers keyNullableMembers;
                var keyLocal = _il.DeclareLocal("ck", Members.Nullable.TryGetValue(dictionaryMembers.KeyType, out keyNullableMembers) ? keyNullableMembers.NullableType : dictionaryMembers.KeyType);
                GenerateEnumerateCode(keyLocal, Members.VisitArgsDictionaryKey, () => {
                    var throwExceptionLabel = _il.DefineLabel();
                    var endBodyLabel = _il.DefineLabel();
                    var argsDictValueVariable = _il.Var.Field(Members.VisitArgsDictionaryValue);

                    NullableMembers valueNullableMembers;
                    var hasNullableMembers = Members.Nullable.TryGetValue(dictionaryMembers.ValueType, out valueNullableMembers);
                    var valueLocal = _il.DeclareLocal("cv", hasNullableMembers ? valueNullableMembers.NullableType : dictionaryMembers.ValueType);
                    var valueType = dictionaryMembers.ValueType;
                    var extValueType = valueType.Extend();

                    if (extValueType.IsValueOrNullableOfValue()) {
                        var loadOneLabel = _il.DefineLabel();
                        var checkConditionLabel = _il.DefineLabel();

                        _il.Var.Load(_visitorVariable);
                        _il.LoadStaticField(Members.VisitArgsDictionaryValue);
                        _il.Var.LoadAddress(valueLocal);
                        _il.CallVirt(Members.VisitorTryVisitValue[dictionaryMembers.ValueType]);
                        _il.TransferLongIfFalse(loadOneLabel);

                        if (hasNullableMembers) {
                            _il.Var.LoadAddress(valueLocal);
                            _il.Call(valueNullableMembers.GetHasValue);
                            _il.Negate();
                        }
                        else {
                            _il.Snippets.AreEqual(valueLocal, ILCodeParameter.Null);
                        }
                        _il.TransferLong(checkConditionLabel);
                        _il.MarkLabel(loadOneLabel);
                        _il.LoadValue(1);
                        _il.MarkLabel(checkConditionLabel);
                        _il.TransferLongIfTrue(throwExceptionLabel);
                    }
                    else {
                        _il.Var.Load(_visitorVariable);
                        _il.LoadStaticField(Members.VisitArgsDictionaryValue);
                        _il.CallVirt(Members.VisitorTryVisit);
                        _il.LoadValue((int) ValueState.Found);
                        _il.CompareEquals();
                        _il.Negate();
                        _il.TransferLongIfTrue(throwExceptionLabel);

                        GenerateCreateAndChildCallCode(valueLocal);
                        _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, argsDictValueVariable);
                    }

                    _il.Var.Load(dictionaryLocal);
                    GenerateLoadLocalValueCode(keyLocal);
                    GenerateLoadLocalValueCode(valueLocal);

                    _il.CallVirt(dictionaryMembers.Add);
                    _il.TransferLong(endBodyLabel);

                    _il.MarkLabel(throwExceptionLabel);
                    _il.LoadValue(target.Ref.Name);
                    _il.Call(Members.ExceptionNoDictionaryValue);
                    _il.Throw();

                    _il.MarkLabel(endBodyLabel);
                });

                _il.Var.Load(graphVariable);
                _il.Var.Load(dictionaryLocal);
                if (dictionaryLocal.VariableType != target.Ref.PropertyType)
                    _il.Cast(target.Ref.PropertyType);
                _il.CallVirt(target.Ref.GetSetMethod());

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, argsFieldVariable);

                _il.TransferLong(endLabel);

                _il.MarkLabel(nullLabel);
                _il.Var.Load(graphVariable);
                _il.LoadNull();
                _il.CallVirt(target.Ref.GetSetMethod());

                _il.MarkLabel(endLabel);
            }
            else if (extPropertyType.Class == TypeClass.Collection) {
                var collectionMembers = new CollectionMembers(extPropertyType);
                var isValueType = collectionMembers.ElementType.IsValueType;
                var argsColItemVariable = _il.Var.Field(Members.VisitArgsCollectionItemField);

                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorTryVisit, argsFieldVariable);
                var stateLocal = _il.DeclareLocal("state", typeof(ValueState));
                _il.Var.Set(stateLocal);

                _il.Snippets.AreEqual(stateLocal, (int)ValueState.NotFound);

                var endLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(endLabel);

                _il.Snippets.AreEqual(stateLocal, (int) ValueState.Found);

                var nullLabel = _il.DefineLabel();
                _il.TransferLongIfFalse(nullLabel);

                var collectionLocal = _il.DeclareLocal("collection", collectionMembers.VariableType);
                _il.Construct(collectionMembers.Constructor);
                _il.Cast(collectionMembers.VariableType);
                _il.Var.Set(collectionLocal);

                var loopConditionLabel = _il.DefineLabel();
                _il.TransferLong(loopConditionLabel);

                var loopBodyLabel = _il.DefineLabel();
                _il.MarkLabel(loopBodyLabel);

                if (collectionMembers.ElementTypeRef.IsValueOrNullableOfValue()) {
                    _il.Var.Load(collectionLocal);

                    var valueLocal = DeclareAndLoadLocal("cv", collectionMembers.ElementType);
                    
                    _il.CallVirt(collectionMembers.Add);

                    _il.MarkLabel(loopConditionLabel);

                    _il.Var.Load(_visitorVariable);
                    _il.LoadStaticField(Members.VisitArgsCollectionItemField);
                    _il.Var.LoadAddress(valueLocal);

                    _il.CallVirt(Members.VisitorTryVisitValue[collectionMembers.ElementType]);

                    // Begin if logic section
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

                    _il.TransferLongIfTrue(loopBodyLabel);
                    // End if logic section
                }
                else {
                    var valueLocal = _il.DeclareLocal("cv", collectionMembers.ElementType);
                    GenerateCreateAndChildCallCode(valueLocal);
                    _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, argsColItemVariable);
                    _il.Snippets.InvokeMethod(collectionLocal, collectionMembers.Add, valueLocal);

                    _il.MarkLabel(loopConditionLabel);

                    var callTryVisit = new CallMethodILCode(_visitorVariable, Members.VisitorTryVisit, argsColItemVariable);
                    _il.Snippets.AreEqual(callTryVisit, (int)ValueState.Found);

                    _il.TransferLongIfTrue(loopBodyLabel);
                }

                _il.Snippets.SetPropertyValue(graphVariable, target.Ref, collectionLocal);
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, argsFieldVariable);
                _il.TransferLong(endLabel);

                _il.MarkLabel(nullLabel);
                _il.Snippets.SetPropertyValue(graphVariable, target.Ref, ILCodeParameter.Null);

                _il.MarkLabel(endLabel);
            }
            else {
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorTryVisit, argsFieldVariable);
                var stateLocal = _il.DeclareLocal("state", typeof(ValueState));
                _il.Var.Set(stateLocal);
                _il.Var.Load(stateLocal);
                _il.LoadValue((int) ValueState.NotFound);
                _il.CompareEquals();

                var endLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(endLabel);

                _il.Var.Load(stateLocal);
                _il.LoadValue((int) ValueState.Found);
                _il.CompareEquals();

                _il.Negate();
                var nullLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(nullLabel);

                var singleLocal = _il.DeclareLocal("single", extPropertyType.Inner);
                GenerateCreateAndChildCallCode(singleLocal);

                _il.Snippets.SetPropertyValue(graphVariable, target.Ref, singleLocal);
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, argsFieldVariable);

                _il.TransferLong(endLabel);

                _il.MarkLabel(nullLabel);
                _il.Snippets.SetPropertyValue(graphVariable, target.Ref, ILCodeParameter.Null);

                _il.MarkLabel(endLabel);
            }
        }

        private void GenerateEnumerateCode(ILCodeVariable local, FieldInfo visitArgsField, Action generateBodyMethod)
        {
            var conditionLabel = _il.DefineLabel();
            var bodyLabel = _il.DefineLabel();
            var type = local.VariableType;
            var extType = type.Extend();
            var visitArgsVariable = _il.Var.Field(visitArgsField);

            // First of all, transfer to the condition part
            _il.TransferLong(conditionLabel);

            // Mark that we enter the body of the loop
            _il.MarkLabel(bodyLabel);
            if (extType.Class == TypeClass.Complex) {
                GenerateCreateAndChildCallCode(local);
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorLeave, visitArgsVariable);
            }
            
            generateBodyMethod();

            _il.MarkLabel(conditionLabel);

            if (extType.IsValueOrNullableOfValue()) {
                var loadZeroLabel = _il.DefineLabel();
                var checkConditionLabel = _il.DefineLabel();

                _il.Var.Load(_visitorVariable);
                _il.Var.Load(visitArgsVariable);
                _il.Var.LoadAddress(local);

                _il.CallVirt(Members.VisitorTryVisitValue[type]);
                _il.TransferLongIfFalse(loadZeroLabel);

                if (type.IsValueType) {
                    _il.Var.LoadAddress(local);
                    _il.Call(Members.Nullable[type].GetHasValue);
                    _il.TransferLong(checkConditionLabel);
                }
                else {
                    _il.Var.Load(local);
                    _il.LoadNull();
                    _il.CompareEquals();
                    _il.Negate();
                    _il.TransferLong(checkConditionLabel);
                }

                _il.MarkLabel(loadZeroLabel);
                _il.LoadValue(0);
                _il.MarkLabel(checkConditionLabel);
                _il.TransferLongIfTrue(bodyLabel);

            }
            else {
                _il.Snippets.InvokeMethod(_visitorVariable, Members.VisitorTryVisit, visitArgsVariable);
                _il.LoadValue((int) ValueState.Found);
                _il.CompareEquals();
                _il.TransferLongIfTrue(bodyLabel);
            }

        }

        private ILCodeVariable DeclareAndLoadLocal(string name, Type type)
        {
            var isValueType = type.IsValueType;
            var valueLocal = _il.DeclareLocal(name, isValueType ? Members.Nullable[type].NullableType : type);

            GenerateLoadLocalValueCode(valueLocal);

            return valueLocal;
        }

        private void GenerateLoadLocalValueCode(ILCodeVariable local)
        {
            var type = local.VariableType;
            var extType = type.Extend();
            if (extType.Class == TypeClass.Nullable)
                type = extType.Container.AsNullable().ElementType;

            var isValueType = type.IsValueType;

            if (isValueType) {
                _il.Var.LoadAddress(local);
                _il.Call(Members.Nullable[type].GetValue);
            }
            else
                _il.Var.Load(local);
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
