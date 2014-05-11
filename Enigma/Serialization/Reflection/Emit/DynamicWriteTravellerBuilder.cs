using System;
using System.Collections.Generic;
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
            if (extPropertyType.IsValueOrNullableOfValue()) {
                var isNullable = extPropertyType.Class == TypeClass.Nullable;
                _il.LoadArgs(1);
                _il.LoadVar(graphVariable);
                _il.CallVirt(context.Property.GetGetMethod());
                if (!isNullable && context.Property.PropertyType.IsValueType)
                    _il.Construct(Members.NullableConstructors[context.Property.PropertyType]);
                _il.LoadVal(context.Property.Name);
                _il.LoadVal(context.Index);
                _il.Call(isNullable
                    ? Members.VisitArgsNullableValue
                    : Members.VisitArgsValue);

                var visitValueMethod = Members.VisitorVisitValue[context.Property.PropertyType];
                _il.CallVirt(visitValueMethod);
            }
            else if (extPropertyType.Class == TypeClass.Collection) {
                var elementType = extPropertyType.Container.AsCollection().ElementType;
                _il.LoadVar(graphVariable);
                _il.CallVirt(context.Property.GetGetMethod());
                var cLocal = _il.DeclareLocal("collection", context.Property.PropertyType);
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
                _il.TransferIfTrue(checkIfNullLabel);

                GenerateChildCall(singleLocal);

                _il.MarkLabel(checkIfNullLabel);

                _il.LoadArgs(1);
                _il.CallVirt(Members.VisitorLeave);
            }
        }

        private void GenerateCollectionCode(IVariable collection, Type elementType)
        {
            var checkIfNullLabel = _il.DefineLabel();
            var checkLocal = _il.DeclareLocal("check", typeof(bool));
            _il.LoadVar(collection);
            _il.LoadNull();
            _il.CompareEquals();
            _il.SetLocal(checkLocal);
            _il.LoadLocal(checkLocal);
            _il.TransferIfTrue(checkIfNullLabel);

            _il.LoadVar(collection);

            var getEnumeratorMethod = typeof(IEnumerable<>).MakeGenericType(elementType).GetMethod("GetEnumerator");
            _il.CallVirt(getEnumeratorMethod);

            var itLocal = _il.DeclareLocal("it", typeof(IEnumerable<>).MakeGenericType(elementType));
            _il.SetLocal(itLocal);

            _il.Try();

            var itHeadLabel = _il.DefineLabel();
            var itBodyLabel = _il.DefineLabel();
            _il.Transfer(itHeadLabel);
            var itVarLocal = _il.DeclareLocal("cv", elementType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);
            var getCurrentMethod = enumeratorType.GetProperty("Current").GetGetMethod();
            _il.MarkLabel(itBodyLabel);
            _il.LoadLocal(itLocal);
            _il.Call(getCurrentMethod);
            _il.SetLocal(itVarLocal);

            var elementContext = TypeReflectionContext.Get(elementType);
            var extElementType = elementContext.Extended;
            if (extElementType.IsValueOrNullableOfValue()) {
                var isNullable = extElementType.Class == TypeClass.Nullable;
                _il.LoadArgs(1);
                _il.LoadLocal(itVarLocal);
                if (!isNullable && elementType.IsValueType)
                    _il.Construct(Members.NullableConstructors[elementType]);
                _il.LoadStaticField(Members.VisitArgsItemField);
                var visitValueMethod = Members.VisitorVisitValue[elementType];
                _il.CallVirt(visitValueMethod);
            }
            else if (extElementType.ImplementsCollection) {
            }
            else {
                _il.LoadArgs(1);
                _il.LoadStaticField(Members.VisitArgsItemField);
                _il.CallVirt(Members.VisitorVisit);

                GenerateChildCall(itVarLocal);

                _il.LoadArgs(1);
                _il.CallVirt(Members.VisitorLeave);
            }

            _il.MarkLabel(itHeadLabel);
            _il.LoadLocal(itLocal);
            _il.CallVirt(Members.EnumeratorMoveNext);

            _il.SetLocal(checkLocal);
            _il.LoadLocal(checkLocal);
            _il.TransferIfTrue(itBodyLabel);

            _il.Finally();
            _il.LoadLocal(itLocal);
            _il.LoadNull();
            _il.CompareEquals();
            _il.SetLocal(checkLocal);
            _il.LoadLocal(checkLocal);

            var endLabel = _il.DefineLabel();
            _il.TransferIfTrue(endLabel);

            _il.LoadLocal(itLocal);
            _il.CallVirt(Members.DisposableDispose);

            _il.EndTry();

            _il.MarkLabel(endLabel);
            _il.MarkLabel(checkIfNullLabel);
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
            _il.CallVirt(childTravellerInfo.TravelWriteMethod);
        }

        
    }
}