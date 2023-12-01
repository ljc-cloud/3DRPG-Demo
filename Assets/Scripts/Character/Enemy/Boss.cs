using RPG.Character.Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character.Enemy
{
    public class Boss : EnemyController
    {
        [Header("Skill")]
        public float KickForce = 20f;
        protected override void ChasePlayer()
        {
            findPlayer = true;
            chasePlayer = true;
            walk = false;
            agent.speed = speed;
            if (TargetInSkillRange() && lastSkillTime >= characterStates.Cooldown)
            {
                lastSkillTime = 0;
                Attak(true);
            }
            else if (TargetInAttackRange() && lastSkillTime >= characterStates.Cooldown)
            {
                lastSkillTime = 0;
                Attak(false);
            }
            else
            {
                agent.destination = attackTarget.transform.position;
                agent.isStopped = false;
            }
        }

        protected virtual void KickOff()
        {
            transform.LookAt(attackTarget.transform);
            if (CanSkill())
            {
                Vector3 dir = attackTarget.transform.position - transform.position;
                dir.Normalize();
                // agent.isStopped = true; 
                attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
                attackTarget.GetComponent<NavMeshAgent>().velocity = dir * KickForce;
            }
        }
    }
}
