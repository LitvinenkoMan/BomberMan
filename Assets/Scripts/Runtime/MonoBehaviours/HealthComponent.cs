using System;
using System.Collections;
using Core.ScriptableObjects;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField, Tooltip("If not set Component will use Starting Health instead, this field not necessary")]
        private ActorBaseParams baseParams;
        [SerializeField, Tooltip("If this GameObject should be Immune after being damaged, set this to true")]
        private bool ImmuneAfterGettingDamaged;
        [SerializeField]
        private float ImmunityTime;
        [SerializeField, Tooltip("Will be used if Base Params is not set")]
        private byte StartingHealth;
        
        public int HealthPoints { get; private set; }

        private bool _isImmune;
        
        
        // Events
        public Action<int> OnHealthChanged;
        public Action OnHealthRunOut;
        

        private void Start()
        {
            if (baseParams)
            {
                HealthPoints = baseParams.ActorHealth;
            }
            else
            {
                HealthPoints = StartingHealth;
            }
        }

        private void OnEnable()
        {
            _isImmune = false;
        }

        private void OnDisable()
        {
            
        }

        /// <summary>
        /// Will try to set new HP to GameObject, if newHealth will be less then previous one will give immune, if ImmuneAfterGettingDamaged is set to true.
        /// </summary>
        /// <param name="newHealth">New amount of health</param>
        public void SetHealth(int newHealth)
        {
            int deltaHealth = newHealth - HealthPoints;
            if (deltaHealth < 0)        // if new health is 2, and previous was 3, then 2-3=-1 - means owner was Damaged 
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
            HealthPoints = newHealth;
            OnHealthChanged?.Invoke(HealthPoints);
            if (HealthPoints <= 0)
            {
                OnHealthRunOut?.Invoke();
            }
        }

        private IEnumerator Immunity()
        {
            _isImmune = true;
            // TODO: Add some kind of animation
            yield return new WaitForSeconds(ImmunityTime);
            _isImmune = false;
        }
    }
}
