using System.Collections.Generic;
using UnityEngine;

namespace UrbanFox
{
    public static class CollectionsExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            if (collection == null)
            {
                return true;
            }
            return collection.Count <= 0;
        }

        public static bool IsInRange<T>(this int index, ICollection<T> collection)
        {
            if (collection.IsNullOrEmpty())
            {
                return false;
            }
            return index.IsInRange(0, collection.Count - 1);
        }

        public static bool IsIndexInRange<T>(this ICollection<T> collection, int index)
        {
            return index.IsInRange(collection);
        }

        public static T SelectRandom<T>(this IList<T> list)
        {
            if (list.IsNullOrEmpty())
            {
                return default;
            }
            // Random.Range is max inclusive, so do not minus one
            return list[Random.Range(0, list.Count)];
        }
    }
}
