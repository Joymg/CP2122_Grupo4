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


    private void OnEnable()
    {

        //se ajustan las estadisticas para que no sobrepasen el maximo
        damage = damage > damageCap ? damageCap : damage;
        range= range> rangeCap ? rangeCap : range;
        fireRate = fireRate > fireRateCap ? fireRateCap : fireRate;

        itemType = ItemType.Weapon;
        utility = (damage / damageCap) * .4f + range / rangeCap * .3f + fireRate / fireRateCap * .3f;
    }
}
