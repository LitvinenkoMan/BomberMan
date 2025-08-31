using Core.ScriptableObjects;
using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.MonoBehaviours.Player
{
    public class PlayerCharacter : MonoBehaviour, ICharacter, InputActions.IPlayerMapActions
    {
        [SerializeField]
        private BaseBomberParameters bomberParams;

        public IHealth Health { get; private set; }
        public IImmune Immune { get; private set; }
        public IBombDeployer BombDeployer { get; private set; }
        public IMovable CharacterMovement { get; private set; }
        public ICharacterAnimator CharacterAnimator { get; private set; }

        private InputActions _input;
        private CharacterController _characterController;

        public event Action OnPlayerDeath;

        private void Awake()
        {
            CollectRefs();
        }

        private void OnEnable()
        {
            Health.OnHealthRunOut += StartDeathSequence;
        }
        private void OnDisable()
        {
            Health.OnHealthRunOut -= StartDeathSequence;
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {            
            if (_input == null)
                _input = new InputActions();
            _input.PlayerMap.AddCallbacks(this);
            _input.Enable();
            //CharacterAnimator.Initialize();
            SetBombDeployAbility(true);
        }
        public void ActivateSpecial()
        {
        }

        public void Damage(int damageAmount)
        {
            if (Immune.IsImmune) return;

            if (Health.GetHealth() > 0)
            {
                Health.SubtractHealth(damageAmount);
                Immune.ActivateImmunity();
            }
        }

        public void DeployBomb()
        {
            BombDeployer.DeployBomb(bomberParams.BombsAtTime, bomberParams.BombsCountdown, bomberParams.BombsDamage, bomberParams.BombsSpreading);
        }

        public void Heal(int healAmount)
        {
        }
        public void Reset()
        {
            if (Health == null)
            {
                Debug.LogWarning("PlayerCharacter: have not PlayerHealth component");
                return;
            }
            Health.Initialize(3);
            _characterController.enabled = true;
        }

        public void SetBombDeployAbility(bool canDeploy)
        {
            BombDeployer.SetAbilityToDeployBombs(canDeploy);
        }

        public void SetMoveAbility(bool canMove)
        {
            CharacterMovement.SetAbilityToMove(canMove);
        }
        private void CollectRefs()
        {
            if (TryGetComponent(out IImmune immune)) Immune = immune;
            if (TryGetComponent(out IBombDeployer bombDeployer)) BombDeployer = bombDeployer;
            if (TryGetComponent(out IMovable playerMovement)) CharacterMovement = playerMovement;
            if (TryGetComponent(out IHealth health)) Health = health;
            if (TryGetComponent(out ICharacterAnimator characterAnimator)) CharacterAnimator = characterAnimator;
            if (TryGetComponent(out CharacterController characterController)) _characterController = characterController;
        }
        private void StartDeathSequence()
        {
            SetMoveAbility(false);
            SetBombDeployAbility(false);
            _input.PlayerMap.RemoveCallbacks(this);
            _input.Disable();

            _characterController.enabled = false;
            CharacterAnimator.PlayDeathAnimation();

            OnPlayerDeath?.Invoke();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 inputVector = context.ReadValue<Vector2>();
            Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);

            CharacterMovement.Move(moveDirection * bomberParams.SpeedMultiplier);
            if (inputVector != Vector2.zero)
            {
                CharacterAnimator.PlayWalkAnimation();
            }
            else
            {
                CharacterAnimator.PlayIdleAnimation();
            }
        }
        public void OnPlaceBomb(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                DeployBomb();
            }
        }
        public void OnQuit(InputAction.CallbackContext context)
        {
        }
    }
}

