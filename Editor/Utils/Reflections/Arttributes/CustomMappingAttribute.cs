using System;

namespace Yueby.Utils.Reflections
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CustomMappingAttribute : Attribute
    {
        public string SourceName { get; }

        public CustomMappingAttribute(string sourceName)
        {
            SourceName = sourceName;
        }
    }
}