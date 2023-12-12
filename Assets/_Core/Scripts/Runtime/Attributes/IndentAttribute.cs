using System;
using UnityEngine;

namespace UrbanFox
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class IndentAttribute : PropertyAttribute
    {
        public int IndentIncrementLevel { get; private set; }

        public IndentAttribute(int indentLevel = 1)
        {
            IndentIncrementLevel = indentLevel;
        }
    }
}
