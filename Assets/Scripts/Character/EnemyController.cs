using RPG.Manager;
using RPG.State;
using RPG.Tools;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace RPG.Character
{
    public enum EnemyStat { GUARD, PATROL, CHASE, DEAD }
    // RequireComponent �鿴�Ƿ��������û��ʱ����Ӵ˽ű����Զ�������
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CharacterStates))]
    public class EnemyController : MonoBehaviour, IEndGameObserve
    {
        private Collider collider;
        protected NavMeshAgent agent;
        protected Animator anim;
        protected CharacterStates characterStates;

        
        [Header("Basic Settings")]
        public float ChaseRadius;
        //public float AttackRadius;
        public bool IsGuard;
        public float PatrolRange;
        public float PatrolLookTime;
        //public event Action OnHitted;

        private EnemyStat enemyStat;
        private float remainPatrolLookTime;
        private Vector3 guardPosition;
        private Quaternion guardRotation;
        private Vector3 patrolPoint;
        private bool playerDead;
        
        protected GameObject attackTarget;
        protected bool walk;
        protected bool findPlayer;
        protected bool chasePlayer;
        protected float speed;
        protected float lastSkillTime;
        protected bool isSkill;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            collider = GetComponent<Collider>();
            characterStates = GetComponent<CharacterStates>();
            speed = agent.speed;
            guardPosition = transform.position;
            guardRotation = transform.rotation;
            patrolPoint = transform.position;
            remainPatrolLookTime = PatrolLookTime;
            lastSkillTime = characterStates.Cooldown;
        }

        void Start()
        {
            GameManager.Instance.AddObserve(this);
        }

        private void OnEnable()
        {
            // fixme ���س���
            //GameManager.Instance.AddObserve(this);
        }
        private void OnDisable()
        {
            // fixme NPE
            if(GameManager.IsInitialized)
                GameManager.Instance.RemoveObserve(this);
        }

        void Update()
        {
            ChangeState();
            ChangeAnimation();
        }

        private void ChangeAnimation()
        {
            anim.SetBool("walk", walk);
            anim.SetBool("findPlayer", findPlayer);
            anim.SetBool("chasePlayer", chasePlayer);
            anim.SetBool("victory", playerDead);
        }

        private void ChangeState()
        {
            if (playerDead)
            {
                return;
            }
            lastSkillTime += Time.deltaTime;
            if (characterStates.CurrentHealth <= 0)
            {
                enemyStat = EnemyStat.DEAD;
            }
            else if (FindPlayer(out attackTarget))
            {
                // ����ҵ�Player���л�enemy״̬ΪCHASE
                enemyStat = EnemyStat.CHASE;
                if (speed < 3)
                {
                    speed *= 1.2f;
                }
            }
            else if (enemyStat != EnemyStat.DEAD)
            {
                agent.destination = transform.position;
                if (IsGuard)
                    enemyStat = EnemyStat.GUARD;
                else
                    enemyStat = EnemyStat.PATROL;
            }
            switch (enemyStat)
            {
                case EnemyStat.GUARD:
                    findPlayer = false;
                    chasePlayer = false;
                    if (guardPosition != transform.position)
                    {
                        agent.isStopped = false;
                        walk = true;
                        agent.destination = guardPosition;
                        if (Vector3.SqrMagnitude(guardPosition - transform.position) <= agent.stoppingDistance)
                        {
                            walk = false;
                            agent.isStopped = true;
                            transform.rotation = Quaternion.Slerp(transform.rotation, guardRotation, 0.01f);
                        }
                    }
                    break;
                case EnemyStat.PATROL:
                    walk = true;
                    findPlayer = false;
                    chasePlayer = false;
                    agent.isStopped = false;
                    agent.speed = 1.5f;
                    if (Vector3.Distance(transform.position, patrolPoint) <= agent.stoppingDistance)
                    {
                        //walk = false;
                        // ����Ѳ�ߵ㣬��ͣ3s��֮�����Ѳ��
                        remainPatrolLookTime -= Time.deltaTime;
                        if (remainPatrolLookTime <= 0)
                        {
                            remainPatrolLookTime = PatrolLookTime;
                            patrolPoint = NewPatrolPoint();
                            Debug.DrawLine(transform.position, patrolPoint);
                        }
                        else
                        {
                            walk = false;
                        }
                        return;
                    }
                    agent.destination = patrolPoint;
                    break;
                case EnemyStat.CHASE:
                    ChasePlayer();
                    break;
                case EnemyStat.DEAD:
                    collider.enabled = false;
                    agent.radius = 0;
                    Destroy(gameObject, 2f);
                    break;
                default:
                    break;
            }
        }

        protected virtual void ChasePlayer()
        {
            findPlayer = true;
            chasePlayer = true;
            walk = false;
            // ׷Player
            agent.speed = speed;
            if (TargetInSkillRange() && lastSkillTime >= characterStates.Cooldown && !TargetInAttackRange())
            {
                lastSkillTime = 0;
                Attak(true);
            }
            else if (TargetInAttackRange())
            {
                Attak(false);
            }
            else
            {
                agent.destination = attackTarget.transform.position;
                agent.isStopped = false;
            }
        }

        protected bool TargetInAttackRange()
        {
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStates.AttackRange;
        }
        protected bool TargetInSkillRange()
        {
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStates.SkillRange;
        }

        protected void Attak(bool skill)
        {
            transform.LookAt(attackTarget.transform);
            agent.isStopped = true;
            isSkill = skill;
            if (skill)
            {
                anim.SetTrigger("skill");
            }
            else
            {
                anim.SetTrigger("attack");
            }
            // ���㱩����
            characterStates.IsCritical = Random.value <= characterStates.CriticalChance;
        }
        protected virtual bool CanSkill() 
        {
            return TargetInSkillRange() && transform.IsFacingTarget(attackTarget.transform);
        }

        private Vector3 NewPatrolPoint()
        {
            float randomX = Random.Range(-PatrolRange, PatrolRange);
            float randomZ = Random.Range(-PatrolRange, PatrolRange);

            Vector3 randomPoint = new Vector3(guardPosition.x + randomX, transform.position.y, guardPosition.z + randomZ);

            //  �µ�Ѳ�ߵ�Ѱ�ҿ��������� WalkAale
            NavMeshHit hit;
            return NavMesh.SamplePosition(randomPoint, out hit, PatrolRange, 1) ? hit.position : transform.position;
        }

        private bool FindPlayer(out GameObject target)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, ChaseRadius);
            foreach (var item in colliders)
            {
                if (item.gameObject.CompareTag("Player"))
                {
                    target = item.gameObject;
                    return true;
                }
            }
            target = null;
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, ChaseRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, PatrolRange);
        }
        public void EndNotify()
        {
            walk = false;
            findPlayer = false;
            chasePlayer = false;
            playerDead = true;
        }
        // attack event
        void hit()
        {
            if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
            {
                var targetStates = attackTarget.GetComponent<CharacterStates>();
                float distance = Vector3.SqrMagnitude(transform.position - attackTarget.transform.position);
                if (isSkill && distance <= characterStates.SkillRange)
                {
                    targetStates.GetComponent<Animator>().SetTrigger("hit");
                }
                targetStates.TakeDamage(characterStates, targetStates);
            }
        }

    }
}
