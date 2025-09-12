using System;
using Core.ScriptableObjects;

namespace Interfaces
{
    public interface ICharacterRuntimeData
    {
        public int CharacterHealth { get; }
        public float SpeedMultiplier { get; }
        public float BombsCountdown { get; }
        public int BombsAtTime { get; }
        public int BombsSpreading { get; }
        public int BombsDamage { get; }
        public float KickForce { get; }


        public void AddHealth(int healthToAdd);
        public void SubtractHealth(int healthToSubtract);
        public void SetSpeedMultiplier(float speedMultiplier);
        public void SetBombsCountdown(float bombsCountdown);
        public void SetBombsAtTime(int bombsAtTime);
        public void SetBombsSpreading(int bombsSpreading);
        public void SetBombsDamage(int bombsDamage);
        public void SetKickForce(float kickForce);

    }
}
