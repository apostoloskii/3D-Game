using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour {

    public AnimationClip replaceableAttackAnim;
    public AnimationClip[] defaultAttackAnimSet;
    protected AnimationClip[] currentAttackAnimSet;

    const float locomationAnimationSmoothTime = .1f;

    NavMeshAgent agent;
    protected Animator animator;
    protected CharacterCombat combat;
    public AnimatorOverrideController overrideController;

    protected virtual void Start () {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        combat = GetComponent<CharacterCombat>();

        if (overrideController == null)
        {
            overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        }
        animator.runtimeAnimatorController = overrideController;

        currentAttackAnimSet = defaultAttackAnimSet;
        combat.OnAttack += OnAttack;
    }
    
    protected virtual void Update () {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speedPercent", speedPercent, locomationAnimationSmoothTime, Time.deltaTime);

        animator.SetBool("inCombat", combat.InCombat);
    }

    protected virtual void OnAttack()
    {
        animator.SetTrigger("attack");

        // ПРОВЕРКА: Ако полето во Inspector е празно, прекини за да нема грешка
        if (replaceableAttackAnim == null)
        {
            Debug.LogWarning("ГРЕШКА: Немаш ставено анимација во 'Replaceable Attack Anim' на објектот: " + gameObject.name);
            return;
        }

        // Проверка дали низата со анимации има содржина
        if (currentAttackAnimSet != null && currentAttackAnimSet.Length > 0)
        {
            int attackIndex = Random.Range(0, currentAttackAnimSet.Length);
            
            // Сега безбедно пристапуваме до .name бидејќи проверивме дека не е null
            overrideController[replaceableAttackAnim.name] = currentAttackAnimSet[attackIndex];
        }
    }
}