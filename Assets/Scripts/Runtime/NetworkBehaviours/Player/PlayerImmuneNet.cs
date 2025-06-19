using System;
using System.Collections;
using Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerImmuneNet : NetworkBehaviour, IImmune
    {
        [SerializeField]
        private float startImmunityTime;
        public bool IsImmune { get; private set; }
        
        public event Action<float> OnGetImmune;

        private IEnumerator _immunityCoroutine;
        private float _immunityTime;

        public override void OnNetworkSpawn()
        {
            Initialize();
        }

        public void Initialize()
        {
            IsImmune = false;
            _immunityCoroutine = Immunity();
            _immunityTime = startImmunityTime;
        }

        public void ActivateImmunity()
        {
            IsImmune = true;
            OnGetImmuneRpc(_immunityTime);
        }

        [Rpc(SendTo.Everyone)]
        private void OnGetImmuneRpc(float time)
        {
            StartCoroutine(_immunityCoroutine);
            OnGetImmune?.Invoke(time);
        }

        public void SetNewImmunityTime(float newTime)
        {
            _immunityTime = newTime;
        }

        private IEnumerator Immunity()
        {
            yield return new WaitForSeconds(_immunityTime);
            IsImmune = false;
            _immunityCoroutine = Immunity();
            yield return null;
        }
    }
}
