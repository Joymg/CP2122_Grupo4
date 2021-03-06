using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Robot>(out Robot robot))
        {
            if (robot.GetItemAction(item))
            {
                Destroy(gameObject);
            }
        }
    }
}
