using System;
using System.Collections.Generic;
using Core.SaveSystem;
using Core.ScriptableObjects;
using UnityEngine;

namespace Runtime.MonoBehaviours.CharacterSelectionSystem
{
    public class PlayerCharacterSelector : MonoBehaviour
    {
        [Header("Selectable characters:")]
        [SerializeField] private List<CharacterData> AllCharacters = new List<CharacterData>();

        public void Initialize()
        {
            
        }

        public void SelectCharacter(CharacterData character)
        {
            SaveManager.Instance.PlayerData.SetSelectedCharacterData(character);
        }
    }
}
