using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

namespace Enigma.Binary.Algorithm
{

    /// <summary>
    /// Binary search algorithm
    /// </summary>
    public static class BinarySearch
    {
        public static int Search<T>(IList<T> list, T value)
        {
            return Search<T>(list, 0, list.Count, value, Comparer<T>.Default);
        }

        public static int Search<T>(IList<T> list, int startIndex, int lengthToSearch, T value, IComparer<T> comparer)
        {
            if (list == null) throw new ArgumentNullException("list");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException();

            if (lengthToSearch < 0 || list.Count - startIndex < lengthToSearch)
                throw new ArgumentException("Invalid lengthToSearch, must be a valid length between startIndex and the count of the list");

            if (comparer == null) comparer = Comparer<T>.Default;

            int left = startIndex;
            int right = startIndex + lengthToSearch - 1;
            while (left <= right) {
                int median = left + (right - left >> 1);
                var compareResult = comparer.Compare(list[median], value);
                if (compareResult == 0) return median;

                if (compareResult < 0)
                    left = median + 1;
                else
                    right = median - 1;
            }

            return ~left;
        }

    }
}
