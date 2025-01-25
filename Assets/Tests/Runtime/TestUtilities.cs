using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public static class TestUtilities
{
    public static IEnumerator GetToGame()
    {
        yield return Addressables.LoadSceneAsync("Assets/Scenes/GameScenes/ManagerScene.unity", LoadSceneMode.Single);
        yield return Addressables.LoadSceneAsync("Assets/Scenes/GameScenes/GameScene.unity", LoadSceneMode.Additive);
    }
}