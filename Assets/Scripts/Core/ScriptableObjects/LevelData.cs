using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.ScriptableObjects
{
    [CreateAssetMenu]
    public class LevelData : SceneDataScriptableObject
    {
        [SerializeField]
        protected byte _maxAmountOfPlayers;
        
        public byte MaxAmountOfPlayers {
            get => _maxAmountOfPlayers;
            private set => _maxAmountOfPlayers = value;
        }
        
        [SerializeField]
        protected string _levelName;
        
        public string LevelName {
            get => _levelName;
            private set => _levelName = value;
        }
    }
}
