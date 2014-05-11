using System;
using System.Collections.Generic;
using System.Linq;
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
            if (extPropertyType.IsValueOrNullableOfValue()) {
                var isNullable = extPropertyType.Class == TypeClass.Nullable;
                var isValueType = extPropertyType.Inner.IsValueType;
                _il.LoadArgs(1);
                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);
                _il.Call(isNullable
                    ? Members.VisitArgsNullableValue
                    : Members.VisitArgsValue);

                var valueType = isValueType ? Members.Nullable[extPropertyType.Inner].NullableType : extPropertyType.Inner;
                var valueLocal = _il.DeclareLocal("value", valueType);
                _il.LoadLocalAddress(valueLocal);
                _il.CallVirt(Members.VisitorTryVisitValue[extPropertyType.Inner]);

                var compareLocal = _il.DeclareLocal("compare", typeof(bool));

                if (isValueType) {
                    var labelValueNotFound = _il.DefineLabel();
                    _il.TransferIfFalse(labelValueNotFound);

                    _il.LoadLocalAddress(valueLocal);
                    _il.Call(Members.Nullable[context.Property.PropertyType].GetHasValue);
                    _il.LoadVal(0);
                    _il.CompareEquals();

                    var nullableHasValueLabel = _il.DefineLabel();
                    _il.Transfer(nullableHasValueLabel);

                    _il.MarkLabel(labelValueNotFound);
                    _il.LoadVal(1);
                    _il.MarkLabel(nullableHasValueLabel);
                }
                else {
                    _il.LoadVal(0);
                    _il.CompareEquals();
                }

                _il.SetLocal(compareLocal);
                _il.LoadLocal(compareLocal);

                var skipSetValueLabel = _il.DefineLabel();
                _il.TransferIfTrue(skipSetValueLabel);

                _il.LoadVar(graphVariable);
                if (isValueType)
                    _il.LoadLocalAddress(valueLocal);
                else
                    _il.LoadLocal(valueLocal);

                if (isValueType)
                    _il.Call(Members.Nullable[context.Property.PropertyType].GetValue);

                _il.CallVirt(context.Property.GetSetMethod());

                _il.MarkLabel(skipSetValueLabel);
            }
            else if (extPropertyType.Class == TypeClass.Collection) {
                var collectionMembers = new CollectionMembers(extPropertyType);
                var isValueType = collectionMembers.ElementType.IsValueType;

                _il.LoadArgs(1);
                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);
                _il.Call(Members.VisitArgsCollection);
                _il.CallVirt(Members.VisitorTryVisit);
                _il.LoadVal(0);
                _il.CompareEquals();

                var compareLocal = _il.DeclareLocal("compare", typeof(bool));
                _il.SetLocal(compareLocal);
                _il.LoadLocal(compareLocal);

                var hasCollectionLabel = _il.DefineLabel();
                _il.TransferIfTrue(hasCollectionLabel);

                var collectionLocal = _il.DeclareLocal("collection", collectionMembers.VariableType);
                _il.Construct(collectionMembers.Constructor);
                _il.SetLocal(collectionLocal);

                var loopLabel = _il.DefineLabel();
                _il.Transfer(loopLabel);

                var addValueLabel = _il.DefineLabel();
                _il.MarkLabel(addValueLabel);
                _il.LoadLocal(collectionLocal);

                var valueLocal = _il.DeclareLocal("cv", isValueType ? Members.Nullable[collectionMembers.ElementType].NullableType : collectionMembers.ElementType);

                if (isValueType) {
                    _il.LoadLocalAddress(valueLocal);
                    _il.Call(Members.Nullable[collectionMembers.ElementType].GetValue);
                }
                else
                    _il.LoadLocal(valueLocal);

                _il.CallVirt(collectionMembers.Add);

                _il.MarkLabel(loopLabel);

                _il.LoadArgs(1);
                _il.LoadStaticField(Members.VisitArgsItemField);
                _il.LoadLocalAddress(valueLocal);

                _il.CallVirt(Members.VisitorTryVisitValue[collectionMembers.ElementType]);

                // Begin if logic section
                var valueNotFoundLabel = _il.DefineLabel();
                _il.TransferIfFalse(valueNotFoundLabel);

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
                _il.Transfer(isNullLabel);

                _il.MarkLabel(valueNotFoundLabel);
                _il.LoadVal(0);
                _il.MarkLabel(isNullLabel);
                _il.SetLocal(compareLocal);
                _il.LoadLocal(compareLocal);

                _il.TransferIfTrue(addValueLabel);
                // End if logic section

                _il.LoadVar(graphVariable);
                _il.LoadLocal(collectionLocal);
                _il.CallVirt(context.Property.GetSetMethod());
                
                _il.LoadArgs(1);
                _il.CallVirt(Members.VisitorLeave);

                _il.MarkLabel(hasCollectionLabel);
            }
            else {
                _il.LoadArgs(1);
                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);
                _il.Call(Members.VisitArgsSingle);
                _il.CallVirt(Members.VisitorTryVisit);
                _il.LoadVal(0);
                _il.CompareEquals();

                var compareLocal = _il.DeclareLocal("compare", typeof(bool));
                _il.SetLocal(compareLocal);
                _il.LoadLocal(compareLocal);

                var hasSingleLabel = _il.DefineLabel();
                _il.TransferIfTrue(hasSingleLabel);

                var constructor = extPropertyType.Inner.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                    throw InvalidGraphException.NoParameterLessConstructor(extPropertyType.Inner);

                _il.Construct(constructor);
                var singleLocal = _il.DeclareLocal("single", extPropertyType.Inner);
                _il.SetLocal(singleLocal);

                GenerateChildCall(singleLocal);

                _il.LoadVar(graphVariable);
                _il.LoadLocal(singleLocal);
                _il.CallVirt(context.Property.GetSetMethod());

                _il.LoadArgs(1);
                _il.CallVirt(Members.VisitorLeave);

                _il.MarkLabel(hasSingleLabel);
            }
        }

        private void GenerateChildCall(LocalBuilder local)
        {
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
