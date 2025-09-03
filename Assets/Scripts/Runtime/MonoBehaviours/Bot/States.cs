using Runtime.MonoBehaviours.Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

namespace Interfaces 
{
    public interface IState
    {
        void Enter(BotAIManager manager);
        void Update(BotAIManager manager);
        void Exit(BotAIManager manager);
    }
}


namespace Runtime.MonoBehaviours.Bot
{
    public class Agro : IState
    {
        public void Enter(BotAIManager manager)
        {
            manager.StateSwitcher.SetSpeed(2.7f);
            manager.Character.CharacterAnimator.PlayWalkAnimation();
        }

        public void Exit(BotAIManager manager)
        {
            manager.Character.CharacterAnimator.PlayIdleAnimation();
        }

        public void Update(BotAIManager manager)
        {

        }
    }

    public class SearchShelter : IState
    {
        public void Enter(BotAIManager manager)
        {
        }

        public void Exit(BotAIManager manager)
        {
        }

        public void Update(BotAIManager manager)
        {
        }
    }

    public class DeployBomb : IState
    {
        public void Enter(BotAIManager manager)
        {
            manager.StateSwitcher.SetSpeed(0f);
            manager.Character.DeployBomb();
        }

        public void Exit(BotAIManager manager)
        {
        }

        public void Update(BotAIManager manager)
        {
        }
    }
}
