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

    public float detectionRange;

    /// <summary>
    /// Starting Movement Speed
    /// </summary>
    private float _initMS = 5f;

    /// <summary>
    /// Agent curretn Movement Speed
    /// </summary>
    public float currentMS;
    public float wanderTimer =2f;

    private Transform target;
    private NavMeshAgent agent;
    private Rigidbody body;
    public float timer;

    public float repairTimer;
    public float reparationAmount;


    public GameObject enemyTarget;
    public GameObject itemTarget;

    public bool IsItemDetected => itemTarget;

    public Equipment currentEquipment;

    List<Item> visitedItems = new List<Item>();

    void OnEnable()
    {
        
    }

    private void Awake()
    {
        maxCurrentHP = _initHP;
        currentHP = maxCurrentHP;
        currentMS = _initMS;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = currentMS;
        body = GetComponent<Rigidbody>();
        timer = wanderTimer;


    }
    protected virtual void Update()
    {
        agent.speed = currentMS;

        Vector3 direction = agent.destination - transform.position;
        body.rotation = Quaternion.LookRotation(agent.destination, Vector3.up);
        
    }

    protected void Die()
    {
        Destroy(gameObject);
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
        Vector3 dirToEnemy = transform.position - enemyTarget.transform.position;
        Vector3 newPos = transform.position + dirToEnemy;

        agent.SetDestination(newPos);
    }

    protected virtual void WanderAction()
    {
        timer += Time.deltaTime;

        if (timer >= 1f/currentClockSpeed)
        {
            Vector3 newPos = RandomNavmeshLocation(currentMS);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    protected virtual void MoveToItemAction()
    {
        if (itemTarget )
        {
            if (!visitedItems.Contains(itemTarget.GetComponent<ItemContainer>().item))
            {
                agent.SetDestination(itemTarget.transform.position);

            }
            else
            {
                itemTarget = null;
            }
        }
    }

    public virtual bool GetItemAction(Item item)
    {
        //Item item = itemTarget.GetComponent<Item>();
        switch (item.itemType)
        {
            case ItemType.Armor:
                if (!currentEquipment.armor)
                {
                    AddArmorToEquipment ((Armor)item);
                    Destroy(itemTarget);
                    return true;
                }
                else
                {
                    if (item.utility > currentEquipment.armor.utility)
                    {
                        AddArmorToEquipment((Armor)item);
                        return true;
                    }
                    else
                    {
                        visitedItems.Add(item);
                        return false;
                    }
                }
                break;
            case ItemType.Processor:
                if (!currentEquipment.processor)
                {
                    AddProcessorToEquipment((Processor)item);
                    return true;
                }
                else
                {
                    if (item.utility > currentEquipment.processor.utility)
                    {
                        AddProcessorToEquipment((Processor)item);
                        return true;
                    }
                    else
                    {
                        visitedItems.Add(item);
                        return false;
                    }
                }
                break;
            case ItemType.Weapon:
                if (!currentEquipment.weapon)
                {
                    AddWeaponToEquipment((Weapon)item);
                    return true;
                }
                else
                {

                    if (item.utility > currentEquipment.weapon.utility)
                    {
                        AddWeaponToEquipment((Weapon)item);
                        return true;
                    }
                    else
                    {
                        visitedItems.Add(item);
                        itemTarget = null;
                        return false; ;
                    }
                }
                break;

        }
        return false;
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


    public void AddArmorToEquipment(Armor newArmor)
    {
        currentEquipment.armor = newArmor;
        currentEquipment.armorValue = newArmor.utility;
        maxCurrentHP += newArmor.hpBoost;
    }

    public void AddWeaponToEquipment(Weapon newWeapon)
    {
        currentEquipment.weapon = newWeapon;
        currentEquipment.weaponValue = newWeapon.utility;
    }

    public void AddProcessorToEquipment(Processor newProcessor)
    {
        currentEquipment.processor = newProcessor;
        currentEquipment.processorValue = newProcessor.utility;
        currentClockSpeed += newProcessor.clockSpeedBoost;
    }


}

[System.Serializable]
public class Equipment{

    public Weapon weapon;
    public Armor armor;
    public Processor processor;

    [Range(0f,1f)]
    public float weaponValue;
    [Range(0f,1f)]
    public float armorValue;
    [Range(0f,1f)]
    public float processorValue;
    
}
