using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inquisitron : Robot
{

    #region variables

    private UtilitySystemEngine utilitySystemEngine;


    private Factor Armor;
    private Factor Weapon;
    private Factor Danger;
    private Factor ItemDetected;

    private Factor Safe;
    private Factor Energy;
    private Factor Reckless;
    private Factor Strength;
    private Factor Calm;
    private Factor Risk;
    private Factor Fear;
    private Factor Security;


    public FactorValues factorValues;
    public CurrentAction currentAction;

    private UtilityAction Repair;
    private UtilityAction Attack;
    private UtilityAction Chase;
    private UtilityAction Flee;
    private UtilityAction Wander;
    private UtilityAction MoveToItem;

    //Place your variables here

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        utilitySystemEngine = new UtilitySystemEngine(false);


        CreateUtilitySystem();
    }


    private void CreateUtilitySystem()
    {
        // FACTORS
        Armor = new LeafVariable(() => currentHP/maxCurrentHP, 1, 0);
        Weapon = new LeafVariable(() => currentEquipment.weaponValue, 1, 0);
        Danger = new LeafVariable(() => currentEquipment.processorValue, 1, 0);
        ItemDetected = new LeafVariable(() => Convert.ToSingle(IsItemDetected), 1, 0);

        Energy = new ExpCurve(Armor, -Mathf.Log(1.4f), 0, -0.91f);
        List<Point2D> CalmPoints = new List<Point2D>
        {
            new Point2D(0, 1),
            new Point2D(0.1f, 1),
            new Point2D(0.1f, 0),
            new Point2D(1, 0),
        };

        Calm = new LinearPartsCurve(Danger, CalmPoints);

        List<Factor> SafeFactors = new List<Factor>
        {
            Energy,
            Calm,
        };

        List<float> SafeWeights = new List<float>
        {
            .5f,
            .5f,
        };

        Safe = new WeightedSumFusion(SafeFactors, SafeWeights);
        Fear = new LinearCurve(Danger, 1, 0);

        List<Factor> RiskFactors = new List<Factor>
        {
            Energy,
            Fear
        };

        List<float> RiskWeights = new List<float>
        {
            0.5f,
            0.5f,
        };

        Risk = new WeightedSumFusion(RiskFactors, RiskWeights);

        Strength = new ExpCurve(Weapon, 1, 0, 0);


        List<Factor> RecklessFactors = new List<Factor>
        {
            Strength,
            Risk,
        };

        List<float> RecklessWeights = new List<float>
        {
            .5f,
            .5f,
        };
        Reckless = new WeightedSumFusion(RecklessFactors, RecklessWeights);

        List<Factor> SecurityFactors = new List<Factor>
        {
            Calm,
            ItemDetected,
        };

        List<float> SecurityWeights = new List<float>
        {
            .5f,
            .5f,
        };
        Security = new WeightedSumFusion(SecurityFactors, SecurityWeights);


        // ACTIONS
        Repair = utilitySystemEngine.CreateUtilityAction("Repair", RepairAction, Safe);
        Chase = utilitySystemEngine.CreateUtilityAction("Chase", ChaseAction, Reckless);
        Attack = utilitySystemEngine.CreateUtilityAction("Attack", AttackAction, Reckless);
        Flee = utilitySystemEngine.CreateUtilityAction("Flee", FleeAction, Risk);
        MoveToItem = utilitySystemEngine.CreateUtilityAction("MoveToItem", MoveToItemAction, Security);
        Wander = utilitySystemEngine.CreateUtilityAction("Wander", WanderAction, Calm);


        // ExitPerceptions

        // ExitTransitions

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (currentHP <= 0)
        {
            Die();
        }
        base.Update();
        utilitySystemEngine.Update();
        Debug.Log(utilitySystemEngine.actualState.Name);
        updateFactorsValues();

        switch (utilitySystemEngine.actualState.Name)
        {
            case "Repair":
                RepairAction();
                break;
            case "Chase":
                ChaseAction();
                break;
            case "Flee":
                FleeAction();
                break;
            case "Wander":
                WanderAction();
                break;
            case "MoveToItem":
                MoveToItemAction();
                break;
            default:
                Debug.Log("Non registered Action");
                break;
        }
    }

    private void updateFactorsValues()
    {
        factorValues.Armor = Armor.getValue();
        factorValues.Weapon = Weapon.getValue();
        factorValues.Danger = Danger.getValue();
        factorValues.Safe = Safe.getValue();
        factorValues.Reckless = Reckless.getValue();
        factorValues.Risk = Risk.getValue();
        factorValues.Fear = Fear.getValue();
        factorValues.Calm = Calm.getValue();
        factorValues.Strength = Strength.getValue();
        factorValues.Security = Security.getValue();
    }

}

[System.Serializable]
public class FactorValues
{
    public float Armor;
    public float Weapon;
    public float Danger;
    public float Safe;
    public float Energy;
    public float Reckless;
    public float Strength;
    public float Calm;
    public float Risk;
    public float Fear;
    public float Security;
}

public enum CurrentAction { 
    Repair,
    Chase,
    Flee,
    Wander
}
