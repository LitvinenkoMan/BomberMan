using Core.ScriptableObjects;
using UnityEngine;

namespace Core.SaveSystem
{
    public class GameData
    {
        private CharacterData _selectedCharacterData;
        private string _playerNickname;
        private int _winsAmount;
        private int _battlesAmount;
        //TODO: may add some extra data related to player
        
        public CharacterData SelectedCharacterData => _selectedCharacterData;
        public string PlayerNickname => _playerNickname;
        public int WinsAmount => _winsAmount;
        public int BattlesAmount => _battlesAmount;
        
        public GameData(string playerNickname, int winsAmount, int battlesAmount, CharacterData selectedCharacterData)
        {
            _playerNickname = playerNickname;
            _winsAmount = winsAmount;
            _battlesAmount = battlesAmount;
            _selectedCharacterData = selectedCharacterData;
        }

        public void UpdateValues(GameData data)
        {
            _playerNickname ??= data._playerNickname;
            _selectedCharacterData ??= data._selectedCharacterData;
            _winsAmount = data._winsAmount;
            _battlesAmount = data._battlesAmount;
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
