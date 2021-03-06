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
        public readonly ILCodeParameter VisitArgsCollectionInCollection;
        public readonly ILCodeParameter VisitArgsDictionaryInCollection;
        public readonly ILCodeParameter VisitArgsDictionaryInDictionaryKey;
        public readonly ILCodeParameter VisitArgsDictionaryInDictionaryValue;
        public readonly ILCodeParameter VisitArgsCollectionInDictionaryKey;
        public readonly ILCodeParameter VisitArgsCollectionInDictionaryValue;

        public readonly MethodInfo VisitorVisit;
        public readonly MethodInfo VisitorLeave;
        public readonly Dictionary<Type, MethodInfo> VisitorVisitValue;

        public readonly Dictionary<Type, ConstructorInfo> NullableConstructors; 

        public readonly MethodInfo EnumeratorMoveNext;
        public readonly MethodInfo DisposableDispose;

        public readonly MethodInfo ArrayGetLength;

        public DynamicWriteTravellerMembers()
        {
            var visitArgsType = typeof (VisitArgs);
            VisitArgsCollectionItem = new StaticFieldILCodeVariable(visitArgsType.GetField("CollectionItem"));
            VisitArgsDictionaryKey = new StaticFieldILCodeVariable(visitArgsType.GetField("DictionaryKey"));
            VisitArgsDictionaryValue = new StaticFieldILCodeVariable(visitArgsType.GetField("DictionaryValue"));
            VisitArgsCollectionInCollection = new StaticFieldILCodeVariable(visitArgsType.GetField("CollectionInCollection"));
            VisitArgsDictionaryInCollection = new StaticFieldILCodeVariable(visitArgsType.GetField("DictionaryInCollection"));
            VisitArgsDictionaryInDictionaryKey = new StaticFieldILCodeVariable(visitArgsType.GetField("DictionaryInDictionaryKey"));
            VisitArgsDictionaryInDictionaryValue = new StaticFieldILCodeVariable(visitArgsType.GetField("DictionaryInDictionaryValue"));
            VisitArgsCollectionInDictionaryKey = new StaticFieldILCodeVariable(visitArgsType.GetField("CollectionInDictionaryKey"));
            VisitArgsCollectionInDictionaryValue = new StaticFieldILCodeVariable(visitArgsType.GetField("CollectionInDictionaryValue"));

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
            ArrayGetLength = typeof (Array).GetMethod("GetLength");
        }

    }
}