using System;

namespace UrbanFox
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class EnableIfNotAttribute : BaseCompareAttribute
    {
        public EnableIfNotAttribute(string comparePropertyName, object compareValue) : base(comparePropertyName, compareValue) { }
    }
}
