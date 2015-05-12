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

        public readonly FieldInfo VisitArgsItemField;
        public readonly FieldInfo VisitArgsDictionaryKey;
        public readonly FieldInfo VisitArgsDictionaryValue;

        public readonly MethodInfo VisitorTryVisit;
        public readonly MethodInfo VisitorLeave;
        public readonly Dictionary<Type, MethodInfo> VisitorTryVisitValue;

        public readonly Dictionary<Type, NullableMembers> Nullable;

        public readonly MethodInfo EnumeratorMoveNext;
        public readonly MethodInfo DisposableDispose;
        
        public readonly MethodInfo ExceptionNoDictionaryValue;

        public DynamicReadTravellerMembers()
        {
            var visitArgsType = typeof(VisitArgs);
            VisitArgsItemField = visitArgsType.GetField("CollectionItem");
            VisitArgsDictionaryKey = visitArgsType.GetField("DictionaryKey");
            VisitArgsDictionaryValue = visitArgsType.GetField("DictionaryValue");

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

                    var nullableMembers = new NullableMembers(innerType);
                    Nullable.Add(innerType, nullableMembers);
                    Nullable.Add(valueType, nullableMembers);
                }
            }

            EnumeratorMoveNext = typeof(IEnumerator).GetMethod("MoveNext");
            DisposableDispose = typeof(IDisposable).GetMethod("Dispose");

            ExceptionNoDictionaryValue = typeof (InvalidGraphException).GetMethod("NoDictionaryValue");
        }

    }
}