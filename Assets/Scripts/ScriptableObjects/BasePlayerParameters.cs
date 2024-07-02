using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class BasePlayerParameters : ScriptableObject
    {
        public byte PlayerHealth { get; private set; }
        public byte SpeedMultiplier { get; private set; }
        public byte BombsAtTime { get; private set; }
        
        public void ResetValues()
        {
            PlayerHealth = 3;
            SpeedMultiplier = 5;
            BombsAtTime = 1;
        }

        public void SetPlayerHealth(byte newValue) => PlayerHealth = newValue;
        public void SetSpeedMultiplier(byte newValue) => SpeedMultiplier = newValue;
        public void SetBombsAtTime(byte newValue) => BombsAtTime = newValue;

    }
}
