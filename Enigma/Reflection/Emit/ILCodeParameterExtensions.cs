using System;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public static class ILCodeParameterExtensions
    {

        public static ILCodeParameter AsNullable(this ILCodeParameter parameter, Type type)
        {
            return new NullableILCodeParameter(parameter);
        }

        public static ILCodeParameter AsNullable(this ILCodeVariable variable)
        {
            return new NullableILCodeParameter(ILCodeParameter.Of(variable));
        }

        public static ILCodeParameter Cast(this ILCodeParameter parameter, Type toType)
        {
            return new CastILCodeParameter(parameter, toType);
        }

        public static ILCodeParameter Cast(this ILCodeVariable variable, Type toType)
        {
            return new CastILCodeParameter(ILCodeParameter.Of(variable), toType);
        }

        public static ILCodeParameter Call(this ILCodeParameter instance, MethodInfo method, params ILCodeParameter[] parameters)
        {
            return new CallMethodILCode(instance, method, parameters);
        }

        public static ILCodeParameter Call(this ILCodeVariable instance, MethodInfo method, params ILCodeParameter[] parameters)
        {
            return new CallMethodILCode(instance, method, parameters);
        }

    }
}
