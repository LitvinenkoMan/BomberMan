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

        private IEnumerator _immunity;
        private float _immunityTime;
        public void Initialize()
        {
            IsImmune = false;
            _immunity = Immunity();
            _immunityTime = startImmunityTime;
        }

        public void ActivateImmunity()
        {
            IsImmune = true;
            StartCoroutine(_immunity);
            OnGetImmuneRpc(_immunityTime);
        }

        [Rpc(SendTo.Everyone)]
        private void OnGetImmuneRpc(float time)
        {
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
            _immunity = Immunity();
            yield return null;
        }
    }
}
