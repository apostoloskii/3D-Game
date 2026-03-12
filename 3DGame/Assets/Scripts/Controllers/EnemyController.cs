using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* Controls the Enemy AI */

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterCombat))]
[RequireComponent(typeof(Animator))]

public class EnemyController : MonoBehaviour {

	public float lookRadius = 10f;

	Transform target;
	NavMeshAgent agent;
	CharacterCombat combat;
	Animator animator;

	void Start ()
	{
		target = PlayerManager.instance.player.transform;
		agent = GetComponent<NavMeshAgent>();
		combat = GetComponent<CharacterCombat>();
		animator = GetComponent<Animator>();
	}

	void Update ()
	{
		float distance = Vector3.Distance(target.position, transform.position);

		if (distance <= lookRadius)
		{
			agent.SetDestination(target.position);

			if (distance <= agent.stoppingDistance)
			{
				CharacterStats targetStats = target.GetComponent<CharacterStats>();

				if (targetStats != null)
				{
					combat.Attack(targetStats);
				}

				FaceTarget();
			}
		}

		// UPDATE ANIMATION SPEED
		float speedPercent = agent.velocity.magnitude / agent.speed;
		animator.SetFloat("speedPercent", speedPercent, 0.1f, Time.deltaTime);
	}

	void FaceTarget ()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lookRadius);
	}

	// FIX FOR Animation Event ERROR
	public void AttackHitEvent()
	{
		// This is called from animation
	}
}
