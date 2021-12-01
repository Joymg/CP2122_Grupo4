using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    /// <summary>
    /// Starting Health points 
    /// </summary>
    private float _initHP = 10f;

    /// <summary>
    /// Maximum Helath points a Robot can have, it can be upgraded with Armor
    /// </summary>
    public float maxCurrentHP;

    /// <summary>
    /// Agent current Health points
    /// </summary>
    public float currentHP;


    /// <summary>
    /// Rate at which the robot will check for enemys or objets
    /// </summary>
    private float _initClockSpeed;
    public float currentClockSpeed;


    /// <summary>
    /// Starting Movement Speed
    /// </summary>
    private float _initMS = 5f;

    /// <summary>
    /// Agent curretn Movement Speed
    /// </summary>
    public float currentMS;
    public float wanderTimer;

    private Transform target;
    private NavMeshAgent agent;
    public float timer;

    public float repairTimer;
    public float reparationAmount;


    public GameObject enemyTarget;

    public Equipment currentEquipment;

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    private void Awake()
    {
        maxCurrentHP = _initHP;
        currentHP = maxCurrentHP;
        currentMS = _initMS;


    }
    protected virtual void Update()
    {

    }

    protected virtual void RepairAction()
    {
        repairTimer += Time.deltaTime;

        if (repairTimer >= 1f/currentClockSpeed)
        {
            currentHP += reparationAmount;
            repairTimer = 0;
        }
    }

    protected virtual void AttackAction()
    {

    }

    protected virtual void ChaseAction()
    {
        agent.SetDestination(enemyTarget.transform.position);
    }

    protected virtual void FleeAction()
    {
        agent.SetDestination((transform.position - enemyTarget.transform.position).normalized * currentMS);
    }

    protected virtual void WanderAction()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavmeshLocation(currentMS);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, -1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }


    public void AddArmorToEquipment(Armor armor)
    {
        currentEquipment.currentArmor = armor;
        maxCurrentHP += armor.hpUpgrade;
    }
}

[System.Serializable]
public class Equipment{

    public Weapon currentWeapon;
    public Armor currentArmor;
    public Processor currentProcessor;

    [Range(0f,1f)]
    public float weaponValue;
    [Range(0f,1f)]
    public float armorValue;
    [Range(0f,1f)]
    public float processorValue;
    
}
