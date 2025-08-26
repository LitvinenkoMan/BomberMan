using System;
using Core.ScriptableObjects;
using UnityEngine;

namespace Core.SaveSystem
{
    [Serializable]
    public class GameData
    {
        [SerializeField] private string _selectedCharacterDataID;
        [SerializeField] private string _playerNickname;
        [SerializeField] private int _winsAmount;
        [SerializeField] private int _battlesAmount;
        //TODO: may add some extra data related to player
        
        private CharacterData _selectedCharacterData;
        
        public CharacterData SelectedCharacterData => _selectedCharacterData;
        public string PlayerNickname => _playerNickname;
        public int WinsAmount => _winsAmount;
        public int BattlesAmount => _battlesAmount;
        
        
        
        public GameData(string playerNickname, int winsAmount, int battlesAmount, string selectedCharacterDataID)
        {
            _playerNickname = playerNickname;
            _winsAmount = winsAmount;
            _battlesAmount = battlesAmount;
            _selectedCharacterDataID = selectedCharacterDataID;
            _selectedCharacterData = Resources.Load<CharacterData>($"SOInstances/CharactersData/{selectedCharacterDataID}Character");      //TODO: this looks bad
        }

        public void LoadCharacterDataFromResources()
        {
            _selectedCharacterData = Resources.Load<CharacterData>($"SOInstances/CharactersData/{_selectedCharacterDataID}Character");
        }

        public void SetPlayerNickname(string playerNickname)
        {
            _playerNickname = playerNickname;
        }

        public void SetSelectedCharacterData(CharacterData characterData)
        {
            _selectedCharacterData = characterData;
            _selectedCharacterDataID = characterData.CharacterName;
        }

        public void SetBattlesAmount(int battlesAmount)
        {
            _battlesAmount = battlesAmount;
        }

        public void SetWinsAmount(int winsAmount)
        {
            _winsAmount = winsAmount;
        }

        // public static GameData operator (GameData gd1, GameData gd2)
        // {
        //     gd1._playerNickname ??= gd2._playerNickname;
        //     gd1._selectedCharacterData ??= gd2._selectedCharacterData;
        //     gd1._winsAmount = gd2._winsAmount;
        //     gd1._battlesAmount = gd2._battlesAmount;
        //     return gd1;
        // }
    }
}
