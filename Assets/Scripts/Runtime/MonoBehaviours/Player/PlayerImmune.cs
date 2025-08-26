using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.MonoBehaviours.Player
{
    public class PlayerImmune : MonoBehaviour, IImmune
    {
        [SerializeField]
        private float startImmunityTime;
        public bool IsImmune { get; private set; }
        private IEnumerator _immunityCoroutine;
        private float _immunityTime;
        private IImmuneVisualizer _immuneVisualizer;

        public event Action<float> OnGetImmune;

        void Start()
        {
            Initialize();
        }
        public void Initialize()
        {
            IsImmune = false;
            _immunityCoroutine = Immunity();
            _immunityTime = startImmunityTime;
            _immuneVisualizer = GetComponent<IImmuneVisualizer>();
        }
        public void ActivateImmunity()
        {
            IsImmune = true;
            StartCoroutine(_immunityCoroutine);
            OnGetImmune?.Invoke(_immunityTime);
        }
        public void SetNewImmunityTime(float newTime)
        {
            _immunityTime = newTime;
        }
        private IEnumerator Immunity()
        {
            _immuneVisualizer?.VisualizeImmunity(_immunityTime);
            yield return new WaitForSeconds(_immunityTime);
            IsImmune = false;
            _immunityCoroutine = Immunity();
            yield return null;
        }
    }
}
