using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityFigmaBridge.Editor.Fonts;

namespace Runtime.MonoBehaviours.Bot
{
    public class StatesSelector
    {
        private Dictionary<BotType, Dictionary<string, IState>> _states;

        public StatesSelector()
        {
            _states = new Dictionary<BotType, Dictionary<string, IState>>
            {
                {
                    BotType.Easy,
                    new Dictionary<string, IState>()
                    {
                        { "Agro", new Agro() },
                        { "Search Shelter", new SearchShelter() },
                        { "Deploy Bomb", new DeployBomb() }
                    }
                }
            };
        }
        public Dictionary<string, IState> GetStatesForType(BotType type)
        {
            if (_states.TryGetValue(type, out var stateArray))
            {
                return stateArray;
            }
            else
            {
                Debug.LogError($"No states found for BotType: {type}");
                return null;
            }
        }
    }

    public abstract class StateSwitcher
    {
        protected NavMeshAgent _agent;
        protected Transform _player;
        protected Transform _target;
        protected Transform _thisBot;
        protected Dictionary<string, IState> _states;
        protected IState _currentState;
        public IState CurrentState => _currentState;
        public void Construct(NavMeshAgent agent, Transform player, Transform thisBot)
        {
            _agent = agent;
            _player = player;
            _thisBot = thisBot;
        }
        public abstract void SwitchState(BotAIManager zombieStateManager);
        public abstract void SetSpeed(float speed);
        public abstract void SetTarget(Transform target);
        public abstract float CheckDistance();
    }

    public class EasyStateSwitcher : StateSwitcher
    {
        public EasyStateSwitcher()
        {
            StatesSelector statesSelector = new StatesSelector();
            _states = statesSelector.GetStatesForType(BotType.Easy);
        }
        public override float CheckDistance()
        {
            if (_thisBot == null)
            {
                Debug.Log("thisBot = null");
                return 0f;
            }
            if (_target == null)
            {
                Debug.Log("target = null");
                return 0f;
            }
            return (_thisBot.position - _target.position).magnitude;
        }

        public override void SetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        public override void SetTarget(Transform target)
        {
            _target = target;
            _agent.destination = target.position;
        }

        public override void SwitchState(BotAIManager manager)
        {            
            float distance = CheckDistance();
            IState newState = (distance >= 2) ? _states["Agro"] : _states["Deploy Bomb"];
            if (newState != _currentState)
            {
                if (_currentState != null)
                {
                    _currentState.Exit(manager);
                }                
                _currentState = newState;
                _currentState.Enter(manager);
            }
        }
    }
}


public enum BotType
{
    Easy,
    Standart,
    Hard
}