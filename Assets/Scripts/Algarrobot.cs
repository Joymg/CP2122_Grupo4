using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Algarrobot : Robot
{
    BehaviourTreeEngine tree;


    bool canIBeatEnemy;

    private void Awake()
    {
        tree = InitializeTree();
        base.Awake();
        //currentHP = maxCurrentHP/2;
    }

    #region Robot Actions
    //checks if hp is >=0 and returns succeed to the tree
    private ReturnValues CheckArmor()
    {
        if (currentHP <= 0)
        {
            return ReturnValues.Succeed;
        }
        return ReturnValues.Failed;
    }

    private ReturnValues CheckLowArmor()
    {
        if (currentHP > maxCurrentHP/2)
        {
            return ReturnValues.Failed;
        }
        return ReturnValues.Succeed;
    }

    private ReturnValues CheckEnemiesNear()
    {
        if (enemyTarget!=null) return ReturnValues.Succeed;
        return ReturnValues.Failed;
    }

    private ReturnValues CheckObjectNear()
    {
        if (itemTarget!=null) return ReturnValues.Succeed;
        return ReturnValues.Failed;
    }

    private ReturnValues CheckIfCanBeatEnemy()
    {
        if (canIBeatEnemy) return ReturnValues.Succeed;
        return ReturnValues.Failed;
    }

    private ReturnValues CheckIfBetterItemAndEquip()
    {
        Item item = itemTarget.GetComponent<ItemContainer>().item;
        switch (item.itemType)
        {
            case ItemType.Armor:
                if (GetEquipment().armor == null || GetEquipment().armorValue > item.utility)
                {
                    AddArmorToEquipment((Armor)item);
                    return ReturnValues.Succeed;
                }
                break;
            case ItemType.Processor:
                if (GetEquipment().processor == null || GetEquipment().processorValue > item.utility)
                {
                    AddProcessorToEquipment((Processor)item);
                    return ReturnValues.Succeed;
                }
                break;
            case ItemType.Weapon:
                if (GetEquipment().weapon == null || GetEquipment().weaponValue > item.utility)
                {
                    AddWeaponToEquipment((Weapon)item);
                    return ReturnValues.Succeed;
                }
                break;
        }

        return ReturnValues.Failed;
    }
    #endregion Robot Actions

    #region ReturnValues
    private void NoneAction() { }
    private ReturnValues AlwaysSucceed()
    {
        return ReturnValues.Succeed;
    }
    private ReturnValues AlwaysFailed()
    {
        return ReturnValues.Failed;
    }
    private ReturnValues AlwaysRunning()
    {
        return ReturnValues.Running;
    }
    #endregion ReturnValues

    private void AnalyzeEnemy()
    {
        Robot enemy = enemyTarget.GetComponent<Robot>();
        if (currentHP > enemy.GetHp())
        {
            if(GetEquipment()!=null&& GetEquipment().weaponValue >= enemy.GetEquipment().armorValue)
            {
                canIBeatEnemy = true;
            }
        }
        else
        {
            canIBeatEnemy = false;
        }
    }
    protected override void WanderAction()
    {
        Debug.Log("TRY WANDER");
        if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
        {
            Vector3 newPos = RandomNavmeshLocation(detectionRange);
            agent.SetDestination(newPos);
            timer = 0;

        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        Debug.Log(tree.ActiveNode);
        tree.Update();
        base.Update();
    }

    BehaviourTreeEngine InitializeTree()
    {
        BehaviourTreeEngine tree = new BehaviourTreeEngine();
        SequenceNode root = tree.CreateSequenceNode("root", false); //we need a previous node for creating the tree so we use it as fake root

        //lvl 1 of the tree
        SelectorNode level1_root = tree.CreateSelectorNode("level1_root");
        LoopDecoratorNode loop = tree.CreateLoopNode("loop", level1_root);
        //nodes conection
        tree.SetRootNode(root);
        root.AddChild(loop);


        //level 1_1
        SequenceNode level1_1 = tree.CreateSequenceNode("DestroyNode", false);
        LeafNode checkArmor = tree.CreateLeafNode("CheckArmor", NoneAction, CheckArmor);
        LeafNode destroyRobot = tree.CreateLeafNode("DestroyRobot", Die, AlwaysSucceed);

                //lvl 1_1 conections
        level1_root.AddChild(level1_1);
        level1_1.AddChild(checkArmor);
        level1_1.AddChild(destroyRobot);

            //level 1_2
        SequenceNode level1_2 = tree.CreateSequenceNode("RepairNode", false);
        LeafNode checkLowArmor = tree.CreateLeafNode("CheckLowArmor", NoneAction, CheckLowArmor);
        LeafNode repairRobot = tree.CreateLeafNode("RepairRobot", RepairAction, AlwaysSucceed);

                //lvl 1_2 conections
        level1_root.AddChild(level1_2);
        level1_2.AddChild(checkLowArmor);
        level1_2.AddChild(repairRobot);

            //level 1_3
        LeafNode level1_3 = tree.CreateLeafNode("Wander", WanderAction, AlwaysSucceed);

                //lvl 1_3 conections
        level1_root.AddChild(level1_3);

        //level 2
        SelectorNode level2_root = tree.CreateSelectorNode("level2_root");
        level1_root.AddChild(level2_root);
        LeafNode enemyNear = tree.CreateLeafNode("enemyNear", NoneAction, CheckEnemiesNear);
        LeafNode objectNear = tree.CreateLeafNode("objectNear", NoneAction, CheckObjectNear);

            //level 2 connections
        level2_root.AddChild(enemyNear);
        level2_root.AddChild(objectNear);

            //level 2_1
        SelectorNode level2_2_1 = tree.CreateSelectorNode("level_2_1");
        enemyNear.Child = level2_2_1;
        LeafNode level2_1_1 = tree.CreateLeafNode("imBeingAttack", NoneAction, AlwaysSucceed);
        SequenceNode level2_1_2 = tree.CreateSequenceNode("level2_1_2", false);

        //Level 2_1 connections
        level2_2_1.AddChild(level2_1_1);
        level2_2_1.AddChild(level2_1_2);

            //level 2_1_1
            //TO DO

            //level 2_1_2
        LeafNode analyzeEnemy = tree.CreateLeafNode("analyzeEnemy", AnalyzeEnemy, AlwaysSucceed);
        LeafNode canIBeatEnemy = tree.CreateLeafNode("canIBeatEnemy", NoneAction, CheckIfCanBeatEnemy);
        LeafNode chaseEnemy = tree.CreateLeafNode("chaseEnemy", ChaseAction, AlwaysSucceed);
        LeafNode attack = tree.CreateLeafNode("attack", AttackAction, AlwaysSucceed);

            //Level 2_1_2 connections
        level2_1_2.AddChild(analyzeEnemy);
        level2_1_2.AddChild(canIBeatEnemy);
        level2_1_2.AddChild(chaseEnemy);
        level2_1_2.AddChild(attack);

        //level 2_2
        SequenceNode level2_2_2 = tree.CreateSequenceNode("level2_2_2", false);
        objectNear.Child = level2_2_2;
        LeafNode goToItem = tree.CreateLeafNode("goToItem", MoveToItemAction, AlwaysSucceed);
        LeafNode checkItem = tree.CreateLeafNode("checkItem", NoneAction, CheckIfBetterItemAndEquip);

        //level 2_2 connections
        level2_2_2.AddChild(goToItem);
        level2_2_2.AddChild(checkItem);

        return tree;

    }
}
