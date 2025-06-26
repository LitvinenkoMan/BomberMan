using System;
using System.Collections.Generic;
using Core.ScriptableObjects;
using MonoBehaviours.Network;
using Runtime.NetworkBehaviours;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.MonoBehaviours
{
    public class LevelConfigurator : MonoBehaviour
    {
        [SerializeField]
        private GeneralGameData GameData;
        [SerializeField]
        private TMP_Dropdown LevelsDropdown;
        [SerializeField]
        private TMP_Dropdown MatchTypesDropdown;
        
        [Header("Loaders for LVL and Match Type")]
        [SerializeField]
        private SceneLoader LevelSceneLoader;  
        [SerializeField]
        private NetworkSceneLoader GameplaySceneLoader;
        
        [Space(20)]
        [Header("List of Levels and MatchManagers")]
        [SerializeField]
        private List<LevelData> LevelsDataList;
        
        [FormerlySerializedAs("MatchTypesList")] [SerializeField]
        private List<SceneDataScriptableObject> MatchTypeScenesList;

        
        void Start()
        {
            UpdateLevelsList();
            UpdateMatchTypesList();
            OnSelectedLevelChanged(0);
            OnSelectedTypeChanged(0);
        }

        private void OnEnable()
        {
            LevelsDropdown.onValueChanged.AddListener(OnSelectedLevelChanged);
            MatchTypesDropdown.onValueChanged.AddListener(OnSelectedTypeChanged);
        }

        private void OnDisable()
        {
            LevelsDropdown.onValueChanged.RemoveListener(OnSelectedLevelChanged);
            MatchTypesDropdown.onValueChanged.RemoveListener(OnSelectedTypeChanged);
        }

        private void UpdateLevelsList()
        {
            LevelsDropdown.ClearOptions();

            List<string> levelsNames = new List<string>();

            foreach (var LD in LevelsDataList)
            {
                levelsNames.Add(LD.LevelName);
            }

            LevelsDropdown.AddOptions(levelsNames);
        }

        private void UpdateMatchTypesList()
        {
            MatchTypesDropdown.ClearOptions();

            List<string> typesNames = new List<string>();

            foreach (var MT in MatchTypeScenesList)
            {
                typesNames.Add(MT.SceneName);
            }     
            
            MatchTypesDropdown.AddOptions(typesNames);
        }

        private void OnSelectedLevelChanged(int index)
        {
            foreach (var LD in LevelsDataList)
            {
                if (LD.LevelName == LevelsDropdown.options[index].text)
                {
                    GameData.SetLastSelectedLevelData(LD);
                    LevelSceneLoader.SetSceneData(LD);
                    Debug.LogWarning($"Selected {LD.LevelName} LVL for loading");
                    return;
                }
            }
        }

        private void OnSelectedTypeChanged(int index)
        {
            foreach (var MatchType in MatchTypeScenesList)
            {
                if (MatchType.SceneName == MatchTypesDropdown.options[index].text)
                {
                    GameData.SetLastSelectedMatchType(MatchType);
                    GameplaySceneLoader.SetSceneData(MatchType);
                    Debug.LogWarning($"Selected {MatchType.SceneName} MM for loading");
                    return;
                }
            }
        }
    }
}
