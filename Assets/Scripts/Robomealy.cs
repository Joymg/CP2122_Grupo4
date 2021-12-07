using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robomealy : Robot
{
    private StateMachineEngine fsm;

    Perception enemyClose;
    Perception itemClose;
    Perception itemPicked;

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

        //Low health
        Perception lowHealth = fsm.CreatePerception<ValuePerception>(() => GetHp() < 5);

        //Enemy detected
        Perception enemyDetected = fsm.CreatePerception<ValuePerception>(() => enemyTarget != null);

        //Enemy detected
        Perception enemyInRange = fsm.CreatePerception<ValuePerception>(() =>
        enemyTarget != null &&
        Vector3.Distance(enemyTarget.transform.position, transform.position)
        <= currentEquipment.weapon.range);

        //Item detected
        Perception itemDetected = fsm.CreatePerception<ValuePerception>(() => itemTarget != null);

        //Item better than existing
        itemPicked = fsm.CreatePerception<PushPerception>();


        fsm.CreateTransition("chaseEnemy", wander, enemyDetected, chase);
        fsm.CreateTransition("attackEnemy", chase, enemyInRange, attack);
        fsm.CreateTransition("chaseEnemyAfterAttack", attack, enemyDetected, chase);
        fsm.CreateTransition("flee", chase, lowHealth, flee);
        fsm.CreateTransition("fleeAfterAttack", attack, lowHealth, flee);


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
        /*CheckLeftClick();
        CheckRightClick();
        transform.Rotate(Vector3.up, 10f * Time.deltaTime);*/
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
