using System;

namespace Enigma.Serialization
{
    public class InvalidGraphException : Exception
    {
        public InvalidGraphException(string message) : base(message)
        {
        }

        public static InvalidGraphException NoParameterLessConstructor(Type type)
        {
            return new InvalidGraphException(string.Format("Type {0} is used, but does not contain a parameterless constructor", type.FullName));
        }

        public static InvalidGraphException ComplexTypeWithoutTravellerDefined(Type type)
        {
            return new InvalidGraphException(string.Format("Use of complex type {0} detected, but no traveller has been defined", type.FullName));
        }

        public static InvalidGraphException MissingCollectionAddMethod(Type type)
        {
            return new InvalidGraphException(string.Format("Collection of type {0} is used in the graph, but the collection is missing an Add method that takes the element type as parameter", type.FullName));
        }

        public static InvalidGraphException NoDictionaryValue(string name)
        {
            return new InvalidGraphException(string.Format("Failed to read the value of a dictionary key value pair of property {0}", name));
        }
    }
}