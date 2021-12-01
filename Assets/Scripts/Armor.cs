using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/New Armor", order = 1)]
public class Armor : Item
{
    public Material material;
    public int hpUpgrade;
}
