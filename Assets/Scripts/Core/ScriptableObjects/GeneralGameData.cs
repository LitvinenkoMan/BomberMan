using Runtime.NetworkBehaviours;
using ScriptableObjects;
using Unity.Collections;
using UnityEngine;

namespace Core.ScriptableObjects
{
    
    [CreateAssetMenu]
    public class GeneralGameData : ScriptableObject
    {
        [ReadOnly]
        private LevelData _lastSelectedLevel;
        [ReadOnly]
        private SceneDataScriptableObject _lastSelectedMatchType;

        
        public LevelData LastSelectedLevelData => _lastSelectedLevel;
        public SceneDataScriptableObject LastSelectedMatchType => _lastSelectedMatchType;
        

        public void SetLastSelectedLevelData(LevelData data)
        {
            _lastSelectedLevel = data;
        }
        
        public void SetLastSelectedMatchType(SceneDataScriptableObject matchType)
        {
            _lastSelectedMatchType = matchType;
        }
    }
}
