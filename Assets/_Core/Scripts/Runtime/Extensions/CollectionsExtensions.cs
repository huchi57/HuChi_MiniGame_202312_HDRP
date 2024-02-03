using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UrbanFox
{
    public static class CollectionsExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return true;
            }
            return collection.Count() <= 0;
        }

        public static bool IsInRange<T>(this int index, IEnumerable<T> collection)
        {
            if (collection.IsNullOrEmpty())
            {
                return false;
            }
            return index.IsInRange(0, collection.Count() - 1);
        }

        public static bool IsIndexInRange<T>(this IEnumerable<T> collection, int index)
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

        public static bool Contains<T>(this IEnumerable<T> list, T item)
        {
            if (item == null)
            {
                return true;
            }
            if (list.IsNullOrEmpty())
            {
                return false;
            }
            foreach (var listItem in list)
            {
                if (item.Equals(listItem))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
