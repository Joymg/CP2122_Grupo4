using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Algarrobot : Robot
{
    BehaviourTreeEngine tree;
    bool canIBeatEnemy;
    protected override void Awake()
    {
        base.Awake();
        tree = InitializeTree();
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
        if (!CheckIfLowHealth())
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
        AnalyzeEnemy();
        if (canIBeatEnemy) return ReturnValues.Succeed;
        return ReturnValues.Failed;
    }

    private ReturnValues CheckIfBetterItemAndEquip()
    {
        if (itemTarget == null) return ReturnValues.Failed;
        Item item = itemTarget.GetComponent<ItemContainer>().item;
        switch (item.itemType)
        {
            case ItemType.Armor:

                if (GetEquipment().armor == null || GetEquipment().armorValue > item.utility)
                {
                    return ReturnValues.Succeed;
                }
                break;
            case ItemType.Processor:
                if (GetEquipment().processor == null || GetEquipment().processorValue > item.utility)
                {
                    return ReturnValues.Succeed;
                }
                break;
            case ItemType.Weapon:
                if (GetEquipment().weapon == null || GetEquipment().weaponValue > item.utility)
                {
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
        /*if (currentHP >= enemy.GetHp())
        {
            if(GetEquipment().weapon!=null)
            {
                canIBeatEnemy = true;
            }
        }
        else
        {
            canIBeatEnemy = false;
        }*/

        canIBeatEnemy = currentEquipment.IsBetterThan(enemy.GetEquipment());
    }

    /*protected override void WanderAction()
    {
        if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
        {
            Vector3 newPos = RandomNavmeshLocation(detectionRange);
            agent.SetDestination(newPos);
            timer = 0;

        }
    }

    protected override void RepairAction()
    {
        agent.SetDestination(transform.position);

        if (currentHP < maxCurrentHP / 2)
        {
            currentHP += reparationAmount;
            repairTimer = 0;
            currentHP = currentHP > maxCurrentHP ? maxCurrentHP : currentHP;

        }
    }*/


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        tree.Update();
        debugText = tree.actualState.Name;
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

        //wander
        LeafNode wanderNode = tree.CreateLeafNode("Wander", WanderAction, AlwaysFailed);
        level1_root.AddChild(wanderNode);

        //level 1_3
        SequenceNode level1_3 = tree.CreateSequenceNode("level1_3", false);   

        //object near management
        LeafNode objectNear = tree.CreateLeafNode("objectNear", NoneAction, CheckObjectNear);
        LeafNode goToItem = tree.CreateLeafNode("goToItem", MoveToItemAction, AlwaysSucceed);
        LeafNode checkItem = tree.CreateLeafNode("checkItem", NoneAction, CheckIfBetterItemAndEquip);

        //lvl 1_3 conections
        level1_root.AddChild(level1_3);
        level1_3.AddChild(objectNear);
        level1_3.AddChild(goToItem);
        level1_3.AddChild(checkItem);

        //enemy near management
        
        //level 1_4
        SequenceNode level1_4 = tree.CreateSequenceNode("level1_4", false);
        LeafNode enemyNear = tree.CreateLeafNode("enemyNear", NoneAction, CheckEnemiesNear);
        SelectorNode level1_5 = tree.CreateSelectorNode("level1_5");

        SequenceNode level1_5_1 = tree.CreateSequenceNode("level1_5_1", false);
        SequenceNode level1_5_2 = tree.CreateSequenceNode("level1_5_2", false);

        //level 1_4 connections
        level1_root.AddChild(level1_4);
        level1_4.AddChild(enemyNear);
        level1_4.AddChild(level1_5);

        //level 1_4_1 connections
        level1_5.AddChild(level1_5_1);
        level1_5.AddChild(level1_5_2);

        //level 1_5_1
        LeafNode lowArmor = tree.CreateLeafNode("lowArmor", NoneAction, CheckLowArmor);
        LeafNode flee = tree.CreateLeafNode("flee", FleeAction, AlwaysSucceed);

        //lowArmor Connections
        level1_5_1.AddChild(lowArmor);
        level1_5_1.AddChild(flee);

        //level 1_5_2
        LeafNode canIBeatEnemy = tree.CreateLeafNode("canIBeatEnemy", NoneAction, CheckIfCanBeatEnemy);
        LeafNode chaseEnemy = tree.CreateLeafNode("chaseEnemy", ChaseAction, AlwaysSucceed);
        LeafNode attack = tree.CreateLeafNode("attack", AttackAction, AlwaysSucceed);


        //noUnderAttackManagement connections
        level1_5_2.AddChild(canIBeatEnemy);
        level1_5_2.AddChild(chaseEnemy);
        level1_5_2.AddChild(attack);

        return tree;
    }
}
