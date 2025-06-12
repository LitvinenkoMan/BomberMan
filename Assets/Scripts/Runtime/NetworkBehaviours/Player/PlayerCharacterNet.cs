using System;
using Core.ScriptableObjects;
using Interfaces;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerCharacterNet : NetworkBehaviour, ICharacter, InputActions.IPlayerMapActions
    {
        [SerializeField]
        private BaseBomberParameters bomberParams;
        [SerializeField]
        private TMP_Text playerName;
        [SerializeField]
        private GameObject playerVisuals;
        
        public IHealth Health { get; private set; }
        public IImmune Immune { get; private set; }
        public IBombDeployer BombDeployer { get; private set; }
        public IMovable CharacterMovement { get; private set; }

        private InputActions _input;

        public event Action<ulong> OnPlayerDeath;

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

        public override void OnNetworkSpawn()
        {
            Initialize();
            name = $"P{GetComponent<NetworkObject>().OwnerClientId}";
            playerName.text = name;
        }

        public override void OnNetworkDespawn()
        {
            
        }

        public void Initialize()
        {
            if (IsOwner)
            {
                _input ??= new InputActions();
                _input.PlayerMap.AddCallbacks(this);
                _input.Enable();
                //bomberParams.ResetValues();
                playerVisuals.SetActive(true);
                playerName.enabled = true;
            }
        }

        public void Damage(int damageAmount)
        {
            if (Immune.IsImmune) return;

            Health.SubtractHealth(damageAmount);
            if (Health.GetHealth() > 0)
            {
                Immune.ActivateImmunity();
            }
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

        public void Reset()
        {
            ResetPlayerRpc(RpcTarget.Single(NetworkObject.OwnerClientId, RpcTargetUse.Temp));
        }

        private void StartDeathSequence()
        {
            if (IsOwner)
            { 
                playerVisuals.SetActive(false);
                SetMoveAbility(false);
                SetBombDeployAbility(false);      
                playerName.enabled = false;
            }
            gameObject.SetActive(false);

            OnPlayerDeath?.Invoke(OwnerClientId);
            //UnspawnPlayerRpc();
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

        private void CollectRefs()
        {
            if (TryGetComponent(out IImmune immune)) Immune = immune;
            if (TryGetComponent(out IBombDeployer bombDeployer)) BombDeployer = bombDeployer;
            if (TryGetComponent(out IMovable playerMovement)) CharacterMovement = playerMovement;
            if (TryGetComponent(out IHealth health)) Health = health;
        }
        
        [Rpc(SendTo.Server)]
		private void UnspawnPlayerRpc()
		{
            NetworkObject.Despawn(true);
		}

        [Rpc(SendTo.SpecifiedInParams)]
        private void ResetPlayerRpc(RpcParams rpcParams)
        {
            Health.Initialize(3);
        }
    }
}
