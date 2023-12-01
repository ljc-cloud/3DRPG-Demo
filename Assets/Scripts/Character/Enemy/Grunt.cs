using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character.Enemy
{
    public class Grunt : Boss
    {
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

        
        void DizzyPlayer()
        {
            KickOff();
            if (CanSkill())
            {
                attackTarget.GetComponent<Animator>().SetTrigger("hit");
            }
        }
    }
}
