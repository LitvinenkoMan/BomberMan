using Interfaces;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot.StandartBotUtils
{
    public class StandartAgro : IState
    {
        private Coroutine _pathFindCoroutine;
        public void Enter(BotLogicExecuter manager)
        {
            manager.Character.CharacterAnimator.PlayWalkAnimation();
            manager.TargetOpponentFinder.SelectTargetOpponent();
            manager.BotNavigation.SetSpeed(3f);
        }

        public void Exit(BotLogicExecuter manager)
        {
            if (_pathFindCoroutine != null)
            {
                manager.StopCoroutine(_pathFindCoroutine);
                _pathFindCoroutine = null;
            }
        }

        public void Update(BotLogicExecuter manager)
        {
            manager.TargetOpponentFinder.SelectTargetOpponent();
            for (int i = 0; i <= 50;  i++)
            {
                manager.BotNavigation.CheckPathToTarget(manager.TargetOpponentFinder.GetCurrentOpponent());
            }
            
            if (_pathFindCoroutine == null)
            {
                //_pathFindCoroutine = manager.StartCoroutine(PathFind(manager));
            }
            float distance = manager.BotNavigation.CheckDistance(manager.TargetOpponentFinder.GetCurrentOpponent());
                        
            if (distance <= 0.2f)
            {
                if (manager.BotNavigation.StayInTarget != 0)
                {
                    manager.SwitchState(manager.States["Stay In Place"]);
                    return;
                }
                
                manager.SwitchState(manager.States["Deploy Bomb"]);
            }
        }
        private IEnumerator PathFind(BotLogicExecuter manager)
        {
            manager.TargetOpponentFinder.SelectTargetOpponent();
            manager.BotNavigation.CheckPathToTarget(manager.TargetOpponentFinder.GetCurrentOpponent());

            yield return new WaitForSeconds(0.2f);

            manager.StopCoroutine(_pathFindCoroutine);
            _pathFindCoroutine = null;           
        }
    }
    public class StayInPlace : IState
    {
        float stateTime = 0f;
        public void Enter(BotLogicExecuter manager)
        {
            manager.Character.CharacterAnimator.PlayIdleAnimation();
            stateTime = manager.BotNavigation.StayInTarget;
        }

        public void Exit(BotLogicExecuter manager)
        {
            manager.Character.CharacterAnimator.PlayWalkAnimation();
        }

        public void Update(BotLogicExecuter manager)
        {
            if (stateTime > 0f)
            {
                stateTime -= Time.deltaTime;
                return;
            }
            else
            {
                manager.SwitchState(manager.States["Agro"]);
                manager.BotNavigation.ResetStayInTarget();
            }
        }
    }


    public class StandartDeployBomb : IState
    {
        float timer = 0f;
        public void Enter(BotLogicExecuter manager)
        {
            manager.Character.CharacterAnimator.PlayWalkAnimation();
            manager.Character.DeployBomb();
        }

        public void Exit(BotLogicExecuter manager)
        {

        }

        public void Update(BotLogicExecuter manager)
        {
            manager.ShelterFinder.RetreatFromBomb(manager);

            float distance = manager.BotNavigation.CheckDistance(manager.TargetOpponentFinder.GetCurrentOpponent());

            if (distance <= 0.1f)
            {
                manager.Character.CharacterAnimator.PlayIdleAnimation();
            }

            timer += Time.deltaTime;

            if (timer >= manager.CharacterData.BombsCountdown)
            {
                manager.SwitchState(manager.States["Agro"]);
                timer = 0f;
            }
        }
    }
    public class TakePowerUp : IState 
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

}
