using UnityEngine;
using System.Collections;
using UnityEditor;

namespace BrokenVector.PolyCount
{
    public class PolyCountUtils
    {
        public struct PolygonCount
        {
            public int PolygonsWithoutChildren;
            public int PolygonsWithChildren;
            public int VerticesWithoutChildren;
            public int VerticesWithChildren;

            public PolygonCount(int polygonsWithoutChildren, int polygonsWithChildren, int verticesWithoutChildren, int verticesWithChildren)
            {
                PolygonsWithoutChildren = polygonsWithoutChildren;
                PolygonsWithChildren = polygonsWithChildren;
                VerticesWithoutChildren = verticesWithoutChildren;
                VerticesWithChildren = verticesWithChildren;
            }
        }

        public class EditorGUINavigationBar : IEnumerable
        {
            private const float height = 20f;

            public int CurrentTab;

            private string[] tabNames;

            private string prefsString;

            public int TabCount { get { return tabNames.Length; } }

            public EditorGUINavigationBar(string prefsName, string[] tabs, int defaultTab = 0)
            {
                this.tabNames = tabs;

                prefsString = prefsName + ".navigationbar.activetab";

                CurrentTab = EditorPrefs.GetInt(prefsString, defaultTab);
            }

            public void DrawNavigationBar()
            {
                var currCurrentTab = CurrentTab;

                GUILayout.Space(10);
                using (new GUILayout.HorizontalScope())
                {
                    for (int i = 0; i < TabCount; i++)
                    {
                        string styleName;
                        if (TabCount == 1)
                            styleName = "button";
                        else if (i == 0)
                            styleName = "buttonLeft";
                        else if (i == TabCount - 1)
                            styleName = "buttonRight";
                        else
                            styleName = "buttonMid";

                        GUIStyle style = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle(styleName);
                        var heightBckp = style.fixedHeight;
                        style.fixedHeight = height;

                        var colorBckp = GUI.backgroundColor;
                        if (i == currCurrentTab)
                            GUI.backgroundColor = Color.gray;

                        if (GUILayout.Button(tabNames[i], style))
                            currCurrentTab = i;

                        style.fixedHeight = heightBckp;
                        GUI.backgroundColor = colorBckp;
                    }
                }
                GUILayout.Space(10);

                if (CurrentTab != currCurrentTab)
                {
                    CurrentTab = currCurrentTab;
                    EditorPrefs.SetInt(prefsString, currCurrentTab);
                }
            }

            public IEnumerator GetEnumerator()
            {
                return tabNames.GetEnumerator();
            }
        }
    }
}
