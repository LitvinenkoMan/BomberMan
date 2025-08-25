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


        private CharacterData _characterData;
        
        public CharacterData CurrentCharacter => _characterData;

        private void Initialize()
        {
            //_characterData = SaveManager.Instance.PlayerData.SelectedCharacterData;
        }

        void Start()
        {
            Initialize();
            //SetCurrentCharacter(_characterData);
        }

        private void OnEnable()
        {
            //SetCurrentCharacter();
        }

        private void OnDisable()
        {
            
        }

        void Update()
        {
        
        }

        public void SetCurrentCharacter(CharacterData  characterData)
        {
            _characterData =  characterData;
            
            _characterName.text = characterData.Name;
            _characterLife.text = characterData.Health.ToString();
            _characterSpeed.text = characterData.Speed.ToString(CultureInfo.InvariantCulture);
            _characterDamage.text = characterData.BombDamage.ToString();
            _characterSpread.text = characterData.BombSpread.ToString();
            _characterBPT.text = characterData.BombsAtTime.ToString();
            _characterKickForce.text = characterData.KickForce.ToString();
            
            hanger.ChangeVisuals(characterData);

            // There should be shown CharactersVisuals
        }

        public void ConfirmSelection(CharacterData  characterData)
        {
            SaveManager.Instance.PlayerData.SetSelectedCharacterData(characterData);
        }
    }
}
