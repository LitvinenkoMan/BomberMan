using Core.ScriptableObjects;
using Interfaces;
using System;
using UnityEngine;
using UnityEngine.AI;
using Core.DataTransferObjects;

namespace Runtime.MonoBehaviours.Bot
{
    public class BotCharacter : MonoBehaviour, ICharacter
    {
        [SerializeField] private CharacterData _characterData;
        public ICharacterRuntimeData CharacterRuntimeData { get; private set; }
        public IImmune Immune { get; private set; }
        public IBombDeployer BombDeployer { get; private set; }
        public IMovable CharacterMovement { get; private set; }
        public ICharacterAnimator CharacterAnimator { get; private set; }

        private BombDto _bombDto;
        private CharacterRuntimeData _characterRuntimeData;

        public BombDto BombDto => _bombDto;
        public CharacterRuntimeData CharacterData => _characterRuntimeData;

        public event Action<string> OnBotDeath;
        public event Action<BombDto> OnBombDeployed;

        private void Awake()
        {
            CollectRefs();
        }

        public void Initialize()
        {
        }
        public void ActivateSpecial()
        {
        }

        public void Damage(int damageAmount)
        {
            if (Immune.IsImmune) return;

            if (_characterRuntimeData.CharacterHealth > 0)
            {
                _characterRuntimeData.SubtractHealth(damageAmount);
                Immune.ActivateImmunity();
            }
            else
            {
                Immune.ActivateImmunity();
                BotDeath();
            }
        }

        public void DeployBomb()
        {
            BombDeployer.DeployBomb(_bombDto);
            OnBombDeployed.Invoke(_bombDto);
        }

        public void Heal(int healAmount)
        {
        }
        public void Reset()
        {
            if (_characterRuntimeData == null)
            {
                Debug.LogWarning("BotCharacter: have not CharacterRuntimeData");
                return;
            }
            _characterRuntimeData.Initialize(_characterData);
        }

        public void SetBombDeployAbility(bool canDeploy)
        {
            BombDeployer.SetAbilityToDeployBombs(canDeploy);
        }

        public void SetMoveAbility(bool canMove)
        {
            gameObject.GetComponent<NavMeshAgent>().speed = canMove ? 3 : 0;
        }

        private void BotDeath()
        {
            OnBotDeath?.Invoke(gameObject.name);
            CharacterAnimator.PlayDeathAnimation();
            SetBombDeployAbility(false);
            SetMoveAbility(false);
        }
        private void CollectRefs()
        {
            if (TryGetComponent(out IImmune immune)) Immune = immune;
            if (TryGetComponent(out IMovable playerMovement)) CharacterMovement = playerMovement;
            if (TryGetComponent(out ICharacterAnimator characterAnimator)) CharacterAnimator = characterAnimator;
            if (TryGetComponent(out IBombDeployer bombDeployer)) BombDeployer = bombDeployer;

            _characterRuntimeData = new CharacterRuntimeData();
            _characterRuntimeData.Initialize(_characterData);

            _bombDto = new BombDto(_characterRuntimeData.BombsCountdown, _characterRuntimeData.BombsAtTime, _characterRuntimeData.BombsSpreading, _characterRuntimeData.BombsDamage);
            
            CharacterRuntimeData = _characterRuntimeData;
        }
    }
}

