using Runtime.MonoBehaviours;
using Runtime.NetworkBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrainingMatchManager : MonoBehaviour
{
    [SerializeField] GameObject StartButton;

    public UnityEvent StartMatchUnityEvent;
    public UnityEvent EndMatchUnityEvent;

    public Action OnInitialized;

    private void Start()
    {
        SpawnPlayer();
    }
    public void SpawnPlayer()
    {
        PlayerSpawner.Instance.RandomSpawnPlayer();   // Need to redo PlayerSpawner
    }
}
