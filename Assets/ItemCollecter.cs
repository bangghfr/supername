using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollecter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.TryGetComponent<Item>(out Item item)
            && Inventory.instance.items.Count < Inventory.instance.space )
        {
            Inventory.instance.Add(item);
        }
    }
}
