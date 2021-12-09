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
    private Factor GoForItem;


    public FactorValues factorValues;
    private CurrentAction currentAction;

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
        Danger = new LeafVariable(() => danger, 1, 0);
        ItemDetected = new LeafVariable(() => Convert.ToSingle(IsItemDetected), 1, 0);

        Energy = new ExpCurve(Armor, -Mathf.Exp(1.3f), -.97f, -0.08f);
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
            .7f,
            .3f,
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
            .35f,
            .65f,
        };

        Risk = new WeightedSumFusion(RiskFactors, RiskWeights);
        Factor RiskInverted = new LeafVariable(()=> 1 - Risk.getValue(),0,1);
        Strength = new ExpCurve(Weapon, 1, 0, 0);


        List<Factor> RecklessFactors = new List<Factor>
        {
            Strength,
            Risk
        };

        List<float> RecklessWeights = new List<float>
        {
            .7f,
            .3f,
        };
        Reckless = new WeightedSumFusion(RecklessFactors, RecklessWeights);
        Factor RecklessInverted = new LeafVariable(() => 1 - Reckless.getValue(), 0, 1);
        List<Factor> GoForItemFactors = new List<Factor>
        {
            RiskInverted,
            RecklessInverted,
            ItemDetected,

        };

        List<float> GoForItemWeights = new List<float>
        {
            .6f,
            .2f,
            .2f,
        };
        GoForItem = new WeightedSumFusion(GoForItemFactors, GoForItemWeights);


        // ACTIONS
        Repair = utilitySystemEngine.CreateUtilityAction("Repair", RepairAction, Safe);
        Chase = utilitySystemEngine.CreateUtilityAction("Chase", ChaseAction, Reckless);
       
        Flee = utilitySystemEngine.CreateUtilityAction("Flee", FleeAction, Risk);
        MoveToItem = utilitySystemEngine.CreateUtilityAction("MoveToItem", MoveToItemAction, GoForItem);
        Wander = utilitySystemEngine.CreateUtilityAction("Wander", WanderAction, Calm);

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (currentHP <= 0)
        {
            Die();
        }

        if (!dead)
        {
            base.Update();
            utilitySystemEngine.Update();
            debugText = utilitySystemEngine.actualState.Name;
            updateFactorsValues();

            //update danger
            if (enemyTarget)
            {
                danger = 1 - Vector3.Distance(enemyTarget.transform.position, transform.position)
                    / detectionRange;
            }
            else
            {
                danger = 0;
            }


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
    }

    private void updateFactorsValues()
    {
        factorValues.Armor = Armor.getValue();
        factorValues.Weapon = Weapon.getValue();
        factorValues.Danger = Danger.getValue();
        factorValues.Safe = Safe.getValue();
        factorValues.Energy = Energy.getValue();
        factorValues.Reckless = Reckless.getValue();
        factorValues.Risk = Risk.getValue();
        factorValues.Fear = Fear.getValue();
        factorValues.Calm = Calm.getValue();
        factorValues.Strength = Strength.getValue();
        factorValues.GoForItem = GoForItem.getValue();
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
    public float GoForItem;
}

public enum CurrentAction { 
    Repair,
    Chase,
    Flee,
    Wander
}
