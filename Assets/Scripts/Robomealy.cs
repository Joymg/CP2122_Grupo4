using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robomealy : Robot
{
    private StateMachineEngine fsm;


    Perception itemPicked;
    Perception isDone;

    // Start is called before the first frame update
    void Start()
    {
        fov.UpdateViewRange(detectionRange);

        fsm = new StateMachineEngine();

        State wander = fsm.CreateEntryState("wander", WanderAction);
        State moveTowardsObject = fsm.CreateState("moveTowardsObject", MoveToItemAction);
        State chase = fsm.CreateState("chase", ChaseAction);
        State attack = fsm.CreateState("attack");
        State flee = fsm.CreateState("flee", FleeAction);
        State repair = fsm.CreateState("repair", RepairAction);

        //Done moving
        isDone = fsm.CreatePerception<PushPerception>();

        //Item detected
        Perception itemDetected = fsm.CreatePerception<ValuePerception>(() => itemTarget != null);

        //Item picked or not
        itemPicked = fsm.CreatePerception<PushPerception>();

        //Enemy close
        Perception enemyClose = fsm.CreatePerception<ValuePerception>(() => enemyTarget != null);

        //Enemy lost
        Perception enemyLost = fsm.CreatePerception<ValuePerception>(() => enemyTarget == null);

        //Enemy detected
        Perception enemyInRange = fsm.CreatePerception<ValuePerception>(() => currentEquipment.weapon != null && Vector3.Distance(enemyTarget.transform.position, transform.position)
        <= currentEquipment.weapon.range);

        //Enemy stronger
        Perception strongerThanEnemy = fsm.CreatePerception<ValuePerception>(() => enemyTarget != null && currentEquipment.IsBetterThan(enemyTarget.GetComponent<Robot>().GetEquipment()));
        Perception weakerThanEnemy = fsm.CreatePerception<ValuePerception>(() => enemyTarget != null && !currentEquipment.IsBetterThan(enemyTarget.GetComponent<Robot>().GetEquipment()));

        //Low health
        Perception lowHealth = fsm.CreatePerception<ValuePerception>(() => CheckIfLowHealth());
        Perception notFullHealth = fsm.CreatePerception<ValuePerception>(() => GetHp() < maxCurrentHP);
        Perception fullHealth = fsm.CreatePerception<ValuePerception>(() => GetHp() == maxCurrentHP);
        Perception canRepair = fsm.CreateAndPerception<AndPerception>(notFullHealth, enemyLost);

        fsm.CreateTransition("isBored", wander, isDone, wander);//Nothing to do, keep wondering
        fsm.CreateTransition("isFleeing", flee, isDone, flee);//Continue fleeing from enemy
        fsm.CreateTransition("isChasingEnemy", chase, isDone, chase);//Continue chasing enemy

        //Start wandering after
        fsm.CreateTransition("wanderAfterLosingEnemy", chase, enemyLost, wander); //Enemy lost during chase
        fsm.CreateTransition("wanderAfterEnemyDead", attack, enemyLost, wander); //Enemy defeated
        fsm.CreateTransition("wanderAfterRepair", repair, fullHealth, wander);//Repairing
        fsm.CreateTransition("arriveToObject", moveTowardsObject, itemPicked, wander);//Picking object
        fsm.CreateTransition("arriveToObjectButNotInterested", moveTowardsObject, isDone, wander);//Not picking object

        //Start chasing if...
        fsm.CreateTransition("chaseEnemy", wander, strongerThanEnemy, chase);//Can beat him
        fsm.CreateTransition("chaseEnemyAfterAttack", attack, strongerThanEnemy, chase);//Is already chasing

        //Start fleeing
        fsm.CreateTransition("flee", wander, lowHealth, flee);
        fsm.CreateTransition("fleeAfterChase", chase, lowHealth, flee);//Gets too hurt while chasing
        fsm.CreateTransition("fleeAfterAttack", attack, lowHealth, flee);//Gets too hurt while attacking
        fsm.CreateTransition("fleeWhileReparing", repair, enemyClose, flee);//Enemy gets too close while repairing

        //Start attacking
        fsm.CreateTransition("attackEnemy", chase, enemyInRange, attack);//After chasing
        fsm.CreateTransition("keepAttacking", attack, enemyInRange, attack);//After attacking

        //Start repairing
        fsm.CreateTransition("repairAfterFlee", flee, enemyLost, repair);//After losing enemy while fleeing
        fsm.CreateTransition("keepRepairing", repair, canRepair, repair);//After losing enemy while fleeing

        //Go to object
        fsm.CreateTransition("goToObject", wander, itemDetected, moveTowardsObject);//When one is detected
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
        debugText = fsm.GetCurrentState().Name;

        //If path is complete or interrupted, inform
        if (agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete ||
            agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial ||
            agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
            isDone.Fire();

        //If health too low, die
        if (GetHp() <= 0)
            Die();
    }

    public void PickItem()
    {
        if (itemPicked != null)
            itemPicked.Fire();
    }
}
