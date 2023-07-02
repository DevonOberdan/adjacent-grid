using ScriptableObjectLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] SceneLoadEventSO loadEvent = default;
    [SerializeField] SceneLoadConfigSO startScene;

    [SerializeField] SceneLoadEventSO editorStartEvent;

    SceneLoadConfigSO currentScene;
    SceneLoadConfigSO newSceneToLoad;

    AsyncOperationHandle<SceneInstance> sceneOperationHandle;
    AsyncOperationHandle<SceneInstance> unloadSceneOperationHandle;

#if UNITY_EDITOR
    AsyncOperation editorStartUnloadOperation;
#endif

    private void OnEnable()
    {
        loadEvent.OnRequestLoad += LoadScene;

#if UNITY_EDITOR
        editorStartEvent.OnRequestLoad += EditorStart;
#endif
    }

    private void OnDisable()
    {
        loadEvent.OnRequestLoad -= LoadScene;
#if UNITY_EDITOR
        editorStartEvent.OnRequestLoad -= EditorStart;
#endif
    }

    void EditorStart(SceneLoadConfigSO editorSceneStarted)
    {
        currentScene = editorSceneStarted;
    }

    void LoadScene(SceneLoadConfigSO gameScene)
    {
        newSceneToLoad = gameScene;

        UnloadPreviousScene();

        if(currentScene == newSceneToLoad)
        {
            if (unloadSceneOperationHandle.IsValid())
                unloadSceneOperationHandle.Completed += handle => LoadNewScene();
#if UNITY_EDITOR
            else
                editorStartUnloadOperation.completed += action => LoadNewScene();
#endif
        }
        else
        {
            LoadNewScene();
        }
    }

    void UnloadPreviousScene()
    {
        if(currentScene != null)
        {
            if (currentScene.sceneRef.OperationHandle.IsValid())
                unloadSceneOperationHandle = currentScene.sceneRef.UnLoadScene();
#if UNITY_EDITOR
            else
                editorStartUnloadOperation = SceneManager.UnloadSceneAsync(currentScene.sceneRef.editorAsset.name);
#endif
        }
    }

    void LoadNewScene()
    {
        sceneOperationHandle = newSceneToLoad.sceneRef.LoadSceneAsync(LoadSceneMode.Additive, true, 0);
        sceneOperationHandle.Completed += OnNewSceneLoaded;
    }

    private void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        currentScene = newSceneToLoad;
        newSceneToLoad = null;

        SceneManager.SetActiveScene(obj.Result.Scene);
    }
}
