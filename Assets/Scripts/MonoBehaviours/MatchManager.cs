using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MonoBehaviours
{
    public class MatchManager : MonoBehaviour
    {
        


        public UnityEvent StartMatch;

        [SerializeField]
        private 
        void Start()
        {
        
        }

        void Update()
        {
        
        }

        public void SpawnPlayer()
        {
            PlayerSpawner.Instance.SpawnPlayerRpc(); // as client
        }

        private IEnumerator SpawnPlayerHost()
        {
            yield return new WaitForSeconds(1);
            PlayerSpawner.Instance.SpawnPlayerRpc(); // as host
        }
    }
}
