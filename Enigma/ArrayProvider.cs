using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma
{
    public class ArrayProvider
    {

        public static T[] ToArray<T>(ICollection<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            if (collection.Count == 0) return new T[0];

            var arr = new T[collection.Count];
            collection.CopyTo(arr, 0);
            return arr;
        }

        public static T[,] To2DArray<T>(ICollection<ICollection<T>> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            var size0 = collection.Count;
            if (size0 == 0) return new T[0,0];

            var size1 = collection.First().Count;
            if (size1 == 0) return new T[size0,0];

            var arr = new T[size0, size1];

            int r0 = 0, r1 = 0;
            foreach (var c0 in collection) {
                if (c0.Count != size1)
                    throw new IndexOutOfRangeException("Inner collection sizes differ from another");

                foreach (var src in c0) {
                    arr[r0, r1++] = src;
                }
                r0++;
                r1 = 0;
            }

            return arr;
        }

        public static T[,,] To3DArray<T>(ICollection<ICollection<ICollection<T>>> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            var size0 = collection.Count;
            if (size0 == 0) return new T[0, 0, 0];

            var c0First = collection.First();
            var size1 = c0First.Count;
            if (size1 == 0) return new T[size0, 0, 0];

            var size2 = c0First.First().Count;
            if (size2 == 0) return new T[size0, size1, 0];

            var arr = new T[size0, size1, size2];

            int r0 = 0, r1 = 0, r2 = 0;
            foreach (var c0 in collection) {
                if (c0.Count != size1)
                    throw new IndexOutOfRangeException("Inner rank 1 collection sizes differ from another");

                foreach (var c1 in c0) {
                    if (c1.Count != size2)
                        throw new IndexOutOfRangeException("Inner rank 2 collection sizes differ from another");

                    foreach (var src in c1) {
                        arr[r0, r1, r2++] = src;
                    }
                    r1++;
                    r2 = 0;
                }
                r0++;
                r1 = 0;
            }

            return arr;
        }

    }
}
