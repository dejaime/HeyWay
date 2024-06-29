using UnityEditor;
using HayWay.Runtime.Attributes;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using HayWay.Runtime.Extensions;

namespace HayWay.Editor.Attributes
{


    [CustomPropertyDrawer(typeof(ResourcesAttribute))]
    public class ResourcesAttributeDrawer : PropertyDrawer
    {

     

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);

                ResourcesAttribute attrib = this.attribute as ResourcesAttribute;

                List<string> objects = new List<string>();
                objects.Add("<NoFile>");

                var resources = Resources.LoadAll(attrib.path, attrib.mtype).ToList();

                foreach (Object ob in resources)
                {
                    objects.Add(ob.name);
                }

                string propertyString = property.stringValue;
                int index = -1;

                if (propertyString == "")
                {
                    index = 0;
                }
                else
                {
                    
                    for (int i = 1; i < objects.Count; i++)
                    {
                        if (objects[i] == propertyString || propertyString == attrib.path.PathCombine(objects[i]))
                        {
                            index = i;
                            break;
                        }
                    }
                }

                //Draw the popup box with the current selected index
                index = EditorGUI.Popup(position, label.text, index, objects.ToArray());

                //Adjust the actual string value of the property based on the selection
                if (index <= 0)
                {
                    property.stringValue = "";

                }
                else 
                {
                    property.stringValue = attrib.returnPath ? attrib.path.PathCombine(objects[index]) : objects[index];

                }

            }

            EditorGUI.EndProperty();
        }
    }

}