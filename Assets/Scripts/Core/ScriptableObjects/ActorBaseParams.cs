using System;
using UnityEngine;

namespace Core.ScriptableObjects
{
    public class ActorBaseParams : ScriptableObject
    {
        [SerializeField]
        protected int _actorHealth;
        
        
        public Action<int> OnHealthChangedEvent;
        
        public int ActorHealth
        {
            get => _actorHealth;
            protected set => _actorHealth = value;
        }

        public void SetActorHealth(int newValue)
        {
            ActorHealth = newValue;
            OnHealthChangedEvent?.Invoke(ActorHealth);            
        }
    }
}
