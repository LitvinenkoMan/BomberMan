using Core.ScriptableObjects;
using Interfaces;
using Runtime.MonoBehaviours.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Runtime.MonoBehaviours.Bot
{
    public class BotCharacter : MonoBehaviour, ICharacter
    {
        [SerializeField] private CharacterData _characterData;
        [SerializeField] private PlayerBombDeployer _concreteBombDeployer;
        public ICharacterRuntimeData CharacterRuntimeData { get; private set; }
        public IImmune Immune { get; private set; }
        public IBombDeployer BombDeployer { get; private set; }
        public IMovable CharacterMovement { get; private set; }
        public ICharacterAnimator CharacterAnimator { get; private set; }

        private CharacterRuntimeData _characterRuntimeData;
        public CharacterRuntimeData CharacterData => _characterRuntimeData;
        public PlayerBombDeployer PlayerBombDeployer => _concreteBombDeployer;

        public event Action<string> OnBotDeath;

        private void Awake()
        {
            CollectRefs();
        }

        private void OnEnable()
        {
            CharacterRuntimeData.OnHealthRunOut += BotDeath;
        }

        private void OnDisable()
        {
            CharacterRuntimeData.OnHealthRunOut -= BotDeath;
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

            if (CharacterRuntimeData.CharacterHealth > 0)
            {
                CharacterRuntimeData.SubtractHealth(damageAmount);
                Immune.ActivateImmunity();
            }
        }

        public void DeployBomb()
        {
            BombDeployer.DeployBomb(CharacterRuntimeData.BombsAtTime, CharacterRuntimeData.BombsCountdown, CharacterRuntimeData.BombsDamage, CharacterRuntimeData.BombsSpreading);
        }

        public void Heal(int healAmount)
        {
        }
        public void Reset()
        {
            if (CharacterRuntimeData == null)
            {
                Debug.LogWarning("PlayerCharacter: have not CharacterRuntimeData");
                return;
            }
            CharacterRuntimeData.Initialize(3);
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

            BombDeployer = _concreteBombDeployer;

            _characterRuntimeData = new CharacterRuntimeData();

            _characterRuntimeData.Initialize(_characterData);
            CharacterRuntimeData = _characterRuntimeData;
        }
    }
}

