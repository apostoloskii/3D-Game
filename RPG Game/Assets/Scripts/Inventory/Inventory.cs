using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton

    public static Inventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }

        instance = this;
    }

    #endregion

    public List<Item> items = new List<Item>();

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public bool Add(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Trying to add a null item!");
            return false;
        }

        if (!item.isDefaultItem)
        {
            items.Add(item);

            onItemChangedCallback?.Invoke();

            return true;
        }

        return false;
    }

    public bool Remove(Item item)
    {
        if (items.Remove(item))
        {
            onItemChangedCallback?.Invoke();
            return true;
        }

        return false;
    }
}
