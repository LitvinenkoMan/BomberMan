using Unity.Netcode;
using UnityEngine;

namespace Core.ScriptableObjects
{
    public class ActorBaseParams : ScriptableObject
    {
        [SerializeField]
        public int _actorHealth;
        public int ActorHealth
        {
            get => _actorHealth;
            protected set => _actorHealth = value;
        }
    }
}
