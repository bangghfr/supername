using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollecter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log(Inventory.instance.items.Count);
        Debug.Log(Inventory.instance.space);
        if (collision.gameObject.TryGetComponent<Collectable>(out Collectable collectable) && Inventory.instance.items.Count < Inventory.instance.space)
        {
            Debug.Log("да");
            Inventory.instance.Add(collectable.Item);
        }
    }
}
