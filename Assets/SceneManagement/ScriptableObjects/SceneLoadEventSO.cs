using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace ScriptableObjectLibrary {

    [CreateAssetMenu(fileName = "SceneLoadEventSO", menuName = "SceneLoad ScriptableObjects/SceneLoadEventSO", order=0)]
    public class SceneLoadEventSO : ScriptableObject
    {
        public UnityAction<SceneLoadConfigSO> OnRequestLoad;

        public void Raise(SceneLoadConfigSO sceneToLoad)
        {
            if(OnRequestLoad != null) 
                OnRequestLoad(sceneToLoad);
            else
            {
                Debug.Log("No listeners on OnRequestLoad. WHY??");
            }
            //OnRequestLoad?.Invoke(sceneToLoad);
        }
    }
}
