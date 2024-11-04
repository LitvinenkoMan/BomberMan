using System;
using System.Collections.Generic;
using Core.ScriptableObjects;
using Runtime.NetworkBehaviours;
using TMPro;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    [RequireComponent(typeof(SceneLoader))]
    public class LevelDataSelector : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown LevelsDropdown;

        [SerializeField]
        private List<LevelData> LevelsData;

        private SceneLoader _sceneLoader;
        
        void Start()
        {
            _sceneLoader = GetComponent<SceneLoader>();
            UpdateLevelsList();
            OnSelectedLevelChanged(0);
        }

        private void OnEnable()
        {
            LevelsDropdown.onValueChanged.AddListener(OnSelectedLevelChanged);
        }

        private void OnDisable()
        {
            LevelsDropdown.onValueChanged.RemoveListener(OnSelectedLevelChanged);
        }

        private void UpdateLevelsList()
        {
            LevelsDropdown.ClearOptions();

            List<string> levelsNames = new List<string>();

            foreach (var LD in LevelsData)
            {
                levelsNames.Add(LD.LevelName);
            }

            LevelsDropdown.AddOptions(levelsNames);
        }

        private void OnSelectedLevelChanged(int index)
        {
            foreach (var LD in LevelsData)
            {
                if (LD.LevelName == LevelsDropdown.options[index].text)
                {
                    _sceneLoader.SetSceneData(LD);
                    Debug.LogWarning($"Selected {LD.LevelName} for loading");
                    if (TryGetComponent(out HostCreator hostCreator))
                    {
                        hostCreator.SetNewLevelForLoading(LD);
                    }
                    return;
                }
            }
        }
    }
}
