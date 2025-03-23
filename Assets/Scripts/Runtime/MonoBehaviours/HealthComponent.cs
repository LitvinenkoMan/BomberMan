using System;
using System.Collections;
using Core.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.MonoBehaviours
{
    public class HealthComponent : MonoBehaviour
    {
        public int HealthPoints { get => _nonBaseActorHealth; private set => _nonBaseActorHealth = value; }
        
        [SerializeField, Tooltip("If not set Component will use Starting Health instead, this field not necessary")]
        private ActorBaseParams baseParams;
        [SerializeField, Tooltip("If this GameObject should be Immune after being damaged, set this to true")]
        private bool ImmuneAfterGettingDamaged;
        [SerializeField]
        private float ImmunityTime;
        [SerializeField, Tooltip("Will be used if Base Params is not set")]
        private byte StartingHealth;

        private int _nonBaseActorHealth;
        private bool _isImmune;
        
        [Space(20)]
        // Events
        public Action<int> OnHealthChanged;
        public Action<float> OnGetImmune;
        public Action OnHealthRunOut;
        public UnityEvent OnHealthRunOutUnityEvent;
        

        private void Start()
        {
            if (baseParams)
            {
                _nonBaseActorHealth = baseParams.ActorHealth;
            }
        }

        private void OnEnable()
        {
            _isImmune = false;
            if (baseParams)
            {
                //baseParams.
            }
        }

        private void OnDisable()
        {
            _isImmune = false;
        }

        /// <summary>
        /// Will try to set new HP to GameObject, if newHealth will be less then previous one will give immune, if ImmuneAfterGettingDamaged is set to true.
        /// </summary>
        /// <param name="newHealth">New amount of health</param>
        public void SetHealth(int newHealth)
        {
            int deltaHealth = newHealth - _nonBaseActorHealth;
            if (deltaHealth < 0) // if new health is 2, and previous was 3, then 2-3=-1 - means owner was Damaged 
            {
                if (_isImmune)
                {
                    return;
                }

                if (ImmuneAfterGettingDamaged)
                {
                    StartCoroutine(Immunity());
                }
            }
            
            if (!baseParams)
            {
                _nonBaseActorHealth = newHealth;
                OnHealthChanged?.Invoke(_nonBaseActorHealth);
            }
            else
            {
                baseParams.SetActorHealth(newHealth);
                _nonBaseActorHealth = baseParams.ActorHealth;
                OnHealthChanged?.Invoke(_nonBaseActorHealth);
            }
            
            if (newHealth <= 0)
            {
                OnHealthRunOut?.Invoke();
                OnHealthRunOutUnityEvent?.Invoke();
            }
        }

        private IEnumerator Immunity()
        {
            _isImmune = true;
            OnGetImmune?.Invoke(ImmunityTime);
            // TODO: Add some kind of animation
            yield return new WaitForSeconds(ImmunityTime);
            _isImmune = false;
        }
    }
}
