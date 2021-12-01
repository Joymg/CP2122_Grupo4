using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Processor", menuName = "ScriptableObjects/New Processor", order = 1)]
public class Processor : Item
{
    public Material material;
    public float clockSpeedBoost;
    

    private void Awake()
    {
        utility = clockSpeedBoost / clockSpeedCap;
    }
    private void OnEnable()
    {
        itemType = ItemType.Processor;
        utility = clockSpeedBoost / clockSpeedCap;
    }
}

