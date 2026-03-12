using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Keep track of equipment. Has functions for adding and removing items. */

public class EquipmentManager : MonoBehaviour {

    #region Singleton

    public enum MeshBlendShape {Torso, Arms, Legs };
    public Equipment[] defaultEquipment;

    public static EquipmentManager instance;
    public SkinnedMeshRenderer targetMesh;

    SkinnedMeshRenderer[] currentMeshes;

    void Awake ()
    {
        instance = this;
    }

    #endregion

    Equipment[] currentEquipment;   // Items we currently have equipped

    // Callback for when an item is equipped/unequipped
    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;
    
    Inventory inventory;    // Reference to our inventory

    void Start ()
    {
        inventory = Inventory.instance;

        // Автоматски го наоѓа Body ако полето во Inspector е празно
        if (targetMesh == null)
        {
            targetMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        // Initialize currentEquipment based on number of equipment slots
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
        currentMeshes = new SkinnedMeshRenderer[numSlots];

        EquipDefaults();
    }

    // Equip a new item
    public void Equip (Equipment newItem)
    {
        int slotIndex = (int)newItem.equipSlot;
        Equipment oldItem = Unequip(slotIndex);

        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        currentEquipment[slotIndex] = newItem;
        AttachToMesh(newItem, slotIndex);
    }

    // Unequip an item with a particular index
    public Equipment Unequip (int slotIndex)
    {
        Equipment oldItem = null;
        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);

            // SetBlendShapeWeight(oldItem, 0); // Исклучено за да не се сече телото

            if (currentMeshes[slotIndex] != null)
            {
                Destroy(currentMeshes[slotIndex].gameObject);
            }

            currentEquipment[slotIndex] = null;

            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }
        }
        return oldItem;
    }

    public void UnequipAll ()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }

        EquipDefaults();
    }

    void AttachToMesh(Equipment item, int slotIndex)
    {
        if (targetMesh == null) return;

        SkinnedMeshRenderer newMesh = Instantiate(item.mesh) as SkinnedMeshRenderer;
        
        // Поправање на позицијата и ротацијата
        newMesh.transform.parent = targetMesh.transform.parent;
        newMesh.transform.localPosition = Vector3.zero;
        newMesh.transform.localRotation = Quaternion.identity;
        newMesh.transform.localScale = Vector3.one;

        newMesh.rootBone = targetMesh.rootBone;
        newMesh.bones = targetMesh.bones;
        
        currentMeshes[slotIndex] = newMesh;

        // SetBlendShapeWeight(item, 100); // Исклучено за да не се сече телото
    }

    // Оваа функција сега е празна за да не прави проблеми
    void SetBlendShapeWeight(Equipment item, int weight)
    {
        // Моментално исклучено
    }

    void EquipDefaults()
    {
        foreach (Equipment e in defaultEquipment)
        {
            Equip(e);
        }
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.U))
            UnequipAll();
    }
}