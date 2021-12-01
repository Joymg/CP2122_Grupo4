using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/New Armor", order = 1)]
public class Item : ScriptableObject
{
    protected static float damageCap = 10;
    protected static float rangeCap = 10;
    protected static float fireRateCap = 10;
    protected static float healthBoostCap = 10;
    protected static float clockSpeedCap = 10;

    public ItemType itemType;
    public float utility;

    
}

public enum ItemType
{
    Weapon,
    Processor,
    Armor
}
