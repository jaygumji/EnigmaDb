using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class ILEnumerateCode : IILCode
    {
        private readonly ILCodeVariable _enumerable;
        private readonly Action<ILExpressed, ILCodeVariable> _iterateBody;

        private static readonly MethodInfo EnumeratorMoveNext;
        private static readonly MethodInfo DisposableDispose;

        static ILEnumerateCode()
        {
            EnumeratorMoveNext = typeof(IEnumerator).GetMethod("MoveNext");
            DisposableDispose = typeof(IDisposable).GetMethod("Dispose");
        }

        public ILEnumerateCode(ILCodeVariable enumerable, Action<ILExpressed, ILCodeVariable> iterateBody)
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
            il.Var.Load(_enumerable);

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var getEnumeratorMethod = enumerableType.GetMethod("GetEnumerator");
            il.CallVirt(getEnumeratorMethod);

            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);
            var itLocal = il.DeclareLocal("it", enumeratorType);
            il.Var.Set(itLocal);

            il.Try();

            var itHeadLabel = il.DefineLabel();
            var itBodyLabel = il.DefineLabel();
            il.TransferLong(itHeadLabel);
            var itVarLocal = il.DeclareLocal("cv", elementType);
            var getCurrentMethod = enumeratorType.GetProperty("Current").GetGetMethod();
            il.MarkLabel(itBodyLabel);
            il.Var.Load(itLocal);
            il.Call(getCurrentMethod);
            il.Var.Set(itVarLocal);

            _iterateBody.Invoke(il, itVarLocal);

            il.MarkLabel(itHeadLabel);
            il.Var.Load(itLocal);
            il.CallVirt(EnumeratorMoveNext);

            il.TransferLongIfTrue(itBodyLabel);

            il.Finally();
            il.Var.Load(itLocal);
            il.LoadNull();
            il.CompareEquals();

            var endLabel = il.DefineLabel();
            il.TransferLongIfTrue(endLabel);

            il.Var.Load(itLocal);
            il.CallVirt(DisposableDispose);

            il.MarkLabel(endLabel);
            il.EndTry();
        }

    }
}