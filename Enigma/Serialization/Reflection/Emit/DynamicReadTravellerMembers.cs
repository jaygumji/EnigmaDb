using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Enigma.Reflection;

namespace Enigma.Serialization.Reflection.Emit
{
    public sealed class DynamicReadTravellerMembers
    {

        public readonly MethodInfo VisitArgsRoot;
        public readonly MethodInfo VisitArgsValue;
        public readonly MethodInfo VisitArgsNullableValue;
        public readonly MethodInfo VisitArgsSingle;
        public readonly MethodInfo VisitArgsCollection;
        public readonly FieldInfo VisitArgsItemField;

        public readonly MethodInfo VisitorTryVisit;
        public readonly MethodInfo VisitorLeave;
        public readonly Dictionary<Type, MethodInfo> VisitorTryVisitValue;

        public readonly Dictionary<Type, NullableMembers> Nullable;

        public readonly MethodInfo EnumeratorMoveNext;
        public readonly MethodInfo DisposableDispose;

        public DynamicReadTravellerMembers()
        {
            var visitArgsType = typeof(ReadVisitArgs);
            VisitArgsRoot = visitArgsType.GetMethod("Root");
            VisitArgsValue = visitArgsType.GetMethod("Value");
            VisitArgsNullableValue = visitArgsType.GetMethod("NullableValue");
            VisitArgsSingle = visitArgsType.GetMethod("Single");
            VisitArgsCollection = visitArgsType.GetMethod("Collection");
            VisitArgsItemField = visitArgsType.GetField("Item");

            var readVisitorType = typeof(IReadVisitor);
            VisitorTryVisit = readVisitorType.GetMethod("TryVisit");
            VisitorLeave = readVisitorType.GetMethod("Leave");

            VisitorTryVisitValue = new Dictionary<Type, MethodInfo>();
            Nullable = new Dictionary<Type, NullableMembers>();

            foreach (var method in readVisitorType.GetMethods()
                .Where(m => m.Name == "TryVisitValue")) {

                var valueType = method.GetParameters()[1].ParameterType;
                if (valueType.IsByRef) valueType = valueType.GetElementType();
                var valueTypeExt = valueType.Extend();

                VisitorTryVisitValue.Add(valueType, method);
                if (valueTypeExt.Class == TypeClass.Nullable) {
                    var innerType = valueTypeExt.Container.AsNullable().ElementType;
                    VisitorTryVisitValue.Add(innerType, method);
                    Nullable.Add(innerType, new NullableMembers(innerType));
                }
            }

            EnumeratorMoveNext = typeof(IEnumerator).GetMethod("MoveNext");
            DisposableDispose = typeof(IDisposable).GetMethod("Dispose");
        }

    }
}