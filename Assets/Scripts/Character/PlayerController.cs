using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using RPG.Manager;
using RPG.State;
using RPG.Character.Wepon;
using UnityEngine.Assertions.Must;
using System.Linq;
using System.Collections.Generic;

namespace RPG.Character
{
    public class PlayerController : MonoBehaviour
    {
        //public event Action OnHitted;

        private NavMeshAgent agent;
        private Animator anim;
        private Collider collider;
        private GameObject attackTaget;
        private CharacterStates characterStates;

        private float lastAttackTime;
        private bool isSkill;
        private bool dead;
        private float stoppingDistance;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            collider = GetComponent<Collider>();
            characterStates = GetComponent<CharacterStates>();
            stoppingDistance = agent.stoppingDistance;
        }

        void Start()
        {
            MouseManager.Instance.OnMouseClicked += MoveToTarget;
            MouseManager.Instance.OnEnemyClicked += EventAttack;
            GameManager.Instance.RegisterPlayerStates(characterStates);

            //StartCoroutine(Skill());
        }
        void Update()
        {
            if (characterStates.CurrentHealth <= 0)
            {
                dead = true;
                agent.enabled = false;
                collider.enabled = false;
                GameManager.Instance.NotiFyEndGame();
                Destroy(gameObject, 2f);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(Skill());
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                agent.isStopped = true;
            }
            ChangeAnimation();
            lastAttackTime -= Time.deltaTime;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.GetComponent<Rock>())
            {

            }
        }

        private void ChangeAnimation()
        {
            anim.SetFloat("speed", agent.velocity.sqrMagnitude * 0.03f);
            anim.SetBool("die", dead);
        }

        private void MoveToTarget(Vector3 target)
        {
            agent.stoppingDistance = stoppingDistance;
            // ´ò¶Ï¹¥»÷
            StopAllCoroutines();
            if (dead) return;
            agent.isStopped = false;
            agent.destination = target;
        }

        private void EventAttack(GameObject target)
        {
            if (dead) return;
            if (target != null && target.CompareTag("Enemy"))
            {
                agent.stoppingDistance = target.GetComponent<NavMeshAgent>().radius + 0.4f;
                characterStates.AttackRange = target.GetComponent<NavMeshAgent>().radius + 0.4f;
                characterStates.IsCritical = Random.value <= characterStates.CriticalChance;
                attackTaget = target;
                StartCoroutine(AttackTarget());
            }
            else if (target.CompareTag("Attackable"))
            {
                attackTaget = target;
                StartCoroutine(AttackTarget());
            }
        }

        IEnumerator AttackTarget()
        {
            agent.isStopped = false;
            // LookAt
            transform.LookAt(attackTaget.transform);

            // GetClose
            float distance;
            while ((distance = Vector3.Distance(transform.position, attackTaget.transform.position)) > characterStates.AttackRange)
            {
                agent.destination = attackTaget.transform.position;
                yield return null;
            }
            agent.isStopped = true;

            // Attack
            if (lastAttackTime <= 0)
            {
                isSkill = false;
                anim.SetTrigger("attack");
                lastAttackTime = 0.5f;
            }
        }

        IEnumerator Skill()
        {
            agent.stoppingDistance = characterStates.SkillRange;
            characterStates.IsCritical = Random.value <= characterStates.CriticalChance;
            anim.SetTrigger("skill");
            isSkill = true;
            yield return null;
        }

        // attack event
        void hit()
        {
            Rock rock = FindObjectsOfType<Rock>().ToList().Find(item =>
            {
                float distance = Vector3.Distance(item.gameObject.transform.position, transform.position);
                return distance < 3f && transform.IsFacingTarget(item.gameObject.transform)
                    && (item.RockStates == RockStates.HIT_PLAYER || item.RockStates == RockStates.HIT_NOTHING);
            });
            if (rock)
            {
                rock.GetComponent<Rigidbody>().velocity = Vector3.one;
                rock.GetComponent<Rock>().RockStates = RockStates.HIT_ENEMY;
                rock.GetComponent<Rigidbody>().AddForce(transform.forward * 300f, ForceMode.Impulse);
                return;
            }

            if (attackTaget != null && !attackTaget.CompareTag("Attackable") && transform.IsFacingTarget(attackTaget.transform))
            {
                var targetStates = attackTaget.GetComponent<CharacterStates>();
                if (isSkill && Vector3.SqrMagnitude(transform.position - attackTaget.transform.position) <= characterStates.SkillRange)
                {
                    targetStates.GetComponent<Animator>().SetTrigger("hit");
                }
                targetStates.TakeDamage(characterStates, targetStates);
            }
        }
    }

}