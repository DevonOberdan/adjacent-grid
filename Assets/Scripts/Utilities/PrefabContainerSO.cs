using UnityEngine;

[CreateAssetMenu(fileName =nameof(PrefabContainerSO), menuName =nameof(PrefabContainerSO), order = 0)]
public class PrefabContainerSO : ScriptableObject
{
    public GameObject Prefab;
}