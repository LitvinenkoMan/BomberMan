using System;
using UnityEngine;

namespace Core.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        [Header("Name of the save folder")]
        [SerializeField]
        private string SaveProfileName;
        
        public static SaveManager Instance;
        public GameData PlayerData => _data;
         
        private GameData _data;
        private FileDataReader _fileDataReader;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else Destroy(this);
        }

        void Start()
        {
            _fileDataReader = new FileDataReader(Application.dataPath, SaveProfileName);
            Debug.Log(Application.dataPath);
            LoadGame();
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        public void AddDataForSaving(GameData data)
        {
            _data.UpdateValues(data);
        } 

        public void SaveGame()
        {
            _fileDataReader.Save(_data, SaveProfileName);
        }

        public void LoadGame()
        {
            _data = _fileDataReader.Load(SaveProfileName);
        }
    }
}
