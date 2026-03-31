using System.Collections;
using FirepowerFullBlast.Combat;
using FirepowerFullBlast.Core;
using UnityEngine;
using UnityEngine.AI;

namespace FirepowerFullBlast.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    public class EnemyAIController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float detectRadius = 18f;
        [SerializeField] private float attackRange = 2.2f;
        [SerializeField] private float attackDamage = 12f;
        [SerializeField] private float attackCooldown = 1.2f;
        [SerializeField] private float viewAngle = 110f;
        [SerializeField] private LayerMask obstructionMask = ~0;

        private NavMeshAgent _agent;
        private Health _health;
        private int _patrolIndex;
        private float _nextAttackTime;
        private bool _isDead;

        private enum EnemyState
        {
            Patrol,
            Chase,
            Attack
        }

        private EnemyState _state;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
            _health.Died += OnDeath;
        }

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }

            GoToNextPatrolPoint();
        }

        private void Update()
        {
            if (_isDead || target == null)
            {
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            bool canSeePlayer = distanceToPlayer <= detectRadius && HasLineOfSight();

            if (canSeePlayer && distanceToPlayer > attackRange)
            {
                _state = EnemyState.Chase;
            }
            else if (distanceToPlayer <= attackRange)
            {
                _state = EnemyState.Attack;
            }
            else
            {
                _state = EnemyState.Patrol;
            }

            switch (_state)
            {
                case EnemyState.Patrol:
                    UpdatePatrol();
                    break;
                case EnemyState.Chase:
                    UpdateChase();
                    break;
                case EnemyState.Attack:
                    UpdateAttack();
                    break;
            }
        }

        private void UpdatePatrol()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                return;
            }

            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.15f)
            {
                GoToNextPatrolPoint();
            }
        }

        private void UpdateChase()
        {
            _agent.isStopped = false;
            _agent.SetDestination(target.position);
        }

        private void UpdateAttack()
        {
            _agent.isStopped = true;
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

            if (Time.time < _nextAttackTime)
            {
                return;
            }

            IDamageable damageable = target.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage, gameObject);
            }
            else
            {
                Health playerHealth = target.GetComponentInParent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage, gameObject);
                }
            }

            _nextAttackTime = Time.time + attackCooldown;
            GameEventBus.RaiseStatusChanged("Enemy Contact");
        }

        private bool HasLineOfSight()
        {
            Vector3 direction = target.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);

            if (angle > viewAngle * 0.5f)
            {
                return false;
            }

            Vector3 origin = transform.position + Vector3.up * 1.4f;
            Vector3 destination = target.position + Vector3.up * 1.2f;

            if (Physics.Linecast(origin, destination, out RaycastHit hit, obstructionMask, QueryTriggerInteraction.Ignore))
            {
                return hit.transform == target || hit.transform.IsChildOf(target);
            }

            return true;
        }

        private void GoToNextPatrolPoint()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                return;
            }

            _agent.isStopped = false;
            _agent.SetDestination(patrolPoints[_patrolIndex].position);
            _patrolIndex = (_patrolIndex + 1) % patrolPoints.Length;
        }

        private void OnDeath()
        {
            if (_isDead)
            {
                return;
            }

            _isDead = true;
            _agent.isStopped = true;
            _agent.enabled = false;

            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider item in colliders)
            {
                item.enabled = false;
            }

            StartCoroutine(SinkRoutine());
        }

        private IEnumerator SinkRoutine()
        {
            Vector3 start = transform.position;
            Vector3 end = start - Vector3.up * 1f;
            float elapsed = 0f;

            while (elapsed < 1.2f)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(start, end, elapsed / 1.2f);
                yield return null;
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.Died -= OnDeath;
            }
        }
    }
}
