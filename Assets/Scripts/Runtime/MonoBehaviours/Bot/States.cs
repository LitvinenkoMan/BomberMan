using Runtime.MonoBehaviours.Bot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

namespace Interfaces
{
    public interface IState
    {
        void Enter(BotLogicExecuter manager);
        void Update(BotLogicExecuter manager);
        void Exit(BotLogicExecuter manager);
    }
}


namespace Runtime.MonoBehaviours.Bot
{
    public class SimpleAgro : IState
    {
        float timer = 0f;
        public void Enter(BotLogicExecuter manager)
        {
            manager.SelectTargetPlayer();
            manager.Character.CharacterAnimator.PlayWalkAnimation();
            
            manager.SetTarget(manager.TargetOpponent.position);
        }

        public void Exit(BotLogicExecuter manager)
        {
            manager.Character.CharacterAnimator.PlayIdleAnimation();
        }

        public void Update(BotLogicExecuter manager)
        {
            float distance = manager.CheckDistance();

            timer += Time.deltaTime;
            
            if (timer >= 0.5f)
            {
                manager.SelectTargetPlayer();
                manager.SetTarget(manager.TargetOpponent.position);
                timer = 0f;
            }
            manager.CheckPath();
            if (distance <= 0.5f)
            {
                manager.SwitchState(manager.States["Deploy Bomb"]);
            }
        }
    }
    public class SimpleSearchShelter : IState
    {
        public void Enter(BotLogicExecuter manager)
        {
            
        }

        public void Exit(BotLogicExecuter manager)
        {

        }

        public void Update(BotLogicExecuter manager)
        {
            
        }
    }
    public class SimpleDeployBomb : IState
    {
        float timer = 0f;
        public void Enter(BotLogicExecuter manager)
        {
            manager.Character.CharacterAnimator.PlayWalkAnimation();
            manager.Character.DeployBomb();
            manager.RetreatFromBomb();
        }

        public void Exit(BotLogicExecuter manager)
        {

        }

        public void Update(BotLogicExecuter manager)
        {
            float distance = manager.CheckDistance();

            if (distance <= 0.1f)
            {
                manager.Character.CharacterAnimator.PlayIdleAnimation();
            }

            timer += Time.deltaTime;

            if (timer >= 3f)
            {
                manager.SwitchState(manager.States["Agro"]);
                timer = 0f;
            }
        }
    }
}
