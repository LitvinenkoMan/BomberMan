using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class BaseBomberParameters : ActorBaseParams
    {
        [SerializeField]
        public byte _startPlayerHealth = 3;
        [SerializeField]
        public byte _startSpeedMultiplier = 5;
        [SerializeField]
        public byte _startBombsAtTime = 1;
        [SerializeField]
        public byte _startBombsSpreading = 1;
        [SerializeField]
        public byte _startBombsCountdown = 3;
        [SerializeField]
        public byte _startBombsDamage = 1;
        
        public byte SpeedMultiplier { get; private set; }
        public byte BombsAtTime { get; private set; }
        public byte BombsSpreading { get; private set; }
        public byte BombsCountdown { get; private set; }
        public byte BombsDamage { get; private set; }
        
        public void ResetValues()
        {
            ActorHealth = _startPlayerHealth;
            SpeedMultiplier = _startSpeedMultiplier;
            BombsAtTime = _startBombsAtTime;
            BombsSpreading = _startBombsSpreading;
            BombsCountdown = _startBombsCountdown;
            BombsDamage = _startBombsDamage;
        }

        public void SetPlayerHealth(byte newValue) => ActorHealth = newValue;
        public void SetSpeedMultiplier(byte newValue) => SpeedMultiplier = newValue;
        public void SetBombsAtTime(byte newValue) => BombsAtTime = newValue;
        public void SetBombsSpreading(byte newValue) => BombsSpreading = newValue;
        public void SetBombsCountdown(byte newValue) => BombsCountdown = newValue;
        public void SetBombsDamage(byte newValue) => BombsDamage = newValue;

    }
}
