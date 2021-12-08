using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/New Armor", order = 1)]
public class Item : ScriptableObject
{
    //topes maximos que se le pueden poner a las estadisicas que aporta un objeto
    protected static float damageCap = 100;
    protected static float rangeCap = 10;
    protected static float fireRateCap = 10;
    protected static float healthBoostCap = 200;
    protected static float clockSpeedCap = 3;
    protected static float detectionRangeCap = 10;

    public ItemType itemType;
    public float utility;
}

public enum ItemType
{
    Weapon,
    Processor,
    Armor
}
