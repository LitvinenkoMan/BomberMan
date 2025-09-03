using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using System;
using Core.ScriptableObjects;

public class CharacterRuntimeData : ICharacterRuntimeData
{
    public int CharacterHealth { get; private set; }

    public float SpeedMultiplier { get; private set; }

    public float BombsCountdown { get; private set; }

    public int BombsAtTime { get; private set; }

    public int BombsSpreading { get; private set; }

    public int BombsDamage { get; private set; }

    public float KickForce { get; private set; }

    public event Action OnHealthRunOut;

    public void AddHealth(int healthToAdd)
    {
        CharacterHealth += healthToAdd;
    }

    public void Initialize(float initialValue)
    {
        
    }

    public void Initialize(CharacterData characterData)
    {
        CharacterHealth = characterData.Health;
        SpeedMultiplier = characterData.Speed;
        BombsCountdown = characterData.BombCountdown;
        BombsAtTime = characterData.BombsAtTime;
        BombsSpreading = characterData.BombSpread;
        BombsDamage = characterData.BombDamage;
        KickForce = characterData.KickForce;
    }

    public void SetBombsAtTime(int bombsAtTime)
    {
        
    }

    public void SetBombsCountdown(float bombsCountdown)
    {
        
    }

    public void SetBombsDamage(int bombsDamage)
    {
        
    }

    public void SetBombsSpreading(int bombsSpreading)
    {
        
    }

    public void SetKickForce(float kickForce)
    {
        
    }

    public void SetSpeedMultiplier(float speedMultiplier)
    {
        
    }

    public void SubtractHealth(int healthToSubtract)
    {
        CharacterHealth -= healthToSubtract;

        if (CharacterHealth <= 0)
        {
            OnHealthRunOut?.Invoke();
        }
    }
}
