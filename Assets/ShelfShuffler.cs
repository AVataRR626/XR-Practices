using System.Collections.Generic;
using UnityEngine;

public class ShelfShuffler : MonoBehaviour
{
    public List<Transform> items = new List<Transform>();
    public List<Transform> slots = new List<Transform>();

    void Start()
    {
        ShuffleShelf();
    }

    [ContextMenu("Shuffle Now")]
    public void ShuffleShelf()
    {
        int count = items.Count;

        if (count == 0 || count != slots.Count)
        {
            Debug.LogError("ShelfShuffler: items and slots count mismatch.");
            return;
        }

        List<int> indexList = new List<int>();
        for (int i = 0; i < count; i++)
        {
            indexList.Add(i);
        }

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(i, count);
            int temp = indexList[i];
            indexList[i] = indexList[randomIndex];
            indexList[randomIndex] = temp;
        }

        for (int i = 0; i < count; i++)
        {
            int slotIndex = indexList[i];

            Transform item = items[i];
            Transform slot = slots[slotIndex];

            item.position = slot.position;
            item.SetSiblingIndex(slotIndex);
        }
    }
}

