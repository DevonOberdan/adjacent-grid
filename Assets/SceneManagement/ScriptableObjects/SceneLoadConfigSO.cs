using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace ScriptableObjectLibrary {

    [CreateAssetMenu(fileName = "SceneLoadConfigSO", menuName = "SceneLoad ScriptableObjects/SceneLoadConfigSO", order=0)]
    public class SceneLoadConfigSO : DataConfigSO
    {
        //public string sceneName;
        public AssetReference sceneRef;
    }
}
