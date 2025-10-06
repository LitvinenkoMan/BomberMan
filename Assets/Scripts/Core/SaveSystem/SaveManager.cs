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
            _fileDataReader = new FileDataReader(Application.persistentDataPath, SaveProfileName);
            Debug.LogError(Application.persistentDataPath);
            _data = new GameData("", 0, 0, "");
            LoadGame();
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        public void SaveGame()
        {
            _fileDataReader.Save(_data, SaveProfileName);
        }

        public void LoadGame()
        {
            _data = _fileDataReader.Load(SaveProfileName);
            if (_data != null)
            {
                _data.LoadCharacterDataFromResources();
            }
            else
            {
                Debug.LogError("Couldn't load character data, creating new one");
                _data = new GameData("PlayerNone", 0, 0, "Golem");
            }
        }
    }
}
