using System;
using Core.ScriptableObjects;
using Interfaces;

namespace CSharp
{
    public class PlayerCharacterRuntimeNet : ICharacterRuntimeData
    {
        private int _characterHealth;
        private float _speedMultiplier;
        private float _bombsCountdown;
        private int _bombsAtTime;
        private int _bombsSpreading;
        private int _bombsDamage;
        private float _kickForce;

        public int CharacterHealth => _characterHealth;

        public float SpeedMultiplier => _speedMultiplier;

        public float BombsCountdown => _bombsCountdown;

        public int BombsAtTime => _bombsAtTime;

        public int BombsSpreading => _bombsSpreading;

        public int BombsDamage => _bombsDamage;

        public float KickForce => _kickForce;

        public event Action OnHealthRunOut;
        
        
        public void Initialize(CharacterData  characterData)
        {
            _characterHealth = characterData.Health;
            _speedMultiplier = characterData.Speed;
            _bombsCountdown = characterData.BombCountdown;
            _bombsAtTime = characterData.BombsAtTime;
            _bombsSpreading = characterData.BombSpread;
            _bombsDamage = characterData.BombDamage;
            _kickForce = characterData.KickForce;
        }

        public void AddHealth(int healthToAdd)
        {
            _characterHealth += _characterHealth;
        }

        public void SubtractHealth(int healthToSubtract)
        {
            _characterHealth -= healthToSubtract;
        }

        public void SetSpeedMultiplier(float speedMultiplier)
        {
            _speedMultiplier = speedMultiplier;
        }

        public void SetBombsCountdown(float bombsCountdown)
        {
            _bombsCountdown = bombsCountdown;
        }

        public void SetBombsAtTime(int bombsAtTime)
        {
            _bombsAtTime = bombsAtTime;
        }

        public void SetBombsSpreading(int bombsSpreading)
        {
            _bombsSpreading = bombsSpreading;
        }

        public void SetBombsDamage(int bombsDamage)
        {
            _bombsDamage = bombsDamage;
        }

        public void SetKickForce(float kickForce)
        {
            _kickForce = kickForce;
        }
        
        

    }
}
