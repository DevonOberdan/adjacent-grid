using UnityEditor;
using UnityEngine;

public static class HelperMethods
{
    public static T Instantiate<T>(T objectToSpawn, Transform parent) where T : Object
    {
        T newObject = null;

        if (Application.isPlaying)
            newObject = Object.Instantiate(objectToSpawn, parent);
        else
            newObject = PrefabUtility.InstantiatePrefab(objectToSpawn, parent) as T;

        return newObject;
    }
}
