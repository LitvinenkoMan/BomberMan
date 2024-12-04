using System.Collections.Generic;
using Runtime.NetworkBehaviours;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    public class LevelSectionsDataHolder : MonoBehaviour
    {
        [Header("Main Values")]
        public List<GroundSection> sections;

        public List<GameObject> SpawnPlaces;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
        }

        void Start()
        {
            GroundSectionsUtils.Instance.SetNewDataHolder(this);
            PlayerSpawner.Instance.SetUpCurrentDataHolder(this);
        }

        void Update()
        {
        
        }
    }
}
