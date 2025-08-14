using System;
using Interfaces;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerImmunityVisualizer : MonoBehaviour, IImmuneVisualizer
    {
        public event Action<float> OnImmuneVisualized;
        
        public void VisualizeImmunity(float time)
        {
           
        }
    }
}
