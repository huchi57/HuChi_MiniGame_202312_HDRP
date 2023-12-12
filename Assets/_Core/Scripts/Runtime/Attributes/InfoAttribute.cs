using System;
using UnityEngine;

namespace UrbanFox
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class InfoAttribute : PropertyAttribute
    {
        public string TooltipContent { get; private set; }

        public InfoAttribute(string tooltipContent)
        {
            TooltipContent = tooltipContent;
        }
    }
}
