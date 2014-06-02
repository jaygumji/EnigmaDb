using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Enigma.Reflection;
using Enigma.Reflection.Emit;
using MethodBuilder = Enigma.Reflection.Emit.MethodBuilder;

namespace Enigma.Serialization.Reflection.Emit
{
    public class DynamicReadTravellerBuilder
    {

        private static readonly DynamicReadTravellerMembers Members = new DynamicReadTravellerMembers();

        private readonly TypeReflectionContext _context;
        private readonly Dictionary<Type, ChildTravellerInfo> _childTravellers;
        private readonly ILExpressed _il;

        public DynamicReadTravellerBuilder(MethodBuilder builder, TypeReflectionContext context, Dictionary<Type, ChildTravellerInfo> childTravellers)
        {
            _context = context;
            _childTravellers = childTravellers;
            _il = builder.IL;
        }

        public void BuildTravelReadMethod()
        {
            var graphArgument = new MethodArgVariable(2, _context.Type);
            foreach (var property in _context.SerializableProperties) {
                GeneratePropertyCode(graphArgument, property);
            }
            _il.Return();
        }

        private void GeneratePropertyCode(MethodArgVariable graphVariable, PropertyReflectionContext context)
        {
            var extPropertyType = context.PropertyTypeContext.Extended;
            //if (extPropertyType.Class == TypeClass.Dictionary) return;
            //if (extPropertyType.Class == TypeClass.Complex) return;
            //if (extPropertyType.Class == TypeClass.Collection) return;
            //if (extPropertyType.Class == TypeClass.Value) return;
            if (extPropertyType.IsValueOrNullableOfValue()) {
                var isNullable = extPropertyType.Class == TypeClass.Nullable;
                var isEnum = extPropertyType.IsEnum();
                var isValueType = extPropertyType.Inner.IsValueType;
                _il.LoadArgs(1);
                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);
                _il.Call(isNullable
                    ? Members.VisitArgsNullableValue
                    : Members.VisitArgsValue);


                Type mediatorPropertyType;
                Type valueType;
                if (isEnum) {
                    mediatorPropertyType = extPropertyType.GetUnderlyingEnumType();
                    valueType = Members.Nullable[mediatorPropertyType].NullableType;
                }
                else {
                    mediatorPropertyType = extPropertyType.Inner;
                    valueType = isValueType ? Members.Nullable[mediatorPropertyType].NullableType : mediatorPropertyType;
                }

                var valueLocal = _il.DeclareLocal("value", valueType);
                _il.LoadLocalAddress(valueLocal);
                _il.CallVirt(Members.VisitorTryVisitValue[valueType]);

                if (isValueType) {
                    var labelValueNotFound = _il.DefineLabel();
                    _il.TransferShortIfFalse(labelValueNotFound);

                    _il.LoadLocalAddress(valueLocal);
                    _il.Call(Members.Nullable[mediatorPropertyType].GetHasValue);
                    _il.LoadVal(0);
                    _il.CompareEquals();

                    var nullableHasValueLabel = _il.DefineLabel();
                    _il.TransferShort(nullableHasValueLabel);

                    _il.MarkLabel(labelValueNotFound);
                    _il.LoadVal(1);
                    _il.MarkLabel(nullableHasValueLabel);
                }
                else {
                    _il.LoadVal(0);
                    _il.CompareEquals();
                }

                var skipSetValueLabel = _il.DefineLabel();
                _il.TransferShortIfTrue(skipSetValueLabel);

                _il.LoadVar(graphVariable);
                if (isValueType)
                    _il.LoadLocalAddress(valueLocal);
                else
                    _il.LoadLocal(valueLocal);

                if (!isNullable && (isValueType || isEnum))
                    _il.Call(Members.Nullable[mediatorPropertyType].GetValue);

                _il.CallVirt(context.Property.GetSetMethod());

                _il.MarkLabel(skipSetValueLabel);
            }
            else if (extPropertyType.Class == TypeClass.Dictionary) {
                var dictionaryMembers = new DictionaryMembers(extPropertyType);
 
                _il.LoadArgs(1);
                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);
                _il.Call(Members.VisitArgsDictionary);
                _il.CallVirt(Members.VisitorTryVisit);
                var stateLocal = _il.DeclareLocal("state", typeof(ValueState));
                _il.SetLocal(stateLocal);
                _il.LoadLocal(stateLocal);
                _il.LoadVal((int)ValueState.NotFound);
                _il.CompareEquals();

                var endLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(endLabel);

                _il.LoadLocal(stateLocal);
                _il.LoadVal((int)ValueState.Found);
                _il.CompareEquals();

                // Invert the result
                _il.LoadVal(0);
                _il.CompareEquals();
                var nullLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(nullLabel);

                var dictionaryLocal = _il.DeclareLocal("dictionary", dictionaryMembers.VariableType);
                _il.Construct(dictionaryMembers.Constructor);
                _il.SetLocal(dictionaryLocal);

                NullableMembers keyNullableMembers;
                var keyLocal = _il.DeclareLocal("ck", Members.Nullable.TryGetValue(dictionaryMembers.KeyType, out keyNullableMembers) ? keyNullableMembers.NullableType : dictionaryMembers.KeyType);
                GenerateEnumerateCode(keyLocal, Members.VisitArgsDictionaryKey, () => {
                    var throwExceptionLabel = _il.DefineLabel();
                    var endBodyLabel = _il.DefineLabel();

                    NullableMembers valueNullableMembers;
                    var hasNullableMembers = Members.Nullable.TryGetValue(dictionaryMembers.ValueType, out valueNullableMembers);
                    var valueLocal = _il.DeclareLocal("cv", hasNullableMembers ? valueNullableMembers.NullableType : dictionaryMembers.ValueType);
                    var valueType = dictionaryMembers.ValueType;
                    _il.LoadArgs(1);
                    _il.LoadStaticField(Members.VisitArgsDictionaryValue);
                    var extValueType = valueType.Extend();

                    if (extValueType.IsValueOrNullableOfValue()) {
                        var loadOneLabel = _il.DefineLabel();
                        var checkConditionLabel = _il.DefineLabel();

                        _il.LoadLocalAddress(valueLocal);
                        _il.CallVirt(Members.VisitorTryVisitValue[dictionaryMembers.ValueType]);
                        _il.TransferShortIfFalse(loadOneLabel);

                        if (hasNullableMembers) {
                            _il.LoadLocalAddress(valueLocal);
                            _il.Call(valueNullableMembers.GetHasValue);
                        }
                        else {
                            _il.LoadLocal(valueLocal);
                            _il.LoadNull();
                            _il.CompareEquals();
                        }
                        _il.LoadVal(0); // Invert the above expression
                        _il.CompareEquals();
                        _il.TransferShort(checkConditionLabel);
                        _il.MarkLabel(loadOneLabel);
                        _il.LoadVal(1);
                        _il.MarkLabel(checkConditionLabel);
                        _il.TransferShortIfTrue(throwExceptionLabel);
                    }
                    else {
                        _il.CallVirt(Members.VisitorTryVisit);
                        _il.LoadVal((int) ValueState.Found);
                        _il.CompareEquals();
                        _il.LoadVal(0);
                        _il.CompareEquals();
                        _il.TransferShortIfTrue(throwExceptionLabel);

                        GenerateCreateAndChildCallCode(valueLocal);
                        _il.LoadArgs(1);
                        _il.CallVirt(Members.VisitorLeave);
                    }

                    _il.LoadLocal(dictionaryLocal);
                    GenerateLoadLocalValueCode(keyLocal);
                    GenerateLoadLocalValueCode(valueLocal);

                    _il.CallVirt(dictionaryMembers.Add);
                    _il.TransferShort(endBodyLabel);

                    _il.MarkLabel(throwExceptionLabel);
                    _il.LoadVal(context.Property.Name);
                    _il.Call(Members.ExceptionNoDictionaryValue);
                    _il.Throw();

                    _il.MarkLabel(endBodyLabel);
                });

                _il.LoadVar(graphVariable);
                _il.LoadLocal(dictionaryLocal);
                if (dictionaryLocal.LocalType != context.Property.PropertyType)
                    _il.Cast(context.Property.PropertyType);
                _il.CallVirt(context.Property.GetSetMethod());

                _il.LoadArgs(1);
                _il.CallVirt(Members.VisitorLeave);

                _il.TransferShort(endLabel);

                _il.MarkLabel(nullLabel);
                _il.LoadVar(graphVariable);
                _il.LoadNull();
                _il.CallVirt(context.Property.GetSetMethod());

                _il.MarkLabel(endLabel);
            }
            else if (extPropertyType.Class == TypeClass.Collection) {
                var collectionMembers = new CollectionMembers(extPropertyType);
                var isValueType = collectionMembers.ElementType.IsValueType;

                _il.LoadArgs(1);
                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);
                _il.Call(Members.VisitArgsCollection);
                _il.CallVirt(Members.VisitorTryVisit);
                var stateLocal = _il.DeclareLocal("state", typeof(ValueState));
                _il.SetLocal(stateLocal);
                _il.LoadLocal(stateLocal);
                _il.LoadVal((int)ValueState.NotFound);
                _il.CompareEquals();

                var compareLocal = _il.DeclareLocal("compare", typeof(bool));
                _il.SetLocal(compareLocal);
                _il.LoadLocal(compareLocal);

                var endLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(endLabel);

                _il.LoadLocal(stateLocal);
                _il.LoadVal((int)ValueState.Found);
                _il.CompareEquals();

                // Invert the result
                _il.LoadVal(0);
                _il.CompareEquals();
                var nullLabel = _il.DefineLabel();
                _il.TransferLongIfTrue(nullLabel);

                var collectionLocal = _il.DeclareLocal("collection", collectionMembers.VariableType);
                _il.Construct(collectionMembers.Constructor);
                _il.Cast(collectionMembers.VariableType);
                _il.SetLocal(collectionLocal);

                var loopLabel = _il.DefineLabel();
                _il.TransferShort(loopLabel);

                var addValueLabel = _il.DefineLabel();
                _il.MarkLabel(addValueLabel);
                _il.LoadLocal(collectionLocal);

                var valueLocal = DeclareAndLoadLocal("cv", collectionMembers.ElementType);
                _il.CallVirt(collectionMembers.Add);

                _il.MarkLabel(loopLabel);

                _il.LoadArgs(1);
                _il.LoadStaticField(Members.VisitArgsItemField);
                _il.LoadLocalAddress(valueLocal);

                _il.CallVirt(Members.VisitorTryVisitValue[collectionMembers.ElementType]);

                // Begin if logic section
                var valueNotFoundLabel = _il.DefineLabel();
                _il.TransferShortIfFalse(valueNotFoundLabel);

                if (isValueType) {
                    _il.LoadLocalAddress(valueLocal);
                    _il.Call(Members.Nullable[collectionMembers.ElementType].GetHasValue);
                }
                else {
                    _il.LoadLocal(valueLocal);
                    _il.LoadNull();
                    _il.CompareEquals();
                    _il.LoadVal(0);
                    _il.CompareEquals();
                }

                var isNullLabel = _il.DefineLabel();
                _il.TransferShort(isNullLabel);

                _il.MarkLabel(valueNotFoundLabel);
                _il.LoadVal(0);
                _il.MarkLabel(isNullLabel);

                _il.TransferShortIfTrue(addValueLabel);
                // End if logic section

                _il.LoadVar(graphVariable);
                _il.LoadLocal(collectionLocal);
                _il.CallVirt(context.Property.GetSetMethod());

                _il.LoadArgs(1);
                _il.CallVirt(Members.VisitorLeave);
                _il.TransferShort(endLabel);

                _il.MarkLabel(nullLabel);
                _il.LoadVar(graphVariable);
                _il.LoadNull();
                _il.CallVirt(context.Property.GetSetMethod());

                _il.MarkLabel(endLabel);
            }
            else {
                _il.LoadArgs(1);
                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);
                _il.Call(Members.VisitArgsSingle);
                _il.CallVirt(Members.VisitorTryVisit);
                var stateLocal = _il.DeclareLocal("state", typeof(ValueState));
                _il.SetLocal(stateLocal);
                _il.LoadLocal(stateLocal);
                _il.LoadVal((int) ValueState.NotFound);
                _il.CompareEquals();

                var compareLocal = _il.DeclareLocal("compare", typeof (bool));
                _il.SetLocal(compareLocal);
                _il.LoadLocal(compareLocal);

                var endLabel = _il.DefineLabel();
                _il.TransferShortIfTrue(endLabel);

                _il.LoadLocal(stateLocal);
                _il.LoadVal((int) ValueState.Found);
                _il.CompareEquals();

                // Invert the result
                _il.LoadVal(0);
                _il.CompareEquals();
                var nullLabel = _il.DefineLabel();
                _il.TransferShortIfTrue(nullLabel);

                var singleLocal = _il.DeclareLocal("single", extPropertyType.Inner);
                GenerateCreateAndChildCallCode(singleLocal);

                _il.LoadVar(graphVariable);
                _il.LoadLocal(singleLocal);
                _il.CallVirt(context.Property.GetSetMethod());

                _il.LoadArgs(1);
                _il.CallVirt(Members.VisitorLeave);

                _il.TransferShort(endLabel);

                _il.MarkLabel(nullLabel);
                _il.LoadVar(graphVariable);
                _il.LoadNull();
                _il.CallVirt(context.Property.GetSetMethod());

                _il.MarkLabel(endLabel);
            }
        }

        private void GenerateEnumerateCode(LocalBuilder local, FieldInfo visitArgsField, Action generateBodyMethod)
        {
            var conditionLabel = _il.DefineLabel();
            var bodyLabel = _il.DefineLabel();
            var type = local.LocalType;
            var extType = type.Extend();

            // First of all, transfer to the condition part
            _il.TransferLong(conditionLabel);

            // Mark that we enter the body of the loop
            _il.MarkLabel(bodyLabel);
            if (extType.Class == TypeClass.Complex) {
                GenerateCreateAndChildCallCode(local);
                _il.LoadArgs(1);
                _il.CallVirt(Members.VisitorLeave);
            }
            
            generateBodyMethod();

            _il.MarkLabel(conditionLabel);
            _il.LoadArgs(1);
            _il.LoadStaticField(visitArgsField);

            if (extType.IsValueOrNullableOfValue()) {
                var loadZeroLabel = _il.DefineLabel();
                var checkConditionLabel = _il.DefineLabel();

                _il.LoadLocalAddress(local);

                _il.CallVirt(Members.VisitorTryVisitValue[type]);
                _il.TransferShortIfFalse(loadZeroLabel);

                if (type.IsValueType) {
                    _il.LoadLocalAddress(local);
                    _il.Call(Members.Nullable[type].GetHasValue);
                    _il.TransferShort(checkConditionLabel);
                }
                else {
                    _il.LoadLocal(local);
                    _il.LoadNull();
                    _il.CompareEquals();
                    _il.LoadVal(0); // Invert the above expression
                    _il.CompareEquals();
                    _il.TransferShort(checkConditionLabel);
                }

                _il.MarkLabel(loadZeroLabel);
                _il.LoadVal(0);
                _il.MarkLabel(checkConditionLabel);
                _il.TransferLongIfTrue(bodyLabel);

            }
            else {
                _il.CallVirt(Members.VisitorTryVisit);
                _il.LoadVal((int) ValueState.Found);
                _il.CompareEquals();
                _il.TransferLongIfTrue(bodyLabel);
            }

        }

        private LocalBuilder DeclareAndLoadLocal(string name, Type type)
        {
            var isValueType = type.IsValueType;
            var valueLocal = _il.DeclareLocal(name, isValueType ? Members.Nullable[type].NullableType : type);

            GenerateLoadLocalValueCode(valueLocal);

            return valueLocal;
        }

        private void GenerateLoadLocalValueCode(LocalBuilder local)
        {
            var type = local.LocalType;
            var extType = type.Extend();
            if (extType.Class == TypeClass.Nullable)
                type = extType.Container.AsNullable().ElementType;

            var isValueType = type.IsValueType;

            if (isValueType) {
                _il.LoadLocalAddress(local);
                _il.Call(Members.Nullable[type].GetValue);
            }
            else
                _il.LoadLocal(local);
        }

        private void GenerateCreateAndChildCallCode(LocalBuilder local)
        {
            var type = local.LocalType;

            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw InvalidGraphException.NoParameterLessConstructor(type);

            _il.Construct(constructor);
            _il.SetLocal(local);

            ChildTravellerInfo childTravellerInfo;
            if (!_childTravellers.TryGetValue(local.LocalType, out childTravellerInfo))
                throw InvalidGraphException.ComplexTypeWithoutTravellerDefined(local.LocalType);

            _il.LoadThis();
            _il.LoadField(childTravellerInfo.Field);
            _il.LoadArgs(1);
            _il.LoadLocal(local);
            _il.CallVirt(childTravellerInfo.TravelReadMethod);
        }
    }
}
