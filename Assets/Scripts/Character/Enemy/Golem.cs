using RPG.Character.Wepon;
using System;
using UnityEngine;

namespace RPG.Character.Enemy
{
    public class Golem : Boss
    {
        [Tooltip("Rock Instantiate Position")]
        public Transform RockOriginPosition;

        [Header("Skill"), Tooltip("Rock Throw Force")]
        public float Force = 20f;

        public event Action<float> OnRockThorw;

        public GameObject RockPrefab;

        public void ThorwRock()
        {
            if (attackTarget == null)
                attackTarget = FindObjectOfType<PlayerController>().gameObject;
            if (attackTarget != null) 
            {
                var rock = Instantiate(RockPrefab, RockOriginPosition.position, Quaternion.identity);
                rock.GetComponent<Rigidbody>().velocity = Vector3.one;
                rock.GetComponent<Rock>().TargetPosition = attackTarget.transform.position;
                rock.GetComponent<Rock>().RockStates = RockStates.HIT_PLAYER;
                OnRockThorw.Invoke(Force);
            }
        }
    }
}
