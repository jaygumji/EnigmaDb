using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Enigma.Reflection;
using Enigma.Reflection.Emit;

namespace Enigma.Serialization.Reflection.Emit
{
    public sealed class DynamicWriteTravellerMembers
    {

        public readonly ILCodeParameter VisitArgsCollectionItem;
        public readonly ILCodeParameter VisitArgsDictionaryKey;
        public readonly ILCodeParameter VisitArgsDictionaryValue;

        public readonly MethodInfo VisitorVisit;
        public readonly MethodInfo VisitorLeave;
        public readonly Dictionary<Type, MethodInfo> VisitorVisitValue;

        public readonly Dictionary<Type, ConstructorInfo> NullableConstructors; 

        public readonly MethodInfo EnumeratorMoveNext;
        public readonly MethodInfo DisposableDispose;

        public DynamicWriteTravellerMembers()
        {
            var visitArgsType = typeof (VisitArgs);
            VisitArgsCollectionItem = new StaticFieldILCodeVariable(visitArgsType.GetField("CollectionItem"));
            VisitArgsDictionaryKey = new StaticFieldILCodeVariable(visitArgsType.GetField("DictionaryKey"));
            VisitArgsDictionaryValue = new StaticFieldILCodeVariable(visitArgsType.GetField("DictionaryValue"));

            var writeVisitorType = typeof (IWriteVisitor);
            VisitorVisit = writeVisitorType.GetMethod("Visit");
            VisitorLeave = writeVisitorType.GetMethod("Leave");

            VisitorVisitValue = new Dictionary<Type, MethodInfo>();
            NullableConstructors = new Dictionary<Type, ConstructorInfo>();
            var nullableType = typeof (Nullable<>);
            foreach (var method in writeVisitorType.GetMethods()
                .Where(m => m.Name == "VisitValue")) {

                var valueType = method.GetParameters()[0].ParameterType;
                var valueTypeExt = valueType.Extend();

                VisitorVisitValue.Add(valueType, method);
                if (valueTypeExt.Class == TypeClass.Nullable) {
                    var innerType = valueTypeExt.Container.AsNullable().ElementType;
                    VisitorVisitValue.Add(innerType, method);
                    NullableConstructors.Add(innerType, nullableType.MakeGenericType(innerType).GetConstructor(new []{innerType}));
                }
            }

            EnumeratorMoveNext = typeof (IEnumerator).GetMethod("MoveNext");
            DisposableDispose = typeof (IDisposable).GetMethod("Dispose");
        }

    }
}