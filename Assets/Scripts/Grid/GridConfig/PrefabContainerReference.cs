using UnityEngine;

public class PrefabContainerReference : MonoBehaviour
{
    [field: SerializeField] public PrefabContainerSO PrefabContainer { get; private set; }
}