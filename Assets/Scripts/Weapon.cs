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
    

}
