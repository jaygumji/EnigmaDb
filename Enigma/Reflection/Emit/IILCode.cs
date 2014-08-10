using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public interface IILCode
    {

        void Generate(ILExpressed il);

    }

    public class ILEnumerateCode : IILCode
    {
        private readonly IVariable _enumerable;
        private readonly Action<ILExpressed, IVariable> _iterateBody;

        private static readonly MethodInfo EnumeratorMoveNext;
        private static readonly MethodInfo DisposableDispose;

        static ILEnumerateCode()
        {
            EnumeratorMoveNext = typeof(IEnumerator).GetMethod("MoveNext");
            DisposableDispose = typeof(IDisposable).GetMethod("Dispose");
        }

        public ILEnumerateCode(IVariable enumerable, Action<ILExpressed, IVariable> iterateBody)
        {
            _enumerable = enumerable;
            _iterateBody = iterateBody;
        }

        void IILCode.Generate(ILExpressed il)
        {
            var variableType = il.TypeCache.Extend(_enumerable.VariableType);
            var collectionContainer = variableType.Container.AsCollection();

            if (collectionContainer == null)
                throw new InvalidOperationException("Could not enumerate the type " + variableType.Inner.FullName);

            var elementType = collectionContainer.ElementType;
            il.LoadVar(_enumerable);

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var getEnumeratorMethod = enumerableType.GetMethod("GetEnumerator");
            il.CallVirt(getEnumeratorMethod);

            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);
            var itLocal = il.DeclareLocal("it", enumeratorType);
            il.SetLocal(itLocal);

            il.Try();

            var itHeadLabel = il.DefineLabel();
            var itBodyLabel = il.DefineLabel();
            il.TransferShort(itHeadLabel);
            var itVarLocal = il.DeclareLocal("cv", elementType);
            var getCurrentMethod = enumeratorType.GetProperty("Current").GetGetMethod();
            il.MarkLabel(itBodyLabel);
            il.LoadLocal(itLocal);
            il.Call(getCurrentMethod);
            il.SetLocal(itVarLocal);

            _iterateBody.Invoke(il, new LocalVariable(itVarLocal));

            il.MarkLabel(itHeadLabel);
            il.LoadLocal(itLocal);
            il.CallVirt(EnumeratorMoveNext);

            il.TransferShortIfTrue(itBodyLabel);

            il.Finally();
            il.LoadLocal(itLocal);
            il.LoadNull();
            il.CompareEquals();

            var endLabel = il.DefineLabel();
            il.TransferShortIfTrue(endLabel);

            il.LoadLocal(itLocal);
            il.CallVirt(DisposableDispose);

            il.MarkLabel(endLabel);
            il.EndTry();
        }

    }
}
