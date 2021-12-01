using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/New Weapon", order = 1)]
public class Weapon : Item
{

    public GameObject model; //(?)
    public float damage;
    public float range;
    public float fireRate;

    private void Awake()
    {
        utility = (damage / damageCap) * .4f + range / rangeCap * .3f + fireRate / fireRateCap * .3f;
    }
    private void OnEnable()
    {
        itemType = ItemType.Weapon;
        utility = (damage / damageCap) * .4f + range / rangeCap * .3f + fireRate / fireRateCap * .3f;
    }
}
