using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MonoBehaviours.Network
{
    public class NetworkSceneLoader : MonoBehaviour
    {
        [Header("   Scene Data")]
        [SerializeField]
        private SceneDataScriptableObject SceneData;

        [Space(10)]
        [Header("Settings")]
        [SerializeField] private bool UnloadInsteadOfLoading;
        [SerializeField] private bool LoadOnStart;
        [SerializeField] private bool LoadWithDelay;
        [SerializeField] private float DelayInSeconds;
    
    
        [Space(10)]
        [SerializeField] private UnityEvent<string> OnScenLoaded;
        [SerializeField] private UnityEvent<string> OnScenUnloaded;
        private NetworkSceneManager.OnEventCompletedDelegateHandler OnScenLoadedAction;
        private NetworkSceneManager.OnEventCompletedDelegateHandler OnScenUnloadedAction;

        void Start()
        {
            if (LoadOnStart)
            {
                LoadScene();
            }
        }

        private void OnEnable()
        {
            OnScenLoadedAction += SceneLoaded;
            OnScenUnloadedAction += SceneUnloaded;
        }

        private void OnDisable()
        {
            OnScenLoadedAction -= SceneLoaded;
            OnScenUnloadedAction -= SceneUnloaded;
        }

        public void LoadScene()
        {
            StartCoroutine(LoadSceneRoutine());
        }

        private IEnumerator LoadSceneRoutine()
        {
            if (SceneManager.GetSceneByName(SceneData.SceneName).isLoaded && !UnloadInsteadOfLoading)
            {
                yield return null;
            }

            if (LoadWithDelay)
            {
                yield return new WaitForSeconds(DelayInSeconds);
            }

            if (UnloadInsteadOfLoading)
            {
                NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName(SceneData.SceneName));
                NetworkManager.Singleton.SceneManager.OnUnloadEventCompleted += OnScenUnloadedAction;
            }
            else
            {
                NetworkManager.Singleton.SceneManager.LoadScene(SceneData.SceneName, LoadSceneMode.Additive);
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnScenLoadedAction;
            }
            yield return null;
        }

        private void SceneUnloaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            OnScenUnloaded?.Invoke(sceneName);
        }

        private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            OnScenLoaded?.Invoke(sceneName);
        }

        public void SetSceneData(SceneDataScriptableObject data)
        {
            SceneData = data;
        }
    }
}
