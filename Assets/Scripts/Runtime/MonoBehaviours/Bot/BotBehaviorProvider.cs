using Interfaces;
using Runtime.MonoBehaviours.Bot;
using Runtime.MonoBehaviours.Bot.SimpleBotUtils;
using Runtime.MonoBehaviours.Bot.StandartBotUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot
{
    public class BotBehaviorProvider
    {
        private Dictionary<BotType, Dictionary<string, IState>> _states;
        // Since we need to return new instances of BaseTargetOpponentSelector, delegates are used.
        private Dictionary<BotType, Func<BaseTargetOpponentSelector>> _targetSelectors; 
        private Dictionary<BotType, IBotNavigation> _botNavigations;
        private Dictionary<BotType, IShelterFinder> _shelterFinder;

        public void InitializeBehaviors(NavMeshAgent _agent)
        {
            _states = new Dictionary<BotType, Dictionary<string, IState>>
            {
                {
                    BotType.Easy,
                    new Dictionary<string, IState>()
                    {
                        { "Agro", new SimpleAgro() },
                        { "Deploy Bomb", new SimpleDeployBomb() }
                    }
                },
                {
                    BotType.Standart,
                    new Dictionary<string, IState>()
                    {
                        { "Agro", new StandartAgro() },
                        { "Deploy Bomb", new StandartDeployBomb() }
                    }
                }
            };

            _targetSelectors = new Dictionary<BotType, Func<BaseTargetOpponentSelector>> 
            {
                {BotType.Easy, () => new SimpleTargetOpponentSelector(_agent) },
                {BotType.Standart, () => new StandartTargetOpponentSelector(_agent) }
            };

            _botNavigations = new Dictionary<BotType, IBotNavigation>
            {
                {BotType.Easy, new SimpleBotNavigation(_agent) },
                {BotType.Standart, new StandartBotNavigation(_agent) }
            };
            _shelterFinder = new Dictionary<BotType, IShelterFinder>
            {
                { BotType.Easy, new SimpleShelterFinder(_agent) },
                { BotType.Standart, new StandartShelterFinder(_agent) }
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
        public BaseTargetOpponentSelector GetTargetSelectorForType(BotType type) 
        { 
            if (_targetSelectors.TryGetValue(type, out var targetSelector))
            {
                return targetSelector();
            }
            else
            {
                Debug.LogError($"No TargetOpponentSelector found for BotType: {type}");
                return null;
            }
        }
        public IBotNavigation GetBotNavigationForType(BotType type)
        {
            if ( _botNavigations.TryGetValue(type,out var botNavigation))
            {
                return botNavigation;
            }
            else
            {
                Debug.LogError($"No BotNavigation found for BotType: {type}");
                return null;
            }
        }
        public IShelterFinder GetShelterFinder(BotType type)
        {
            if (_shelterFinder.TryGetValue(type, out var shelterFinder))
            {
                return shelterFinder;
            }
            else
            {
                Debug.LogError($"No ShelterFinder found for BotType: {type}");
                return null;
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