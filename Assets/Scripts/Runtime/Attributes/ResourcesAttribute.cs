
using System;
using UnityEngine;

namespace HayWay.Runtime.Attributes
{
    public class ResourcesAttribute : PropertyAttribute
    {
        public string path;
        public Type mtype;
        public bool returnPath = false;

        public ResourcesAttribute(string path, Type mtype, bool returnPath)
        {
            this.path = path;
            this.mtype = mtype;
            this.returnPath = returnPath;
        }

    }
}
