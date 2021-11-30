using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    /// <summary>
    /// Starting Health points 
    /// </summary>
    private int _initHP;

    /// <summary>
    /// Maximum Helath points a Robot can have, it can be upgraded with Armor
    /// </summary>
    public int maxCurrentHP;

    /// <summary>
    /// Agent current Health points
    /// </summary>
    public int currentHP;


    /// <summary>
    /// Rate at which the robot will check for enemys or objets
    /// </summary>
    private float _initClockSpeed;

    /// <summary>
    /// Starting Movement Speed
    /// </summary>
    private int _initMS;

    /// <summary>
    /// Agent curretn Movement Speed
    /// </summary>
    public int currentMS;

    public Equipment currentEquipment;

    private void Start()
    {
        currentHP = _initHP;
        currentMS = _initMS;


    }
}

[System.Serializable]
public class Equipment{

    public Weapon currentWeapon;
    public Armor currentArmor;
    public Processor currentProcessor;

    
}
