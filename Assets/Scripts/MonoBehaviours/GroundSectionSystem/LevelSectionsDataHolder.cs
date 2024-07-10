using System.Collections.Generic;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    public class LevelSectionsDataHolder : MonoBehaviour
    {
        public List<GroundSection> sections;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
        }

        void Start()
        {
            GroundSectionsUtils.Instance.SetNewDataHolder(this);
        }

        void Update()
        {
        
        }
    }
}
