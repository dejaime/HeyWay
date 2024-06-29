using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HayWay.Runtime.Attributes
{
    public class RangeStepAttribute : PropertyAttribute
    {
        public float min;
        public float max;
        public float step;

        public RangeStepAttribute(float min, float max, float step=0)
        {
            this.min = min;
            this.max = max;
            this.step = step;
        }
    }
}