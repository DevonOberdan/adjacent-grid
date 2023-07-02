using ScriptableObjectLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadRequester : MonoBehaviour
{
    [SerializeField] SceneLoadEventSO loadEvent = default;
    [SerializeField] SceneLoadConfigSO defaultScene;

    public void Request(SceneLoadConfigSO sceneToLoad = null)
    {
        if (sceneToLoad != null)
            loadEvent.OnRequestLoad?.Invoke(sceneToLoad);
        else
            loadEvent.OnRequestLoad?.Invoke(defaultScene);
    }
}
