using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Processor", menuName = "ScriptableObjects/New Processor", order = 1)]
public class Processor : Item
{
    public Material material;
    public float clockSpeedBoost;
    public float detectionRangeBoost;
    


    private void OnEnable()
    {
        //se ajustan las estadisticas para que no sobrepasen el maximo
        clockSpeedBoost = clockSpeedBoost > clockSpeedCap ? clockSpeedCap : clockSpeedBoost;
        detectionRangeBoost = detectionRangeBoost > detectionRangeCap ? detectionRangeCap : detectionRangeBoost;

        itemType = ItemType.Processor;
        utility = (clockSpeedBoost / clockSpeedCap) * .4f +  (detectionRangeBoost / detectionRangeCap) * .6f;
    }
}

