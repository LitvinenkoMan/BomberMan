using UnityEngine;

namespace Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Character Data")]
    public class CharacterData : ScriptableObject, ICharacterData
    {
        [Header("Main Refs")]
        [SerializeField]
        private string _characterName;
        [SerializeField]
        private GameObject _visuals;
        [SerializeField]
        private GameObject _bomb;
        [SerializeField]
        private RuntimeAnimatorController  _animatorController;
        [SerializeField]
        private Avatar  _animatorAvatar;
        
        [Header("Values:")]
        [SerializeField]
        private int _characterHealth;
        [SerializeField]
        private float _speedMultiplier;
        [SerializeField]
        private float _bombsCountdown;
        [SerializeField]
        private int _bombsAtTime;
        [SerializeField]
        private int _bombsSpreading;
        [SerializeField]
        private int _bombsDamage;
        [SerializeField]
        private float _kickForce;

        public string CharacterName => _characterName;
        public GameObject Visuals => _visuals;
        public GameObject Bomb => _bomb;
        public RuntimeAnimatorController AnimatorController => _animatorController;
        public Avatar Avatar => _animatorAvatar;

        public int Health => _characterHealth;
        public float Speed => _speedMultiplier;
        public float BombCountdown => _bombsCountdown;
        public int BombsAtTime => _bombsAtTime;
        public int BombSpread => _bombsSpreading;
        public int BombDamage => _bombsDamage;
        public float KickForce => _kickForce;
    }

    public interface ICharacterData
    {
        public string CharacterName { get; }
        public GameObject Visuals { get; }
        public GameObject Bomb { get; }
        public RuntimeAnimatorController AnimatorController { get; }
        public Avatar Avatar { get; }

        public int Health { get; }
        public float Speed { get; }
        public float BombCountdown { get; }
        public int BombsAtTime { get; }
        public int BombSpread { get; }
        public int BombDamage { get; }
        public float KickForce { get; }
    }
}
