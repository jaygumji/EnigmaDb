using System.Collections.Generic;
using System.Linq;
using Enigma.Test.Fakes;

namespace Enigma.Test.Serialization
{
    internal class CategoryComparer : IEqualityComparer<Category>
    {
        public bool Equals(Category x, Category y)
        {
            if (!(x.Name == y.Name && x.Description == y.Description))
                return false;

            if (x.Image == null && y.Image == null) return true;
            if (x.Image == null || y.Image == null) return false;
            return x.Image.SequenceEqual(y.Image);
        }

        public int GetHashCode(Category obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}