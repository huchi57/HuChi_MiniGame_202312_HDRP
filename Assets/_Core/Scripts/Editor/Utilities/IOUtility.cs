using System.IO;
using UnityEngine;
using UnityEditor;

namespace UrbanFox
{
    public static class IOUtility
    {
        public static string GetCurrentFolderInProjectPanel()
        {
            string path = "Assets/";

            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }
    }
}
