using ScriptableObjectLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class EditorInitialSceneLoader : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField] SceneLoadConfigSO managerScene;

    [SerializeField] SceneLoadConfigSO currentStartScene;
    
    [SerializeField] AssetReference editorStartEventSO;

    void Awake()
    {
        if (!SceneManager.GetSceneByName(managerScene.sceneRef.editorAsset.name).isLoaded)
            managerScene.sceneRef.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += OnManagerLoaded;
    }

    void OnManagerLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        editorStartEventSO.LoadAssetAsync<SceneLoadEventSO>().Completed += NotifyOfEditorStart;
    }

    private void NotifyOfEditorStart(AsyncOperationHandle<SceneLoadEventSO> obj)
    {
        if (currentStartScene != null)
            obj.Result.Raise(currentStartScene);
        else
            print("Manager Scene loaded, but currentScene value not set to broadcast out.");
    }



#endif
}
