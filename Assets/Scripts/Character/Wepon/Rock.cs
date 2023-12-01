using RPG.Character.Enemy;
using RPG.State;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character.Wepon
{
    public enum RockStates { HIT_NOTHING, HIT_PLAYER, HIT_ENEMY }
    public class Rock : MonoBehaviour
    {
        [Header("Damage")]
        public float Damage = 10f;

        [Header("Partical")]
        public GameObject RockBreakEffect;
        
        [HideInInspector]
        public RockStates RockStates;

        [HideInInspector]
        public Vector3 TargetPosition;

        private Rigidbody rb;
        private Vector3 direction;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            FindObjectOfType<Golem>().OnRockThorw += FlyToTarget;
        }

        private void FixedUpdate()
        {
            if (rb.velocity.sqrMagnitude < 1)
                RockStates = RockStates.HIT_NOTHING;
        }

        public void FlyToTarget(float force)
        {
            if (TargetPosition != null)
            {
                direction = (TargetPosition - transform.position + Vector3.up * 2).normalized;
                rb.AddForce(direction * force, ForceMode.Impulse);
            }
            FindObjectOfType<Golem>().OnRockThorw -= FlyToTarget;
        }

        private void OnCollisionEnter(Collision collision)
        {
            switch (collision.transform.tag)
            {
                case "Player":
                    if (RockStates == RockStates.HIT_PLAYER)
                    {
                        var go = collision.gameObject;
                        CharacterStates characterstates = go.GetComponent<CharacterStates>();
                        characterstates.TakeDamage(Damage, characterstates);
                        go.GetComponent<NavMeshAgent>().isStopped = true;
                        go.GetComponent<NavMeshAgent>().velocity = direction * 20f;
                        go.GetComponent<Animator>().SetTrigger("hit");
                        RockStates = RockStates.HIT_NOTHING;
                    }
                    break;
                case "Enemy":
                    if (RockStates == RockStates.HIT_ENEMY)
                    {
                        var go = collision.gameObject;
                        CharacterStates characterstates = go.GetComponent<CharacterStates>();
                        characterstates.TakeDamage(Damage * 1.5f, characterstates);
                        go.GetComponent<NavMeshAgent>().isStopped = true;
                        go.GetComponent<NavMeshAgent>().velocity = -direction * 20f;
                        Instantiate(RockBreakEffect, transform.position, Quaternion.identity);
                        go.GetComponent<Animator>().SetTrigger("hit");
                        RockStates = RockStates.HIT_NOTHING;
                        Destroy(gameObject, 0.5f);
                    }
                    break;
                    // Todo 石头落在地面上超过一定时间，Destroy
                default:
                    break;
            }
        }
    }
}