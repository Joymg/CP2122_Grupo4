using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/New Armor", order = 1)]
public class Item : ScriptableObject
{
    public ItemType itemType;

}

public enum ItemType
{
    Weapon,
    Processor,
    Armour
}
