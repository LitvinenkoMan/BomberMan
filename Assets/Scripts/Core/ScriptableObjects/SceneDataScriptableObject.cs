using System;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class SceneDataScriptableObject : ScriptableObject
    { 
        [SerializeField]
        private string _sceneName;

        public string SceneName => _sceneName;
    }
}
