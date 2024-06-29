using UnityEditor;

using HayWay.Runtime.Attributes;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

namespace HayWay.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(BuildSceneAttribute))]
    public class ScenesAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String || property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.BeginProperty(position, label, property);

                BuildSceneAttribute attrib = this.attribute as BuildSceneAttribute;

                List<string> scenes = new List<string>();
                scenes.Add("<NoScene>");

                for (int a = 0; a < SceneManager.sceneCountInBuildSettings; a++)
                {
                    string p = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(a));
                    scenes.Add(p);
                }



                int index = 0;


                //Restore property Value
                if (property.propertyType == SerializedPropertyType.String)
                {
                    index = -1;
                    string propertyString = property.stringValue;
                    if (propertyString == "")
                    {
                        index = 0;
                    }
                    else
                    {
                        //check if there is an entry that matches the entry and get the index
                        //we skip index 0 as that is a special custom case
                        for (int i = 1; i < scenes.Count; i++)
                        {
                            if (scenes[i] == propertyString)
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    index = property.intValue + 1;
                }

                //Draw the popup box with the current selected index
                index = EditorGUI.Popup(position, label.text, index, scenes.ToArray());

                if (property.propertyType == SerializedPropertyType.Integer)
                {
                   
                    property.intValue = index-1;

                }
                else
                {
                    //Adjust the actual string value of the property based on the selection
                    if (index == 0)
                    {
                        property.stringValue = "";
                    }
                    else if (index >= 1)
                    {
                        property.stringValue = scenes[index];
                    }
                    else
                    {
                        property.stringValue = "";
                    }
                }
                EditorGUI.EndProperty();
            }

        }
    }

}