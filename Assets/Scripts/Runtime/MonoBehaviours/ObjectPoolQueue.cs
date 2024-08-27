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
        [SerializeField, Tooltip("If true, will reparent Objects on Get and Add to pool")]
        private bool UseReparenting;
        
        
        private Queue<GameObject> _pool;


        private void Awake()
        {
            _pool = new Queue<GameObject>(StartingCount);
            if (StartingCount > 0)
            {
                for (int i = 0; i < StartingCount; i++)
                {
                    AddToPool(InstantiateNewMember(false));
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
            _pool.Enqueue(newMember);
            if (UseReparenting)
            {
                newMember.transform.SetParent(transform);
            }
            newMember.SetActive(false);
        }
        
        private GameObject InstantiateNewMember(bool activeFromStart)
        {
            GameObject newObject;
            if (UseReparenting)
            {
                newObject = Instantiate(ObjectExample, Vector3.zero, new Quaternion(0, 0, 0, 0), transform);
            }
            else
            {
                newObject = Instantiate(ObjectExample, Vector3.zero, new Quaternion(0, 0, 0, 0));
            }
            newObject.SetActive(activeFromStart);
            return newObject;
        }
    }
}
