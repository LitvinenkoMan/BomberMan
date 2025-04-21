using System;
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

        private InputActions _input;

        public override void OnNetworkSpawn()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (TryGetComponent(out IImmune immune)) Immune = immune;
            if (TryGetComponent(out IBombDeployer bombDeployer)) BombDeployer = bombDeployer;
            if (TryGetComponent(out IHealth health)) 
            {
                Health = health;
            }

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
            //throw new System.NotImplementedException();
        }

        public void DeployBomb()
        {
            BombDeployer.DeployBomb(bomberParams.BombsAtTime, bomberParams.BombsCountdown, bomberParams.BombsDamage, bomberParams.BombsSpreading);
        }

        public void SetMoveAbility()
        {
            
        }

        private void StartDeathSequence()
        {
            
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            //throw new NotImplementedException();
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
