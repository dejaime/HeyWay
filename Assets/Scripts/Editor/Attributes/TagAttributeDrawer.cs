using UnityEditor;

using HayWay.Runtime.Attributes;
using UnityEngine;
using System.Collections.Generic;

namespace HayWay.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);

                var attrib = this.attribute as TagAttribute;

                if (attrib.useList)
                {
                    property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                }
                else
                {
                    //generate the taglist + custom tags
                    List<string> tagList = new List<string>();
                    tagList.Add("<NoTag>");
                    tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);

                    string propertyString = property.stringValue;
                    int index = -1;

                    if (propertyString == "")
                    {
                        index = 0;
                    }
                    else
                    {
                        //check if there is an entry that matches the entry and get the index
                        //we skip index 0 as that is a special custom case
                        for (int i = 1; i < tagList.Count; i++)
                        {
                            if (tagList[i] == propertyString)
                            {
                                index = i;
                                break;
                            }
                        }
                    }

                    //Draw the popup box with the current selected index
                    index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());

                    //Adjust the actual string value of the property based on the selection
                    if (index == 0)
                    {
                        property.stringValue = "";
                    }
                    else if (index >= 1)
                    {
                        property.stringValue = tagList[index];
                    }
                    else
                    {
                        property.stringValue = "";
                    }
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }

}