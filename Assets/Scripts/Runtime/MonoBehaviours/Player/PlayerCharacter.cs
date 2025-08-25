using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.MonoBehaviours.Player
{
    public class PlayerCharacter : MonoBehaviour, ICharacter, InputActions.IPlayerMapActions
    {
        public IHealth Health { get; private set; }
        public IImmune Immune { get; private set; }
        public IBombDeployer BombDeployer { get; private set; }
        public IMovable CharacterMovement { get; private set; }
        public ICharacterAnimator CharacterAnimator { get; private set; }

        private CharacterController _characterController;

        public void Initialize()
        {
        }
        public void ActivateSpecial()
        {
        }

        public void Damage(int damageAmount)
        {
        }

        public void DeployBomb()
        {
        }

        public void Heal(int healAmount)
        {
        }
        public void Reset()
        {

        }

        public void SetBombDeployAbility(bool canDeploy)
        {
        }

        public void SetMoveAbility(bool canMove)
        {
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
        public void OnMove(InputAction.CallbackContext context)
        {
        }
        public void OnPlaceBomb(InputAction.CallbackContext context)
        {
        }
        public void OnQuit(InputAction.CallbackContext context)
        {
        }
    }
}

