using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class BaseBomberParameters : ActorBaseParams
    {
        [SerializeField]
        public int _startPlayerHealth = 3;
        [SerializeField]
        public int _startSpeedMultiplier = 5;
        [SerializeField]
        public int _startBombsAtTime = 1;
        [SerializeField]
        public int _startBombsSpreading = 1;
        [SerializeField]
        public int _startBombsCountdown = 3;
        [SerializeField]
        public int _startBombsDamage = 1;
        
        public int SpeedMultiplier { get; private set; }
        public int BombsAtTime { get; private set; }
        public int BombsSpreading { get; private set; }
        public int BombsCountdown { get; private set; }
        public int BombsDamage { get; private set; }
        
        public void ResetValues()
        {
            ActorHealth = _startPlayerHealth;
            SpeedMultiplier = _startSpeedMultiplier;
            BombsAtTime = _startBombsAtTime;
            BombsSpreading = _startBombsSpreading;
            BombsCountdown = _startBombsCountdown;
            BombsDamage = _startBombsDamage;
        }

        public void SetActorHealth(byte newValue) => ActorHealth = newValue;
        public void SetSpeedMultiplier(byte newValue) => SpeedMultiplier = newValue;
        public void SetBombsAtTime(byte newValue) => BombsAtTime = newValue;
        public void SetBombsSpreading(byte newValue) => BombsSpreading = newValue;
        public void SetBombsCountdown(byte newValue) => BombsCountdown = newValue;
        public void SetBombsDamage(byte newValue) => BombsDamage = newValue;

    }
}
