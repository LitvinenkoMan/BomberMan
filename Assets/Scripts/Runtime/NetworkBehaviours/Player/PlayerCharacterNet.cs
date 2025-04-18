using System;
using Interfaces;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerCharacterNet : NetworkBehaviour, ICharacter
    {
        [SerializeField]
        private BaseBomberParameters bomberParams;
        public IHealth Health { get; private set; }
        public IImmune Immune { get; private set; }
        public IBombDeployer BombDeployer { get; private set; }

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
            throw new System.NotImplementedException();
        }

        private void StartDeathSequence()
        {
            
        }
    }
}
