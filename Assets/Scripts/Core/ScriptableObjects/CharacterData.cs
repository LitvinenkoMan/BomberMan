using UnityEngine;

namespace Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Character Data")]
    public class CharacterData : ScriptableObject
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

        public string Name => _characterName;
        public GameObject Visuals => _visuals;
        public GameObject Bomb => _bomb;
        public RuntimeAnimatorController AnimatorController => _animatorController;

        public int Health => _characterHealth;
        public float Speed => _speedMultiplier;
        public float BombCountdown => _bombsCountdown;
        public int BombsAtTime => _bombsAtTime;
        public int BombSpread => _bombsSpreading;
        public int BombDamage => _bombsDamage;
        public float KickForce => _kickForce;
    }
}
