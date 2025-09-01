using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotCharacter : MonoBehaviour, ICharacter
{
    public IHealth Health { get; private set; }
    public IImmune Immune { get; private set; }
    public IBombDeployer BombDeployer { get; private set; }
    public IMovable CharacterMovement { get; private set; }
    public ICharacterAnimator CharacterAnimator { get; private set; }

    private NavMeshAgent _agent;

    public event Action<string> OnBotDeath;

    private void Awake()
    {
        CollectRefs();
    }

    private void OnEnable()
    {
        Health.OnHealthRunOut += BotDeath;
    }

    private void OnDisable()
    {
        Health.OnHealthRunOut -= BotDeath;
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

        if (Health.GetHealth() > 0)
        {
            Health.SubtractHealth(damageAmount);
            Immune.ActivateImmunity();
        }
    }

    public void DeployBomb()
    {
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
    }

    public void SetBombDeployAbility(bool canDeploy)
    {
    }

    public void SetMoveAbility(bool canMove)
    {
    }

    private void BotDeath()
    {
        OnBotDeath?.Invoke(gameObject.name);
        Debug.Log("Смерть бота");
    }


    private void CollectRefs()
    {
        if (TryGetComponent(out IImmune immune)) Immune = immune;
        if (TryGetComponent(out IBombDeployer bombDeployer)) BombDeployer = bombDeployer;
        if (TryGetComponent(out IMovable playerMovement)) CharacterMovement = playerMovement;
        if (TryGetComponent(out IHealth health)) Health = health;
        if (TryGetComponent(out ICharacterAnimator characterAnimator)) CharacterAnimator = characterAnimator;
        if (TryGetComponent(out NavMeshAgent agent)) _agent = agent;
    }
}
