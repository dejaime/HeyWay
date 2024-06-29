
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

namespace HayWay.Editor.Attributes
{
    public static class AttributesEditorExtensions
    {

        public static T GetPropertyAttribute<T>(this SerializedProperty prop, bool inherit) where T : PropertyAttribute
        {
            if (prop == null) { Debug.Log(0); return null; }

            Type t = prop.serializedObject.targetObject.GetType();
            FieldInfo f = null;
            PropertyInfo p = null;

            //return null;
            var path = prop.propertyPath;
            var elements = path.Split('.');


            string[] slices = prop.propertyPath.Split('.');
            System.Type type = prop.serializedObject.targetObject.GetType();


            //Field
            for (int i = 0; i < slices.Length; i++)
            {
                if (slices[i] == "Array")
                {
                    i++; //skips "data[x]"
                    type = type.GetElementType(); //gets info on array elements
                }
                else
                {
                  
                    f = type.GetField(slices[i], (BindingFlags)(-1));
                    p = type.GetProperty(slices[i], (BindingFlags)(-1));
                    type = f.FieldType;

                }
            }


            T[] attributes;
            if (f != null)
            {
                attributes = f.GetCustomAttributes(typeof(T), inherit) as T[];

            }
            else if (p != null)
            {
                attributes = p.GetCustomAttributes(typeof(T), inherit) as T[];
            }
            else
            {
                return null;
            }

            return attributes.Length > 0 ? attributes[0] : null;
        }
    }

}
