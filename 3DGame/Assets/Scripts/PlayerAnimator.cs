using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class PlayerAnimator : CharacterAnimator {

    public WeaponAnimations[] weaponAnimations;
    Dictionary<Equipment, AnimationClip[]> weaponAnimationsDict;

    NavMeshAgent agent; 

    protected override void Start()
    {
        base.Start();
        
        agent = GetComponent<NavMeshAgent>();

        if (EquipmentManager.instance != null)
        {
            EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
        }

        weaponAnimationsDict = new Dictionary<Equipment, AnimationClip[]>();
        foreach (WeaponAnimations a in weaponAnimations)
        {
            if (a.weapon != null)
                weaponAnimationsDict.Add(a.weapon, a.clips);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (agent != null)
        {
            // Ја пресметуваме брзината без да ја пишуваме во конзола
            float speedPercent = agent.velocity.magnitude / agent.speed;
            
            // Ја праќаме до аниматорот
            animator.SetFloat("speedPercent", speedPercent, 0.1f, Time.deltaTime);
        }
    }

    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if (newItem != null && newItem.equipSlot == EquipmentSlot.Weapon)
        {
            animator.SetLayerWeight(1, 1);
            if (weaponAnimationsDict.ContainsKey(newItem))
            {
                currentAttackAnimSet = weaponAnimationsDict[newItem];
            }
        }
        else if (newItem == null && oldItem != null && oldItem.equipSlot == EquipmentSlot.Weapon)
        {
            animator.SetLayerWeight(1, 0);
            currentAttackAnimSet = defaultAttackAnimSet;
        }

        if (newItem != null && newItem.equipSlot == EquipmentSlot.Shield)
        {
            animator.SetLayerWeight(2, 1);
        }
        else if (newItem == null && oldItem != null && oldItem.equipSlot == EquipmentSlot.Shield)
        {
            animator.SetLayerWeight(2, 0);
        }
    }

    [System.Serializable]
    public struct WeaponAnimations
    {
        public Equipment weapon;
        public AnimationClip[] clips;
    }
}