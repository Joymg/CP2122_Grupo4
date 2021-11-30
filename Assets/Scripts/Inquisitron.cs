using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inquisitron : Robot
{
    UtilitySystemEngine utilitySystemEngine;

    #region variables

    private UtilitySystemEngine NewUS_US;


    private Factor Armor;
    private Factor Weapon;
    private Factor Danger;
    private Factor Safe;
    private Factor Energy;
    private Factor Reckless;
    private Factor Strength;
    private Factor Calm;
    private Factor Risk;
    private Factor Fear;
    private UtilityAction Repair;
    private UtilityAction Attack;
    private UtilityAction Chase;
    private UtilityAction Flee;
    private UtilityAction Prowl;

    //Place your variables here

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        NewUS_US = new UtilitySystemEngine(false);


        CreateUtilitySystem();
    }


    private void CreateUtilitySystem()
    {
        // FACTORS
        Armor = new LeafVariable(() => /*Reference to desired variable*/0.0f, 1, 0);
        Weapon = new LeafVariable(() => /*Reference to desired variable*/0.0f, 1, 0);
        Danger = new LeafVariable(() => /*Reference to desired variable*/0.0f, 1, 0);
        List<Factor> SafeFactors = new List<Factor>
        {
            Energy,
            Calm,
        };

        List<System.Single> SafeWeights = new List<System.Single>
        {
            1f,
            1f,
        };

        Safe = new WeightedSumFusion(SafeFactors, SafeWeights);
        Energy = new ExpCurve(Armor, 0, 0, 0);
        List<Factor> RecklessFactors = new List<Factor>
        {
            Strength,
            Risk,
        };

        List<System.Single> RecklessWeights = new List<System.Single>
        {
            1f,
            1f,
        };

        Reckless = new WeightedSumFusion(RecklessFactors, RecklessWeights);
        Strength = new ExpCurve(Weapon, 1, 0, 0);
        List<Point2D> CalmPoints = new List<Point2D>
        {
            new Point2D(0, 1),
            new Point2D(0.1f, 1),
            new Point2D(0.1f, 0),
            new Point2D(1, 0),
        };

        Calm = new LinearPartsCurve(Danger, CalmPoints);
        List<Factor> RiesgoFactors = new List<Factor>
        {
            Fear,
        };

        List<System.Single> RiskWeights = new List<System.Single>
        {
            1f,
        };

        Risk = new WeightedSumFusion(RiesgoFactors, RiskWeights);
        Fear = new LinearCurve(Danger, 1, 0);

        // ACTIONS
        Repair = NewUS_US.CreateUtilityAction("Repair", RepairAction, Safe);
        Attack = NewUS_US.CreateUtilityAction("Attack", AttackAction, Reckless);
        Chase = NewUS_US.CreateUtilityAction("Chase", ChaseAction, Reckless);
        Flee = NewUS_US.CreateUtilityAction("Flee", FleeAction, Risk);
        Prowl = NewUS_US.CreateUtilityAction("Prowl", ProwlAction, Calm);


        // ExitPerceptions

        // ExitTransitions

    }

    // Update is called once per frame
    private void Update()
    {
        NewUS_US.Update();
    }

    // Create your desired actions

    private void RepairAction()
    {

    }

    private void AttackAction()
    {

    }

    private void ChaseAction()
    {

    }

    private void FleeAction()
    {

    }

    private void ProwlAction()
    {

    }
}
