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

            if (value == null) value = ILCodeParameter.Null;
            var setMethod = property.GetSetMethod();
            InvokeMethod(instance, setMethod, value);
        }

        public void InvokeMethod(ILCodeParameter instance, MethodInfo method, params ILCodeParameter[] parameters)
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
            if (type.IsEnum) {
                type = _il.TypeCache.Extend(type).GetUnderlyingEnumType();
            }

            var container = _il.TypeCache.Extend(type.AsNullable())
                .Container.AsNullable();

            _il.Construct(container.Constructor);
        }

        public void AreEqual(ILCodeParameter left, ILCodeParameter right)
        {
            if (left == null) left = ILCodeParameter.Null;
            if (right == null) right = ILCodeParameter.Null;

            ((IILCodeParameter) left).Load(_il);
            ((IILCodeParameter) right).Load(_il);
            _il.CompareEquals();
        }

        public void SetVariable(ILCodeVariable variable, ILCodeParameter valueToSet)
        {
            if (valueToSet == null) valueToSet = ILCodeParameter.Null;

            ((IILCodeParameter) valueToSet).Load(_il);
            _il.Var.Set(variable);
        }

        public void Throw(ILCodeParameter exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            ((IILCodeParameter) exception).Load(_il);
            _il.Throw();
        }
    }
}