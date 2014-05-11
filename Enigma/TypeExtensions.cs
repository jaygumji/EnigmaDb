using System;
using Enigma.Reflection;

namespace Enigma
{
    public static class TypeExtensions
    {

        public static ExtendedType Extend(this Type type)
        {
            return new ExtendedType(type);
        }

    }
}
