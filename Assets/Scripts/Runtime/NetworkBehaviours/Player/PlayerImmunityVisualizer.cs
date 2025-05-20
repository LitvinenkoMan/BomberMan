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
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
