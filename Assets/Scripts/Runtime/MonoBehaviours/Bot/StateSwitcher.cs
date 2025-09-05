using Interfaces;
using Runtime.MonoBehaviours.Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
using UnityFigmaBridge.Editor.Fonts;

namespace Interfaces 
{
    public abstract class StateSwitcher
    {
        private Dictionary<string, IState> _states;
        public Dictionary<string, IState> States => _states;
        public abstract void LogicUpdate(BotLogicExecuter zombieStateManager);
        public abstract void SetSpeed(float speed);
        public abstract void SetTarget(Transform target);
        public abstract float CheckDistance();
        public abstract void CheckPath();
        public abstract void SwitchState(IState newState);
        public abstract void SetStateChangeAllowed(bool allowed);
    }
}

namespace Runtime.MonoBehaviours.Bot
{
    public class StateSwitchersDto
    {
        private NavMeshAgent _agent;
        private Transform _player;
        private Transform _thisBot;
        private Dictionary<string, IState> _states;
        private ICharacterRuntimeData _characterData;
        private BotLogicExecuter _botlogicExecuter;

        public NavMeshAgent Agent => _agent;
        public Transform Player => _player;
        public Transform ThisBot => _thisBot;
        public Dictionary<string, IState> States => _states;
        public ICharacterRuntimeData CharacterData => _characterData;
        public BotLogicExecuter BotLogicExecuter => _botlogicExecuter;

        public StateSwitchersDto(
            NavMeshAgent agent, 
            Transform player, 
            Transform thisBot, 
            Dictionary<string, IState> states, 
            ICharacterRuntimeData characterData, 
            BotLogicExecuter botLogicExecuter)
        {
            _agent = agent;
            _player = player;
            _thisBot = thisBot;
            _states = states;
            _characterData = characterData;
            _botlogicExecuter = botLogicExecuter;
        }
    }
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
                        { "Agro", new SimpleAgro() },
                        { "Search Shelter", new SimpleSearchShelter() },
                        { "Deploy Bomb", new SimpleDeployBomb() }
                    }
                },
                {
                    BotType.Standart,
                    new Dictionary<string, IState>()
                    {
                        { "Agro", new SimpleAgro() },
                        { "Search Shelter", new SimpleSearchShelter() },
                        { "Deploy Bomb", new SimpleDeployBomb() }
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

    
    public class EasyStateSwitcher : StateSwitcher
    {
        private BotLogicExecuter _botlogicExecuter;
        private NavMeshAgent _agent;
        private Transform _player;
        private Transform _target;
        private Transform _thisBot;
        private Dictionary<string, IState> _states;
        private ICharacterRuntimeData _characterData;
        private IState _currentState;
        private bool _stateChangeAllowed = true;

        
        public IState CurrentState => _currentState;

        public EasyStateSwitcher(StateSwitchersDto dto)
        {
            _thisBot = dto.ThisBot;
            _agent = dto.Agent;
            _player = dto.Player;
            _states = dto.States;
            _characterData = dto.CharacterData;
            _botlogicExecuter = dto.BotLogicExecuter;
            SwitchState(_states["Agro"]);
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

        public override void CheckPath()
        {
            NavMeshPath path = new NavMeshPath();
            _agent.CalculatePath(_target.position, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                SetTarget(_target);
            }
            else
            {
                
            }
        }
        public override void SwitchState(IState newState)
        {
            if (!_stateChangeAllowed) return;

            if (newState != _currentState)
            {
                if (_currentState != null)
                {
                    _currentState.Exit(_botlogicExecuter);
                }
                _currentState = newState;
                _currentState.Enter(_botlogicExecuter);
            }
        }

        public override void LogicUpdate(BotLogicExecuter manager)
        {
            _currentState.Update(manager);
            
        }

        public override void SetStateChangeAllowed(bool allowed)
        {
            _stateChangeAllowed = allowed;
        }
    }
}


public enum BotType
{
    Easy,
    Standart,
    Hard
}