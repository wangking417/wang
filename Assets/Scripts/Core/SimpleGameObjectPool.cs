using System.Collections.Generic;
using UnityEngine;

namespace FirepowerFullBlast.Core
{
    public class SimpleGameObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int preloadCount = 8;

        private readonly Queue<GameObject> _pool = new();

        private void Awake()
        {
            for (int i = 0; i < preloadCount; i++)
            {
                ReturnToPool(CreateInstance());
            }
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject instance = _pool.Count > 0 ? _pool.Dequeue() : CreateInstance();
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
            return instance;
        }

        public void ReturnToPool(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            instance.SetActive(false);
            instance.transform.SetParent(transform);
            _pool.Enqueue(instance);
        }

        private GameObject CreateInstance()
        {
            GameObject instance = Instantiate(prefab, transform);
            instance.SetActive(false);
            return instance;
        }
    }
}

