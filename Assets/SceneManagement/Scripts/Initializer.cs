using ScriptableObjectLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class Initializer : MonoBehaviour
{
    [SerializeField] SceneLoadConfigSO managerScene;

    [SerializeField] SceneLoadConfigSO sceneToLoad;

    // assetreference so that we dont have duplicate that is stranded from the rest of the game!
    [SerializeField] AssetReference sceneLoadEvent;

    void Start()
    {
        managerScene.sceneRef.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += GrabSceneLoadAsset;    
    }

    private void GrabSceneLoadAsset(AsyncOperationHandle<SceneInstance> obj)
    {
        sceneLoadEvent.LoadAssetAsync<SceneLoadEventSO>().Completed += LoadRealScene;
    }

    private void LoadRealScene(AsyncOperationHandle<SceneLoadEventSO> obj)
    {
        obj.Result.Raise(sceneToLoad);

        //unload this current scene, which is Initializer scene
        SceneManager.UnloadSceneAsync(0);
    }
}
