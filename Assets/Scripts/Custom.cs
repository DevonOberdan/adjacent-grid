using UnityEditor;
using UnityEngine;

namespace FinishOne.GeneralUtilities
{
    /// <summary>
    /// Class containing "overrides"/wrappers for existing Unity functionality.
    /// 
    /// Usage:
    /// When needing to contextually Instantiate normally at runtime or with preserved Prefab linking
    /// linking in the editor, can call Custom.Instantiate(...)
    /// </summary>
    public static class Custom
    {
        public static T Instantiate<T>(T original, Transform parent) where T : Object
        {
            T newObject = null;

            if (Application.isPlaying)
                newObject = Object.Instantiate(original, parent);
            else
                newObject = PrefabUtility.InstantiatePrefab(original, parent) as T;

            return newObject;
        }
    }
}