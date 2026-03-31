using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirepowerFullBlast.AI
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyAIController enemyPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int maxAliveEnemies = 5;
        [SerializeField] private float spawnInterval = 4f;

        private readonly List<EnemyAIController> _aliveEnemies = new();

        private void Start()
        {
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                CleanupDeadEntries();

                if (_aliveEnemies.Count < maxAliveEnemies && enemyPrefab != null && spawnPoints.Length > 0)
                {
                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    EnemyAIController enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                    _aliveEnemies.Add(enemy);
                }

                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void CleanupDeadEntries()
        {
            _aliveEnemies.RemoveAll(enemy => enemy == null);
        }
    }
}

