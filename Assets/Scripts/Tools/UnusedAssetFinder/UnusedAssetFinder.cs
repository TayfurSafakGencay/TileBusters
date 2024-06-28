using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools.UnusedAssetFinder
{
    public class UnusedAssetsFinder : EditorWindow
    {
        private readonly List<string> excludedPaths = new();
        private readonly List<string> unusedAssets = new();
        private readonly List<bool> selectedAssets = new();
        private Vector2 scrollPosition = Vector2.zero;
        private string newExcludedPath = "";

        [MenuItem("Tools/Find Unused Assets")]
        public static void ShowWindow()
        {
            GetWindow<UnusedAssetsFinder>("Find Unused Assets");
        }

        private void OnGUI()
        {
            GUILayout.Label("Excluded Paths:");

            foreach (string path in excludedPaths)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(path);
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    excludedPaths.Remove(path);
                    UpdateUnusedAssetsList();
                    break;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            newExcludedPath = GUILayout.TextField(newExcludedPath);
            if (GUILayout.Button("Add Path", GUILayout.Width(80)))
            {
                if (!string.IsNullOrEmpty(newExcludedPath))
                {
                    excludedPaths.Add(newExcludedPath);
                    newExcludedPath = "";
                    UpdateUnusedAssetsList();
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Find Unused Assets"))
            {
                FindUnusedAssets();
            }

            GUILayout.Label("Unused Assets:");
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < unusedAssets.Count; i++)
            {
                GUI.backgroundColor = i % 2 == 0 ? Color.gray : Color.white; // Satırları renklendirme
                GUILayout.BeginHorizontal("box");
                GUI.backgroundColor = Color.white;

                selectedAssets[i] = GUILayout.Toggle(selectedAssets[i], "", GUILayout.Width(20));

                GUILayout.Label(unusedAssets[i], GUILayout.Width(position.width - 150));

                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    SelectAssetInProject(unusedAssets[i]);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("Select All"))
            {
                SelectAllSelectedAssetsInProject();
            }

            if (GUILayout.Button("Delete Selected"))
            {
                DeleteSelectedAssetsFromProject();
            }
        }

        private void FindUnusedAssets()
        {
            unusedAssets.Clear();
            selectedAssets.Clear();

            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            HashSet<string> usedAssets = new(AssetDatabase.GetDependencies(EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes), true));

            foreach (string assetPath in allAssetPaths)
            {
                if (!IsPathExcluded(assetPath) && assetPath.StartsWith("Assets/") && !usedAssets.Contains(assetPath))
                {
                    unusedAssets.Add(assetPath);
                    selectedAssets.Add(false);
                }
            }

            // Sort unusedAssets list by name
            unusedAssets.Sort();

            Debug.Log("Unused assets found: " + unusedAssets.Count);
        }

        private bool IsPathExcluded(string path)
        {
            foreach (string excludedPath in excludedPaths)
            {
                if (path.StartsWith(excludedPath))
                {
                    return true;
                }
            }
            return false;
        }

        private void SelectAssetInProject(string assetPath)
        {
            Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        private void SelectAllSelectedAssetsInProject()
        {
            for (int i = 0; i < selectedAssets.Count; i++)
            {
                selectedAssets[i] = true;
            }
        }

        private void DeleteSelectedAssetsFromProject()
        {
            for (int i = selectedAssets.Count - 1; i >= 0; i--)
            {
                if (selectedAssets[i])
                {
                    if (AssetDatabase.DeleteAsset(unusedAssets[i]))
                    {
                        Debug.Log("Deleted asset: " + unusedAssets[i]);
                    }
                    else
                    {
                        Debug.LogError("Failed to delete asset: " + unusedAssets[i]);
                    }
                    unusedAssets.RemoveAt(i);
                    selectedAssets.RemoveAt(i);
                }
            }
        }

        private void UpdateUnusedAssetsList()
        {
            // Trigger a reevaluation of unused assets when paths are added or removed
            FindUnusedAssets();
        }
    }
}
