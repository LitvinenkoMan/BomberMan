using Core.ScriptableObjects;
using Interfaces;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerCharacterNet : NetworkBehaviour, ICharacter, InputActions.IPlayerMapActions
    {
        [SerializeField]
        private BaseBomberParameters bomberParams;
        public IHealth Health { get; private set; }
        public IImmune Immune { get; private set; }
        public IBombDeployer BombDeployer { get; private set; }
        public IMovable CharacterMovement { get; private set; }

        private InputActions _input;

        public override void OnNetworkSpawn()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (TryGetComponent(out IImmune immune)) Immune = immune;
            if (TryGetComponent(out IBombDeployer bombDeployer)) BombDeployer = bombDeployer;
            if (TryGetComponent(out IMovable playerMovement)) CharacterMovement = playerMovement;
            if (TryGetComponent(out IHealth health)) Health = health;

            if (IsOwner)
            {
                _input ??= new InputActions();
                _input.PlayerMap.AddCallbacks(this);
                _input.Enable();
            }
        }

        public void Damage(int damageAmount)
        {
            if (Immune.IsImmune) return;

            Health.SubtractHealth(damageAmount);
            Immune.ActivateImmunity();
        }

        public void Heal(int healAmount)
        {
            Health.AddHealth(healAmount);
        }

        public void ActivateSpecial()
        {
            //TODO: Should to add specials
            //throw new System.NotImplementedException();
        }

        public void DeployBomb()
        {
            BombDeployer.DeployBomb(bomberParams.BombsAtTime, bomberParams.BombsCountdown, bomberParams.BombsDamage, bomberParams.BombsSpreading);
        }

        public void SetMoveAbility(bool canMove)
        {
            CharacterMovement.SetAbilityToMove(canMove);
        }

        public void SetBombDeployAbility(bool canDeploy)
        {
            BombDeployer.SetAbilityToDeployBombs(canDeploy);
        }

        private void StartDeathSequence()
        {
            SetMoveAbility(false);
            SetBombDeployAbility(false);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            var moveDirection = new Vector3(input.x, 0, input.y);
            
            CharacterMovement.Move(moveDirection * bomberParams.SpeedMultiplier);
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
            //throw new NotImplementedException();
        }
    }
}
