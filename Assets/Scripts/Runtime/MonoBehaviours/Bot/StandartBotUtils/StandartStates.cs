using System.Collections;
using UnityEngine;
using Interfaces;

namespace Runtime.MonoBehaviours.Bot.StandartBotUtils
{
    public class StandartAgro : IState
    {
        private Coroutine _coroutine;
        public void Enter(BotLogicExecuter manager)
        {
            manager.Character.CharacterAnimator.PlayWalkAnimation();
            manager.TargetOpponentFinder.SelectTargetOpponent();
            manager.BotNavigation.SetSpeed(3f);
        }

        public void Exit(BotLogicExecuter manager)
        {
            if (_coroutine != null)
            {
                manager.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        public void Update(BotLogicExecuter manager)
        {
            if (_coroutine == null)
            {
                _coroutine = manager.StartCoroutine(corr(manager));
            }
            float distance = manager.BotNavigation.CheckDistance(manager.TargetOpponentFinder.GetCurrentOpponent());

            if (distance <= 0.8f)
            {
                //Debug.Log("Switch to Deploy Bomb");
                manager.SwitchState(manager.States["Deploy Bomb"]);
            }
        }
        private IEnumerator corr(BotLogicExecuter manager)
        {
            manager.TargetOpponentFinder.SelectTargetOpponent();
            manager.BotNavigation.CheckPathToTarget(manager.TargetOpponentFinder.GetCurrentOpponent());

            yield return new WaitForSeconds(0.5f);

            manager.StopCoroutine(_coroutine);
            _coroutine = null;      
            
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
