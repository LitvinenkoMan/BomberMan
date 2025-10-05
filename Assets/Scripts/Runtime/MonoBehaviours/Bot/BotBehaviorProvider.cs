using Interfaces;
using AbstractClasses;
using Runtime.MonoBehaviours.Bot.SimpleBotUtils;
using Runtime.MonoBehaviours.Bot.StandartBotUtils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot
{
    public class BotBehaviorProvider
    {
        private Dictionary<BotType, Dictionary<string, IState>> _states;
        private Dictionary<BotType, BaseTargetOpponentSelector> _targetSelectors; 
        private Dictionary<BotType, BaseBotNavigation> _botNavigations;
        private Dictionary<BotType, BaseShelterFinder> _shelterFinder;

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

            _targetSelectors = new Dictionary<BotType, BaseTargetOpponentSelector> 
            {
                {BotType.Easy, new SimpleTargetOpponentSelector(_agent) },
                {BotType.Standart, new StandartTargetOpponentSelector(_agent) }
            };

            _botNavigations = new Dictionary<BotType, BaseBotNavigation>
            {
                {BotType.Easy, new SimpleBotNavigation(_agent) },
                {BotType.Standart, new StandartBotNavigation(_agent) }
            };
            _shelterFinder = new Dictionary<BotType, BaseShelterFinder>
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
                return targetSelector;
            }
            else
            {
                Debug.LogError($"No TargetOpponentSelector found for BotType: {type}");
                return null;
            }
        }
        public BaseBotNavigation GetBotNavigationForType(BotType type)
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
        public BaseShelterFinder GetShelterFinder(BotType type)
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