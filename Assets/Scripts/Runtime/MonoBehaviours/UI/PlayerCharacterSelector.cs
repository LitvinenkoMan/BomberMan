using System;
using System.Globalization;
using Core.SaveSystem;
using Core.ScriptableObjects;
using Runtime.MonoBehaviours.CharacterSelectionSystem;
using TMPro;
using UnityEngine;

namespace Runtime.MonoBehaviours.UI
{
    public class PlayerCharacterSelector : MonoBehaviour
    {
        [SerializeField] private CharacterVisualsHanger hanger;
        
        [Space]
        [SerializeField] private TMP_Text _characterName;
        [SerializeField] private TMP_Text _characterLife;
        [SerializeField] private TMP_Text _characterSpeed;
        [SerializeField] private TMP_Text _characterDamage;
        [SerializeField] private TMP_Text _characterSpread;
        [SerializeField] private TMP_Text _characterBPT;
        [SerializeField] private TMP_Text _characterKickForce;
        
        private CharacterData _currentCharacterData;
        
        public CharacterData CurrentCharacter => _currentCharacterData;

        private void Initialize()
        {
            if (SaveManager.Instance.PlayerData != null)
            {
                _currentCharacterData = SaveManager.Instance.PlayerData.SelectedCharacterData;
            }
        }

        void Start()
        {
            Initialize();
            SetCurrentCharacter(SaveManager.Instance.PlayerData.SelectedCharacterData);
        }
        
        public void SetCurrentCharacter(CharacterData  characterData)
        {
            _currentCharacterData =  characterData;
            
            _characterName.text = characterData.CharacterName;
            _characterLife.text = characterData.Health.ToString();
            _characterSpeed.text = characterData.Speed.ToString(CultureInfo.InvariantCulture);
            _characterDamage.text = characterData.BombDamage.ToString();
            _characterSpread.text = characterData.BombSpread.ToString();
            _characterBPT.text = characterData.BombsAtTime.ToString();
            _characterKickForce.text = characterData.KickForce.ToString();
            
            hanger.ChangeVisuals(characterData);

            // There should be shown CharactersVisuals
        }

        public void ConfirmSelection()
        {
            SaveManager.Instance.PlayerData.SetSelectedCharacterData(_currentCharacterData);
        }

        public void CancelSelection()
        {
            SetCurrentCharacter(SaveManager.Instance.PlayerData.SelectedCharacterData);
        }
    }
}
