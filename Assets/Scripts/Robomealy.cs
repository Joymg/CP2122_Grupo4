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
        fsm = new StateMachineEngine();

        State wander = fsm.CreateEntryState("wander", WanderAction);
        State moveTowardsObject = fsm.CreateState("moveTowardsObject", MoveToItemAction);
        //State equipObject = fsm.CreateState("equipObject");
        State chase = fsm.CreateState("chase", ChaseAction);
        State attack = fsm.CreateState("attack", AttackAction);
        State flee = fsm.CreateState("flee", FleeAction);
        State repair = fsm.CreateState("repair");

        //Done moving
        isDone = fsm.CreatePerception<PushPerception>();

        //Item detected
        Perception itemDetected = fsm.CreatePerception<ValuePerception>(() => itemTarget != null);

        //Item picked or not
        itemPicked = fsm.CreatePerception<PushPerception>();

        //Enemy detected
        Perception enemyDetected = fsm.CreatePerception<ValuePerception>(() => enemyTarget != null);

        //Enemy lost
        Perception enemyLost = fsm.CreatePerception<ValuePerception>(() => enemyTarget == null);

        //Enemy detected
        Perception enemyInRange = fsm.CreatePerception<ValuePerception>(() => currentEquipment.weapon != null && Vector3.Distance(enemyTarget.transform.position, transform.position)
        <= currentEquipment.weapon.range);

        //Enemy stronger
        Perception strongerThanEnemy = fsm.CreatePerception<ValuePerception>(() => enemyTarget != null && currentEquipment.IsBetterThan(enemyTarget.GetComponent<Robot>().GetEquipment()));
        Perception weakerThanEnemy = fsm.CreatePerception<ValuePerception>(() => enemyTarget != null && !currentEquipment.IsBetterThan(enemyTarget.GetComponent<Robot>().GetEquipment()));

        Perception strongerAndClose = fsm.CreateAndPerception<AndPerception>(enemyDetected, strongerThanEnemy);
        Perception weakerAndClose = fsm.CreateAndPerception<AndPerception>(enemyDetected, weakerThanEnemy);


        //Low health
        Perception lowHealth = fsm.CreatePerception<ValuePerception>(() => GetHp() < 5);
        Perception fullHealth = fsm.CreatePerception<ValuePerception>(() => GetHp() == maxCurrentHP);

        fsm.CreateTransition("isBored", wander, isDone, wander);
        fsm.CreateTransition("isFleeing", flee, isDone, flee);
        fsm.CreateTransition("isChasingEnemy", chase, isDone, chase);

        fsm.CreateTransition("chaseEnemy", wander, strongerThanEnemy, chase);
        fsm.CreateTransition("looseEnemy", chase, enemyLost, wander);
        fsm.CreateTransition("attackEnemy", chase, enemyInRange, attack);
        fsm.CreateTransition("chaseEnemyAfterAttack", attack, enemyDetected, chase);
        fsm.CreateTransition("flee", wander, lowHealth, flee);
        fsm.CreateTransition("fleeAfterChase", chase, lowHealth, flee);
        fsm.CreateTransition("fleeAfterAttack", attack, lowHealth, flee);
        fsm.CreateTransition("repairAfterFlee", flee, enemyLost, repair);
        fsm.CreateTransition("wanderAfterFlee", repair, fullHealth, wander);
        fsm.CreateTransition("fleeWhileReparing", repair, enemyDetected, flee);


        fsm.CreateTransition("goToObject", wander, itemDetected, moveTowardsObject);
        fsm.CreateTransition("arriveToObject", moveTowardsObject, itemPicked, wander);



        //fsm.CreateTransition("arriveToWorseObject", moveTowardsObject, worseItem, wander);

        //fsm.CreateTransition("keep wandering", wander, , wander);



        /*State blue = fsm.CreateState("blue", TurnBlue);
        Perception click = fsm.CreatePerception<PushPerception>();
        State green = fsm.CreateState("gree", TurnGreen);

        Perception timer = fsm.CreatePerception<TimerPerception>(1f);
        Perception threeClicks = fsm.CreatePerception<ValuePerception>(() => count >= 3);

        fsm.CreateTransition("mouse click", red, click, blue);
        fsm.CreateTransition("timer", blue, timer, red);

        fsm.CreateTransition("mouseClickFromGreen", green, click, red);
        fsm.CreateTransition("3clicksFromRed", red, threeClicks, green);
        fsm.CreateTransition("3clicksFromBlue", blue, threeClicks, green);*/
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
        debugText = fsm.GetCurrentState().Name;
        /*CheckLeftClick();
        CheckRightClick();
        transform.Rotate(Vector3.up, 10f * Time.deltaTime);*/

        if (agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete)
            isDone.Fire();

        if (GetHp() <= 0)
            Die();
    }

    void DoSomething()
    {
        fsm.Fire("mouse click");
        fsm.Fire("mouseClickFromGreen");
    }

    public void PickItem()
    {
        itemPicked.Fire();
    }
}
