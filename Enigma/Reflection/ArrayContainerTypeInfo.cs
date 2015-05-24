using System;

namespace Enigma.Reflection
{
    public class ArrayContainerTypeInfo : CollectionContainerTypeInfo
    {
        private readonly int _ranks;

        public ArrayContainerTypeInfo(Type elementType, int ranks) : base(elementType)
        {
            _ranks = ranks;
        }

        public int Ranks
        {
            get { return _ranks; }
        }
    }
}