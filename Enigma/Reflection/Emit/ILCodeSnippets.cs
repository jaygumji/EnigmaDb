using System;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class ILCodeSnippets
    {
        private readonly ILExpressed _il;

        public ILCodeSnippets(ILExpressed il)
        {
            _il = il;
        }

        public void GetField(ILCodeVariable instance, FieldInfo field)
        {
            _il.Var.Load(instance);
            _il.LoadField(field);
        }

        public void GetPropertyValue(ILCodeVariable instance, PropertyInfo property)
        {
            if (!property.CanRead) throw new InvalidOperationException("Can not get value from a property with no getter");
            var getMethod = property.GetGetMethod();
            InvokeMethod(instance, getMethod);
        }

        public void SetPropertyValue(ILCodeVariable instance, PropertyInfo property, ILCodeParameter value)
        {
            if (!property.CanWrite) throw new InvalidOperationException("Can not set value from a property with no setter");

            var setMethod = property.GetSetMethod();
            InvokeMethod(instance, setMethod, value);
        }

        public void InvokeMethod(ILCodeVariable instance, MethodInfo method, params ILCodeParameter[] parameters)
        {
            _il.Generate(new CallMethodILCode(instance, method, parameters));
        }

        public void InvokeMethod(MethodInfo method, params ILCodeParameter[] parameters)
        {
            _il.Generate(new CallMethodILCode(method, parameters));
        }

        public void ConstructInstance(ConstructorInfo constructor, params ILCodeParameter[] parameters)
        {
            _il.Generate(new CallConstructorILCode(constructor, parameters));
        }

        public void WhileLoop(ILGenerationHandler conditionHandler, ILGenerationHandler bodyHandler)
        {
            _il.Generate(new WhileLoopILCode(conditionHandler,  bodyHandler));
        }

        public void AsNullable(Type type)
        {
            var container = _il.TypeCache.Extend(type.AsNullable())
                .Container.AsNullable();

            _il.Construct(container.Constructor);
        }

        public void AreEqual(ILCodeParameter left, ILCodeParameter right)
        {
            _il.Generate(left);
            _il.Generate(right);
            _il.CompareEquals();
        }
    }
}