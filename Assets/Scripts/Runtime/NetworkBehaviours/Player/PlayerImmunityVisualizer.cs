using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerImmunityVisualizer : MonoBehaviour, IImmuneVisualizer
    {
        public event Action<float> OnImmuneVisualized;
        
        private UniTask<float> VisualizeImmunityTask;
        
        public async void VisualizeImmunity(float time)
        {
            try
            {
                VisualizeImmunityTask = ImmunityVisualization(time);
                OnImmuneVisualized?.Invoke(time);
                await VisualizeImmunityTask;
            }
            catch (Exception e)
            {
                throw e.GetBaseException();
            }
        }
        
        void Start()
        {
            VisualizeImmunityTask = ImmunityVisualization(0);
        }


        private async UniTask<float> ImmunityVisualization(float time)
        {
            
            
            
            await UniTask.WaitForSeconds(time);
            return time;
        }
    }
}
