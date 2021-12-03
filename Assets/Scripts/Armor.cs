using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/New Armor", order = 1)]
public class Armor : Item
{
    public Material material;
    public float hpBoost;


    private void OnEnable()
    {
        //se ajustan las estadisticas para que no sobrepasen el maximo
        hpBoost = hpBoost > healthBoostCap ? healthBoostCap : hpBoost;

        itemType = ItemType.Armor;
        utility = hpBoost / healthBoostCap;
    }
}
