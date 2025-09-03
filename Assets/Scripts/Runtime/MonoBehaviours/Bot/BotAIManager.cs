using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot
{
    public class BotAIManager : MonoBehaviour
    {
        [SerializeField] private Transform p;
        private StateSwitcher _stateSwitcher;
        private BotCharacter _character;
        public StateSwitcher StateSwitcher => _stateSwitcher;
        public BotCharacter Character => _character;

        private void Awake()
        {
            CollectRefs();
            StateSwitcher stateSwitcher = new EasyStateSwitcher();
            SetStateSwitcher(stateSwitcher);
        }

        private void CollectRefs()
        {
            if (TryGetComponent(out BotCharacter character)) _character = character;
        }

        public void SetStateSwitcher(StateSwitcher stateSwitcher)
        {
            _stateSwitcher = stateSwitcher;
            _stateSwitcher.Construct(GetComponent<NavMeshAgent>(), Spawner.Instance.Player.transform, gameObject.transform);
            _stateSwitcher.SetTarget(Spawner.Instance.Player.transform);
        }

        private void Update()
        {
            _stateSwitcher.SwitchState(this);
            _stateSwitcher.SetTarget(Spawner.Instance.Player.transform);
            _stateSwitcher.CurrentState.Update(this);
        }
    }
}


