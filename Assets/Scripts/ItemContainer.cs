using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Item item;
    private Collider collider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Robot>(out Robot robot))
        {
            robot.GetItemAction(item);
        }
    }
}
