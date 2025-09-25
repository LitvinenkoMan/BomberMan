using System;
using Core.DataTransferObjects;
using Core.EventBuses;
using Core.SaveSystem;
using Core.ScriptableObjects;
using CSharp;
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
        private CharacterData characterData;
        [SerializeField]
        private TMP_Text playerName;
        [SerializeField]
        private GameObject playerVisuals;   
        
        public ICharacterRuntimeData CharacterRuntimeData { get; private set; }
        public IImmune Immune { get; private set; }
        public IBombDeployer BombDeployer { get; private set; }
        public IMovable CharacterMovement { get; private set; }
        public ICharacterAnimator CharacterAnimator { get; private set; }

        private InputActions _input;
        private CharacterController _characterController;
        private PlayerCharacterRuntimeNet _playerCharacterRuntimeNet;

        public event Action<ulong> OnPlayerDeath;

        private void Awake()
        {
            CollectRefs();
        }

        private void OnEnable()
        {
            //CharacterRuntimeData.OnHealthRunOut += StartDeathSequence;
        }

        private void OnDisable()
        {
            //CharacterRuntimeData.OnHealthRunOut -= StartDeathSequence;
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
                playerVisuals.SetActive(true);
                playerName.enabled = true;
            }
            //CharacterRuntimeData.
            CharacterAnimator.Initialize();
        }

        public void Damage(int damageAmount)
        {
            if (Immune.IsImmune) return;

            if (CharacterRuntimeData.CharacterHealth > 0)
            { 
                CharacterRuntimeData.SubtractHealth(damageAmount);
                Immune.ActivateImmunity();
            }
            else
            {
                Immune.ActivateImmunity();
                StartDeathSequence();
            }
        }

        public void Heal(int healAmount)
        {
            CharacterRuntimeData.AddHealth(healAmount);
        }

        public void ActivateSpecial()
        {
            //TODO: Should to add specials
        }

        public void DeployBomb()
        {
            BombDeployer.DeployBomb(new BombDto(CharacterRuntimeData.BombsCountdown, CharacterRuntimeData.BombsAtTime, CharacterRuntimeData.BombsSpreading, CharacterRuntimeData.BombsDamage));
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
                SetMoveAbility(false);
                SetBombDeployAbility(false);      
                _input.PlayerMap.RemoveCallbacks(this);
                _input.Disable();
            }
            _characterController.enabled = false;
            CharacterAnimator.PlayDeathAnimation();

            GameplayUIEvents.Instance.RiseOnHealthRunOutEvent(NetworkManager.Singleton.LocalClientId, _playerCharacterRuntimeNet.CharacterHealth);
            OnPlayerDeath?.Invoke(OwnerClientId);
            //UnspawnPlayerRpc();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            var moveDirection = new Vector3(input.x, 0, input.y);
            
            CharacterMovement.Move(moveDirection * CharacterRuntimeData.SpeedMultiplier);
            if (input != Vector2.zero)
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
            //throw new NotImplementedException();
        }

        private void CollectRefs()
        {
            characterData = SaveManager.Instance.PlayerData.SelectedCharacterData;
            
            if (TryGetComponent(out IImmune immune)) Immune = immune;
            if (TryGetComponent(out IBombDeployer bombDeployer)) BombDeployer = bombDeployer;
            if (TryGetComponent(out IMovable playerMovement)) CharacterMovement = playerMovement;
            if (TryGetComponent(out ICharacterAnimator characterAnimator)) CharacterAnimator = characterAnimator;
            if (TryGetComponent(out CharacterController characterController)) _characterController = characterController;

            if (TryGetComponent(out ICharacterRuntimeData characterRuntimeData))
            {
                CharacterRuntimeData = characterRuntimeData;
                _playerCharacterRuntimeNet = characterRuntimeData as PlayerCharacterRuntimeNet;
            }
        }
        
        [Rpc(SendTo.Server)]
		private void UnspawnPlayerRpc()
		{
            NetworkObject.Despawn(true);
		}

        [Rpc(SendTo.SpecifiedInParams)]
        private void ResetPlayerRpc(RpcParams rpcParams)
        {
            _playerCharacterRuntimeNet.Initialize(characterData);
            _characterController.enabled = true;
        }
    }
}
