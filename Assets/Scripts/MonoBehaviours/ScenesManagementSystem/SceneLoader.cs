using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("   Scene Data")]
    [SerializeField]
    private SceneDataScriptableObject SceneData;

    [Space(10)]
    [Header("Settings")]
    [SerializeField] private bool UnloadInsteadOfLoading;
    [SerializeField] private bool LoadSceneAsync = true;
    [SerializeField] private bool LoadOnStart;
    
    
    [Space(10)]
    [SerializeField] private UnityEvent<string> OnScenLoaded;
    [SerializeField] private UnityEvent<string> OnScenUnloaded;
    private Action<AsyncOperation> OnScenLoadedAction;
    private Action<AsyncOperation> OnScenUnloadedAction;

    void Start()
    {
        SceneData.Initialize();
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
        if (SceneManager.GetSceneByName(SceneData.SceneName).isLoaded && !UnloadInsteadOfLoading)
        {
            return;
        }
        
        if (UnloadInsteadOfLoading)
        {
            SceneManager.UnloadSceneAsync(SceneData.SceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects).completed += OnScenUnloadedAction;
        }
        else
        {
            if (LoadSceneAsync)
            {
                SceneManager.LoadSceneAsync(SceneData.SceneName, LoadSceneMode.Additive).completed += OnScenLoadedAction;
            }
            else
            {
                SceneManager.LoadScene(SceneData.SceneName);
            }
        }
    }

    private void SceneUnloaded(AsyncOperation op)
    {
        OnScenUnloaded?.Invoke(SceneData.SceneName);
    }

    private void SceneLoaded(AsyncOperation op)
    {
        OnScenLoaded?.Invoke(SceneData.SceneName);
    }
}
