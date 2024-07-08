using System.Collections.Generic;
using UnityEngine;

namespace MonoBehaviours
{
    public class ObjectPoolQueue : MonoBehaviour
    {
        [SerializeField]
        public GameObject ObjectExample;

        [SerializeField]
        private int StartingCount;
        
        
        private Queue<GameObject> _pool;


        private void Awake()
        {
            _pool = new Queue<GameObject>(StartingCount);
            if (StartingCount > 0)
            {
                for (int i = 0; i < StartingCount; i++)
                {
                    InstantiateNewMember(false);
                }
            }
        }

        public GameObject GetFromPool(bool isEnabled)
        {
            if (_pool.Count > 0)
            {
                GameObject newObject = _pool.Dequeue();
                newObject.SetActive(isEnabled);
                return newObject;
            }
            
            return InstantiateNewMember(isEnabled);
        }

        public void AddToPool(GameObject newMember)
        {
            newMember.SetActive(false);
            _pool.Enqueue(newMember);
        }
        
        private GameObject InstantiateNewMember(bool activeFromStart)
        {
            GameObject newObject = Instantiate(ObjectExample, Vector3.zero, new Quaternion(0, 0, 0, 0), transform);
            newObject.SetActive(activeFromStart);
            return newObject;
        }
    }
}
