using Interfaces;
using Runtime.MonoBehaviours;
using Runtime.MonoBehaviours.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraModeController : MonoBehaviour
{
    [SerializeField] private GameObject CameraExample;

    private GameObject _player;
    private ICameraViewer _cameraViewer;
    private GameObject _instantiatedCamera;

    private void OnEnable()
    {
        PlayerSpawner.Instance.OnPlayerSpawned += FollowSpawnedPlayer;
        CheckForCameraInstance();
    }
    private void OnDisable()
    {
        PlayerSpawner.Instance.OnPlayerSpawned -= FollowSpawnedPlayer;
    }

    private void CheckForCameraInstance()
    {
        if (_instantiatedCamera == null)
        {
            _instantiatedCamera = Instantiate(CameraExample, Vector3.zero, Quaternion.identity);
            _cameraViewer = _instantiatedCamera.GetComponent<ICameraViewer>();
        }
    }
    private void CheckForInstancedPlayer()
    {
        if (_player == null)
        {
            _player = FindAnyObjectByType<PlayerCharacter>().gameObject;
        }
        else
        {
            FollowSpawnedPlayer();
        }
    }

    private void SwitchToGameplayMode()
    {
        _cameraViewer.ClearTargetsList();
        _cameraViewer.AddToViewTarget(_player.transform);
    }

    public void FollowSpawnedPlayer()
    {
        CheckForInstancedPlayer();
        SwitchToGameplayMode();
    }
}
