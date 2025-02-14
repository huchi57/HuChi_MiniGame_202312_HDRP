using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UrbanFox
{
    public static class GameObjectExtensions
    {
        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponentInChildren<T>();
            return component != null;
        }

        public static bool TryGetComponentInChildrenIncludeInactive<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponentInChildren<T>(includeInactive: true);
            return component != null;
        }

        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponentInParent<T>();
            return component != null;
        }

        public static bool TryGetComponentInParentIncludeInactive<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponentInParent<T>(includeInactive: true);
            return component != null;
        }

        public static bool TryGetComponentInChildren<T>(this Component transform, out T component) where T : Component
        {
            if (transform == null)
            {
                component = null;
                return false;
            }
            return transform.gameObject.TryGetComponentInChildren(out component);
        }

        public static bool TryGetComponentInChildrenIncludeInactive<T>(this Component transform, out T component) where T : Component
        {
            if (transform == null)
            {
                component = null;
                return false;
            }
            return transform.gameObject.TryGetComponentInChildrenIncludeInactive(out component);
        }

        public static bool TryGetComponentInParent<T>(this Component transform, out T component) where T : Component
        {
            if (transform == null)
            {
                component = null;
                return false;
            }
            return transform.gameObject.TryGetComponentInParent(out component);
        }

        public static bool TryGetComponentInParentIncludeInactive<T>(this Component transform, out T component) where T : Component
        {
            if (transform == null)
            {
                component = null;
                return false;
            }
            return transform.gameObject.TryGetComponentInParentIncludeInactive(out component);
        }

        public static T[] GetComponentsInChildrenExcludeSelf<T>(this GameObject gameObject, bool includeInactive = false) where T : Component
        {
            if (gameObject == null)
            {
                return null;
            }
            var children = gameObject.GetComponentsInChildren<T>(includeInactive);
            return children.IsNullOrEmpty() ? null : children.Where(child => child != null && child.gameObject != gameObject).ToArray();
        }

        public static T[] GetComponentsInChildrenExcludeSelf<T>(this Component transform, bool includeInactive = false) where T : Component
        {
            if (transform == null)
            {
                return null;
            }
            return transform.gameObject.GetComponentsInChildren<T>(includeInactive);
        }

        public static void DeleteAllChildren(this Transform transform)
        {
            if (transform == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                while (transform.childCount > 0)
                {
                    Object.Destroy(transform.GetChild(0).gameObject);
                }
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(transform.gameObject, "Delete All Children");
                var undoStepGroup = UnityEditor.Undo.GetCurrentGroup();
                while (transform.childCount > 0)
                {
                    UnityEditor.Undo.DestroyObjectImmediate(transform.GetChild(0).gameObject);
                }
                UnityEditor.Undo.CollapseUndoOperations(undoStepGroup);
#else
                while (transform.childCount > 0)
                {
                    Object.DestroyImmediate(transform.GetChild(0).gameObject);
                }
#endif
            }
        }

        public static void DeleteAllChildren(this GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            gameObject.transform.DeleteAllChildren();
        }

        public static void MoveObjectToScene(this GameObject gambeObject, Scene scene)
        {
            SceneManager.MoveGameObjectToScene(gambeObject, scene);
        }

        public static void MoveObjectToScene(this Component component, Scene scene)
        {
            if (component && scene != null && scene.isLoaded)
            {
                SceneManager.MoveGameObjectToScene(component.gameObject, scene);
            }
        }
    }
}
