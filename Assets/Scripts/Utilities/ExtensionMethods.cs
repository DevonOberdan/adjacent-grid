using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ExtensionMethods
{
    public static TObj GetPrefabFromSource<TObj>(this TObj obj) where TObj : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return PrefabUtility.GetCorrespondingObjectFromSource(obj);
        }
#endif

        TObj prefab = default;

        if (obj is GameObject go)
        {
            prefab = GetPrefabFromGameObject(go) as TObj;
        }
        else if (obj is Component c)
        {
            prefab = GetPrefabFromComponent(obj as Component) as TObj;
        }

        return prefab;
    }

    private static UnityEngine.Object GetPrefabFromGameObject(GameObject obj)
    {
        if (!obj.TryGetComponent(out PrefabContainerReference prefabContainerRef))
        {
            Debug.LogWarning($"Object does not have a {nameof(PrefabContainerReference)} instance attached.");
            return null;
        }

        if (prefabContainerRef.PrefabContainer == null)
        {
            Debug.LogWarning($"{nameof(PrefabContainerReference)} instance's {nameof(PrefabContainerSO)} reference is empty.");
            return null;
        }

        GameObject prefab = prefabContainerRef.PrefabContainer.Prefab;

        if (prefab == null)
        {
            Debug.LogWarning($"Found {nameof(PrefabContainerSO)}, but it has no prefab assigned.");
        }

        return prefab;
    }

    private static T GetPrefabFromComponent<T>(T component) where T : Component
    {
        GameObject prefabObj = (GameObject)GetPrefabFromGameObject(component.gameObject);

        if (prefabObj == null)
        {
            Debug.Log("GameObject prefab is null.");
            return null;
        }

        T prefabComponent = (T)prefabObj.GetComponent(component.GetType());

        if (prefabComponent == null)
        {
            Debug.LogWarning($"Valid prefab found ({component.gameObject.name}), but it does not contain a component of type {nameof(T)}.\n" +
                             $"Add the desired component to the given prefab, or call " +
                             $"TryGetPrefabAtRuntime(this Component c, out GameObject prefab) instead.");
            return null;
        }

        return prefabComponent;
    }

    public static Renderer GrabRenderer(this GameObject go)
    {
        if (!go.TryGetComponent(out Renderer renderer))
        {
            renderer = go.GetComponentInChildren<Renderer>();

            if (renderer == null)
            {
                Debug.LogError("No Renderer in GridPiece", go);
            }
        }
        return renderer;
    }

    public static void SetColor(this Renderer renderer, Color color)
    {
        MeshRenderer mesh = renderer as MeshRenderer;
        SpriteRenderer sprite = renderer as SpriteRenderer;

        if (mesh != null)
        {
            mesh.material.color = color;
        }
        else if (sprite != null)
        {
            sprite.color = color;
        }
    }

    public static Color GetColor(this Renderer rend)
    {
        Color color = Color.white;

        MeshRenderer mesh = rend as MeshRenderer;
        SpriteRenderer sprite = rend as SpriteRenderer;

        if (mesh != null)
        {
            color = Application.isPlaying ? mesh.material.color : mesh.sharedMaterial.color;
        }
        else if (sprite != null)
        {
            color = sprite.color;
        }
        else
        {
            Debug.LogError("Color not found in GameObject " + rend.name, rend.gameObject);
        }

        return color;
    }
}
