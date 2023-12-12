using System;
using UnityEngine;

namespace UrbanFox
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class NonEditableAttribute : PropertyAttribute { }
}
