using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace BrokenVector.PolyCount
{
    public class PolyCount : EditorWindow
    {

        private static int totalPolygonCount;
        private static int selectedPolygonCount;
        private static int selectedPolygonChildrenCount;
        private static int totalVertexCount;
        private static int selectedVertexCount;
        private static int selectedVertexChildrenCount;
        private static List<KeyValuePair<string, PolyCountUtils.PolygonCount>> selectionPolygonList = new List<KeyValuePair<string, PolyCountUtils.PolygonCount>>();
        private static List<KeyValuePair<string, PolyCountUtils.PolygonCount>> totalPolygonList = new List<KeyValuePair<string, PolyCountUtils.PolygonCount>>();
        private static bool showInSceneView = true;
        private static bool autoRefresh = true;
        private static bool sortByChildren = false;
        private static PolyCountUtils.EditorGUINavigationBar listsBar;
        private static PolyCountUtils.EditorGUINavigationBar modeBar;
        private static Vector2 totalListScroll;
        private static Vector2 selectionListScroll;

        [MenuItem("Window/BrokenVector/PolyCount"), MenuItem("Tools/PolyCount")]
        public static void Enable()
        {
            listsBar = new PolyCountUtils.EditorGUINavigationBar("polycount_1", new[] { "Total", "Selection" }, 0);
            modeBar = new PolyCountUtils.EditorGUINavigationBar("polycount_2", new[] { "Polygons", "Vertices" }, 0);

            PolyCount window = (PolyCount) GetWindow(typeof(PolyCount));

#if UNITY_5_4_OR_NEWER
            window.titleContent = new GUIContent("PolyCount");
#else
            window.title = "PolyCount";
#endif

            window.UpdatePolygonCount();

            if (SceneView.onSceneGUIDelegate != null)
            {
                SceneView.onSceneGUIDelegate -= OnScene;
                SceneView.onSceneGUIDelegate += OnScene;
            }

            sortByChildren = EditorPrefs.GetBool("polycount.settings.sortbychildren", false);
            autoRefresh = EditorPrefs.GetBool("polycount.settings.autorefresh", true);
            showInSceneView = EditorPrefs.GetBool("polycount.settings.sceneview", true);
        }

        void OnProjectChange()
        {
            if (SceneView.onSceneGUIDelegate != null)
            {
                SceneView.onSceneGUIDelegate -= OnScene;
                SceneView.onSceneGUIDelegate += OnScene;
            }

            if (autoRefresh)
            {
                UpdatePolygonCount();
            }
        }

        void OnSelectionChange()
        {
            if (autoRefresh)
            {
                UpdatePolygonCount();
            }
        }

        void OnHierarchyChange()
        {
            if (autoRefresh)
            {
                UpdatePolygonCount();
            }
        }

        private void UpdatePolygonCount()
        {
            if (listsBar == null)
            {
                listsBar = new PolyCountUtils.EditorGUINavigationBar("polycount_1", new[] { "Total", "Selection" }, 0);
            }

            if (modeBar == null)
            {
                modeBar = new PolyCountUtils.EditorGUINavigationBar("polycount_2", new[] { "Polygons", "Vertices" }, 0);
            }

            totalPolygonCount = 0;
            selectedPolygonCount = 0;
            selectedPolygonChildrenCount = 0;
            totalVertexCount = 0;
            selectedVertexCount = 0;
            selectedVertexChildrenCount = 0;

            totalPolygonList.Clear();

            foreach (MeshFilter filter in FindObjectsOfType<MeshFilter>())
            {
                if (filter.sharedMesh == null)
                    continue;

                int objectPolygonCount = filter.sharedMesh.triangles.Length / 3;
                int objectVertexCount = filter.sharedMesh.vertexCount;

                totalPolygonCount += objectPolygonCount;
                totalVertexCount += objectVertexCount;

                int objectPolygonCountChildren = 0;
                int objectVertexCountChildren = 0;

                foreach (MeshFilter childfilter in filter.gameObject.GetComponentsInChildren<MeshFilter>())
                {
                    if (childfilter.sharedMesh == null)  // thanks to Ultramattt
                        continue;

                    objectPolygonCountChildren += childfilter.sharedMesh.triangles.Length / 3;
                    objectVertexCountChildren += childfilter.sharedMesh.vertexCount;
                }

                totalPolygonList.Add(new KeyValuePair<string, PolyCountUtils.PolygonCount>(filter.gameObject.name, new PolyCountUtils.PolygonCount(objectPolygonCount, objectPolygonCountChildren, objectVertexCount, objectVertexCountChildren)));
            }

            selectionPolygonList.Clear();

            foreach (GameObject obj in Selection.gameObjects)
            {
                MeshFilter filter = obj.GetComponent<MeshFilter>();

                int objectPolygonCount = 0;
                int objectVertexCount = 0;

                if (filter != null && filter.sharedMesh != null)
                {
                    objectPolygonCount = filter.sharedMesh.triangles.Length / 3;
                    objectVertexCount = filter.sharedMesh.vertexCount;

                    selectedPolygonCount += objectPolygonCount;
                    selectedVertexCount += objectVertexCount;
                }

                int childrenPolygonCount = 0;
                int childrenVertexCount = 0;

                foreach (MeshFilter childfilter in obj.GetComponentsInChildren<MeshFilter>())
                {
                    if (childfilter.sharedMesh == null || childfilter == filter)  // thanks to Ultramattt
                        continue;

                    int objectPolygonCountChildren = childfilter.sharedMesh.triangles.Length / 3;
                    int objectVertexCountChildren = childfilter.sharedMesh.vertexCount;

                    selectedPolygonChildrenCount += objectPolygonCountChildren;
                    selectedVertexChildrenCount += objectVertexCountChildren;

                    childrenPolygonCount += objectPolygonCountChildren;
                    childrenVertexCount += objectVertexCountChildren;
                }

                selectionPolygonList.Add(new KeyValuePair<string, PolyCountUtils.PolygonCount>(obj.name, new PolyCountUtils.PolygonCount(objectPolygonCount, objectPolygonCount + childrenPolygonCount, objectVertexCount, objectVertexCount + childrenVertexCount)));
            }

            if (modeBar.CurrentTab == 0)
            {
                if (sortByChildren)
                {
                    selectionPolygonList.Sort((pair1, pair2) => pair2.Value.PolygonsWithChildren.CompareTo(pair1.Value.PolygonsWithChildren));
                    totalPolygonList.Sort((pair1, pair2) => pair2.Value.PolygonsWithChildren.CompareTo(pair1.Value.PolygonsWithChildren));
                }
                else
                {
                    selectionPolygonList.Sort((pair1, pair2) => pair2.Value.PolygonsWithoutChildren.CompareTo(pair1.Value.PolygonsWithoutChildren));
                    totalPolygonList.Sort((pair1, pair2) => pair2.Value.PolygonsWithoutChildren.CompareTo(pair1.Value.PolygonsWithoutChildren));
                }
            }
            else
            {
                if (sortByChildren)
                {
                    selectionPolygonList.Sort((pair1, pair2) => pair2.Value.VerticesWithChildren.CompareTo(pair1.Value.VerticesWithChildren));
                    totalPolygonList.Sort((pair1, pair2) => pair2.Value.VerticesWithChildren.CompareTo(pair1.Value.VerticesWithChildren));
                }
                else
                {
                    selectionPolygonList.Sort((pair1, pair2) => pair2.Value.VerticesWithoutChildren.CompareTo(pair1.Value.VerticesWithoutChildren));
                    totalPolygonList.Sort((pair1, pair2) => pair2.Value.VerticesWithoutChildren.CompareTo(pair1.Value.VerticesWithoutChildren));
                }
                    
            }
            

            SceneView.onSceneGUIDelegate -= OnScene;
            SceneView.onSceneGUIDelegate += OnScene;

            Repaint();
        }

        void OnGUI()
        {
            if (listsBar == null)
            {
                listsBar = new PolyCountUtils.EditorGUINavigationBar("polycount_1", new[] { "Total", "Selection" }, 0);
            }

            if (modeBar == null)
            {
                modeBar = new PolyCountUtils.EditorGUINavigationBar("polycount_2", new[] { "Triangles", "Vertices" }, 0);
            }

            modeBar.DrawNavigationBar();

            if (modeBar.CurrentTab == 0)
            {
                EditorGUILayout.LabelField("Total triangle count: " + totalPolygonCount, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Selected triangle count: " + selectedPolygonCount, EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField("Total vertex count: " + totalVertexCount, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Selected vertex count: " + selectedVertexCount, EditorStyles.boldLabel);
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            listsBar.DrawNavigationBar();

            EditorGUILayout.EndHorizontal();

            if (listsBar.CurrentTab == 0)
            {
                if (totalPolygonList.Count > 0)
                {
                    EditorGUILayout.BeginVertical("ShurikenEffectBg", GUILayout.MaxHeight(150.0f));

                    totalListScroll = EditorGUILayout.BeginScrollView(totalListScroll);

                    EditorGUILayout.BeginHorizontal();

                    float totalMaxWidthName;
                    float totalMaxWidthValue;

                    if (modeBar.CurrentTab == 0)
                    {
                        totalMaxWidthName = Mathf.Max(totalPolygonList.Max(o => EditorStyles.label.CalcSize(new GUIContent(o.Key)).x), EditorStyles.label.CalcSize(new GUIContent("Name")).x);
                        totalMaxWidthValue = Mathf.Max(totalPolygonList.Max(o => EditorStyles.label.CalcSize(new GUIContent(o.Value.PolygonsWithoutChildren.ToString())).x), EditorStyles.label.CalcSize(new GUIContent("Polygons")).x);

                        EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.MaxWidth(totalMaxWidthName));
                        EditorGUILayout.LabelField("Polygons", EditorStyles.boldLabel, GUILayout.MaxWidth(totalMaxWidthValue));
                        EditorGUILayout.LabelField("Polygons (+ children)", EditorStyles.boldLabel);
                    }
                    else
                    {
                        totalMaxWidthName = Mathf.Max(totalPolygonList.Max(o => EditorStyles.label.CalcSize(new GUIContent(o.Key)).x), EditorStyles.label.CalcSize(new GUIContent("Name")).x);
                        totalMaxWidthValue = Mathf.Max(totalPolygonList.Max(o => EditorStyles.label.CalcSize(new GUIContent(o.Value.VerticesWithoutChildren.ToString())).x), EditorStyles.label.CalcSize(new GUIContent("Vertices  ")).x);

                        EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.MaxWidth(totalMaxWidthName));
                        EditorGUILayout.LabelField("Vertices", EditorStyles.boldLabel, GUILayout.MaxWidth(totalMaxWidthValue));
                        EditorGUILayout.LabelField("Vertices (+ children)", EditorStyles.boldLabel);
                    }

                    EditorGUILayout.EndHorizontal();

                    foreach (KeyValuePair<string, PolyCountUtils.PolygonCount> pair in totalPolygonList)
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (modeBar.CurrentTab == 0)
                        {
                            EditorGUILayout.LabelField(pair.Key, EditorStyles.label, GUILayout.MaxWidth(totalMaxWidthName));
                            EditorGUILayout.LabelField(pair.Value.PolygonsWithoutChildren.ToString(), EditorStyles.label, GUILayout.MaxWidth(totalMaxWidthValue));
                            EditorGUILayout.LabelField(pair.Value.PolygonsWithChildren.ToString());
                        }
                        else
                        {
                            EditorGUILayout.LabelField(pair.Key, EditorStyles.label, GUILayout.MaxWidth(totalMaxWidthName));
                            EditorGUILayout.LabelField(pair.Value.VerticesWithoutChildren.ToString(), EditorStyles.label, GUILayout.MaxWidth(totalMaxWidthValue));
                            EditorGUILayout.LabelField(pair.Value.VerticesWithChildren.ToString());
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.HelpBox("No objects found!", MessageType.Warning);
                }
            }

            if (listsBar.CurrentTab == 1)
            {
                if (selectionPolygonList.Count > 0)
                {
                    EditorGUILayout.BeginVertical("ShurikenEffectBg", GUILayout.MaxHeight(150.0f));

                    selectionListScroll = EditorGUILayout.BeginScrollView(selectionListScroll);

                    EditorGUILayout.BeginHorizontal();

                    float selMaxWidthName;
                    float selMaxWidthValue;

                    if (modeBar.CurrentTab == 0)
                    {
                        selMaxWidthName = Mathf.Max(selectionPolygonList.Max(o => EditorStyles.label.CalcSize(new GUIContent(o.Key)).x), EditorStyles.label.CalcSize(new GUIContent("Name")).x);
                        selMaxWidthValue = Mathf.Max(selectionPolygonList.Max(o => EditorStyles.label.CalcSize(new GUIContent(o.Value.PolygonsWithoutChildren.ToString())).x), EditorStyles.label.CalcSize(new GUIContent("Polygons")).x);

                        EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.MaxWidth(selMaxWidthName));
                        EditorGUILayout.LabelField("Triangles", EditorStyles.boldLabel, GUILayout.MaxWidth(selMaxWidthValue), GUILayout.MaxWidth(selMaxWidthValue));
                        EditorGUILayout.LabelField("Triangles (+ children)", EditorStyles.boldLabel);
                    }
                    else
                    {
                        selMaxWidthName = Mathf.Max(selectionPolygonList.Max(o => EditorStyles.label.CalcSize(new GUIContent(o.Key)).x), EditorStyles.label.CalcSize(new GUIContent("Name")).x);
                        selMaxWidthValue = Mathf.Max(selectionPolygonList.Max(o => EditorStyles.label.CalcSize(new GUIContent(o.Value.VerticesWithoutChildren.ToString())).x), EditorStyles.label.CalcSize(new GUIContent("Vertices  ")).x);

                        EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.MaxWidth(selMaxWidthName));
                        EditorGUILayout.LabelField("Vertices", EditorStyles.boldLabel, GUILayout.MaxWidth(selMaxWidthValue), GUILayout.MaxWidth(selMaxWidthValue));
                        EditorGUILayout.LabelField("Vertices (+ children)", EditorStyles.boldLabel);
                    }

                    EditorGUILayout.EndHorizontal();

                    foreach (KeyValuePair<string, PolyCountUtils.PolygonCount> pair in selectionPolygonList)
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (modeBar.CurrentTab == 0)
                        {
                            EditorGUILayout.LabelField(pair.Key, EditorStyles.label, GUILayout.MaxWidth(selMaxWidthName));
                            EditorGUILayout.LabelField(pair.Value.PolygonsWithoutChildren.ToString(), EditorStyles.label, GUILayout.MaxWidth(selMaxWidthValue));
                            EditorGUILayout.LabelField(pair.Value.PolygonsWithChildren.ToString());
                        }
                        else
                        {
                            EditorGUILayout.LabelField(pair.Key, EditorStyles.label, GUILayout.MaxWidth(selMaxWidthName));
                            EditorGUILayout.LabelField(pair.Value.VerticesWithoutChildren.ToString(), EditorStyles.label, GUILayout.MaxWidth(selMaxWidthValue));
                            EditorGUILayout.LabelField(pair.Value.VerticesWithChildren.ToString());
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.HelpBox("No objects selected", MessageType.Warning);
                }
            }

            EditorGUILayout.Space();

            bool sortbychildrentemp = EditorGUILayout.Toggle("Sort by children", sortByChildren, EditorStyles.toggle);
            bool showinsceneviewtemp = EditorGUILayout.Toggle("Show in scene view", showInSceneView, EditorStyles.toggle);
            bool autorefreshtemp = EditorGUILayout.Toggle("Automatically refresh", autoRefresh, EditorStyles.toggle);

            if (sortbychildrentemp != sortByChildren)
            {
                EditorPrefs.SetBool("polycount.settings.sortbychildren", sortbychildrentemp);
                sortByChildren = sortbychildrentemp;
                UpdatePolygonCount();
            }

            if (showInSceneView != showinsceneviewtemp)
            {
                EditorPrefs.SetBool("polycount.settings.sceneview", showinsceneviewtemp);
                showInSceneView = showinsceneviewtemp;
            }

            if (autoRefresh != autorefreshtemp)
            {
                EditorPrefs.SetBool("polycount.settings.autorefresh", autorefreshtemp);
                autoRefresh = autorefreshtemp;
            }

            if (GUILayout.Button("Refresh manually"))
            {
                UpdatePolygonCount();
            }
        }

        private static void OnScene(SceneView sceneview)
        {
            if (showInSceneView)
            {
                Handles.BeginGUI();

                if (modeBar.CurrentTab == 0)
                {
                    GUILayout.Label("Total Triangles: " + totalPolygonCount, EditorStyles.label);
                    GUILayout.Label("Selected Triangles: " + selectedPolygonCount, EditorStyles.label);
                    GUILayout.Label("Selected Triangles (+Children): " + (selectedPolygonCount + selectedPolygonChildrenCount), EditorStyles.label);
                }
                else
                {
                    GUILayout.Label("Total Vertices: " + totalVertexCount, EditorStyles.label);
                    GUILayout.Label("Selected Vertices: " + selectedVertexCount, EditorStyles.label);
                    GUILayout.Label("Selected Vertices (+Children): " + (selectedVertexCount + selectedVertexChildrenCount), EditorStyles.label);
                }

                Handles.EndGUI();
            }
        }

        void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= OnScene;
        }
    }
}

