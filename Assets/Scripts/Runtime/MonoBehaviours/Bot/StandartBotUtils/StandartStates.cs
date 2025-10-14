using System.Collections;
using UnityEngine;
using Interfaces;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot.StandartBotUtils
{
    public class StandartAgro : IState
    {
        private Coroutine _pathFindCoroutine;
        private Coroutine _stayInTargetCoroutine;
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
            if (_stayInTargetCoroutine != null) return;
            if (_pathFindCoroutine == null)
            {
                _pathFindCoroutine = manager.StartCoroutine(PathFind(manager));
            }
            float distance = manager.BotNavigation.CheckDistance(manager.TargetOpponentFinder.GetCurrentOpponent());


            if (distance <= 0.6f)
            {
                _stayInTargetCoroutine = manager.StartCoroutine(StayInTarget(manager.BotNavigation.StayInTarget));
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
        private IEnumerator StayInTarget(float waitingTime)
        {
            yield return new WaitForSeconds(waitingTime);
            _stayInTargetCoroutine = null;
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
}
