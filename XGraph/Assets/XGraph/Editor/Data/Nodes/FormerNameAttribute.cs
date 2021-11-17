using System;

namespace XGraph
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    class FormerNameAttribute : Attribute
    {
        public string fullName { get; private set; }

        public FormerNameAttribute(string fullName)
        {
            this.fullName = fullName;
        }
    }
}
