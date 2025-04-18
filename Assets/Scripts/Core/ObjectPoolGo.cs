using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Core
{
    public class ObjectPoolGo : MonoBehaviour, IObjectPool<GameObject>
    {
        [SerializeField]
        public GameObject objectExample;

        [SerializeField]
        private int startingCount;
        [SerializeField, Tooltip("If true, will reparent Objects on Get and Add to pool")]
        private bool useReparenting;
        
        private Queue<GameObject> _queue;

        public void Initialize()
        {
            _queue = new Queue<GameObject>(startingCount);
            if (startingCount > 0)
            {
                for (int i = 0; i < startingCount; i++)
                {
                    AddToPool(InstantiateNewMember(true));
                }
            }
        }

        /// <summary>
        /// Will add new item to the pool and make it disabled, also will change parent if "Use Reparenting" is true 
        /// </summary>
        /// <param name="item"></param>
        public void AddToPool(GameObject item)
        {
            _queue.Enqueue(item);
            if (useReparenting)
            {
                item.transform.SetParent(transform);
            }
            item.SetActive(false);
        }

        public GameObject GetFromPool(bool makeActive)
        {
            if (_queue.Count > 0)
            {
                var newObject = _queue.Dequeue();
                newObject.SetActive(makeActive);
                return newObject;
            }
            
            return InstantiateNewMember(makeActive);
        }

        public void Clear()
        {
            while (_queue.Count > 0)
            {
                Destroy(_queue.Peek());
            }
        }

        private GameObject InstantiateNewMember(bool activeFromStart)
        {
            var newObject = useReparenting
                ? Instantiate(objectExample, Vector3.zero, new Quaternion(0, 0, 0, 0), transform)
                : Instantiate(objectExample, Vector3.zero, new Quaternion(0, 0, 0, 0));
            newObject.SetActive(activeFromStart);
            return newObject;
        }
    }
}
