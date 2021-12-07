using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(FieldOfView))]
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
    private float _initClockSpeed = .5f;
    public float currentClockSpeed;

    public float detectionRange = 10f;

    /// <summary>
    /// Starting Movement Speed
    /// </summary>
    private float _initMS = 5f;

    /// <summary>
    /// Agent current Movement Speed
    /// </summary>
    public float currentMS;
    public float wanderTimer = 2f;

    /// <summary>
    /// Indicates if the object is being under attack
    /// </summary>
    public bool underAttack = false;

    protected NavMeshAgent agent;
    private Rigidbody body;
    public float timer;

    public float repairTimer;
    public float reparationAmount = 3f;

    public float fleeCountDown = 3f;


    public GameObject enemyTarget;
    protected float danger;
    public GameObject itemTarget;

    public bool IsItemDetected => itemTarget;

    public Equipment currentEquipment;
    protected FieldOfView fov;

    protected List<Item> visitedItems = new List<Item>();

    public virtual void OnEnable()
    {
        FieldOfView.OnItemDetected += FieldOfView_OnItemDetected;
        FieldOfView.OnEnemyDetected += FieldOfView_OnEnemyDetected;
    }

    protected void FieldOfView_OnEnemyDetected(Robot robot)
    {
        if (robot.gameObject != this.gameObject) enemyTarget = robot.gameObject;
    }

    private void FieldOfView_OnItemDetected(ItemContainer itemContainer)
    {
        if (!visitedItems.Contains(itemContainer.item))
        {
            itemTarget = itemContainer.gameObject;
        }
    }

    protected void Awake()
    {
        maxCurrentHP = 100;
        currentHP = maxCurrentHP;
        currentMS = _initMS;

        currentClockSpeed = _initClockSpeed;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = currentMS;
        body = GetComponent<Rigidbody>();
        fov = GetComponent<FieldOfView>();
        timer = wanderTimer;


    }

    private void Start()
    {
        fov.UpdateViewRange(detectionRange);
    }
    protected virtual void Update()
    {
        agent.speed = currentMS;
        fov.UpdateViewRange(detectionRange);

        Vector3 direction = agent.destination - transform.position;
        body.rotation = Quaternion.LookRotation(agent.destination, Vector3.up);

        //transform.LookAt(agent.destination + Vector3.up * transform.position.y);

        //update danger
        if (enemyTarget)
        {
            danger = (1f / Vector3.Distance(enemyTarget.transform.position, transform.position)) / detectionRange;
        }
        else
        {
            danger = 0;
        }

    }

    protected void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void RepairAction()
    {
        agent.SetDestination(transform.position);
        repairTimer += Time.deltaTime;
        if (repairTimer >= 1f / currentClockSpeed)
        {
            if (currentHP < maxCurrentHP)
            {
                currentHP += reparationAmount;
                repairTimer = 0;
                currentHP = currentHP > maxCurrentHP ? maxCurrentHP : currentHP;

            }


        }
    }

    protected virtual void AttackAction()
    {
        if (enemyTarget != null)
        {
            HurtEnemy();
        }
    }

    protected virtual void ChaseAction()
    {
        Debug.Log(gameObject.name + "Chasing");
        agent.SetDestination(enemyTarget.transform.position);
    }

    protected virtual void FleeAction()
    {
        if (enemyTarget)
        {
            Vector3 dirToEnemy = transform.position - enemyTarget.transform.position;
            //enemyTarget = Vector3.Distance(enemyTarget.transform.position,transform.position) > detectionRange ? null : enemyTarget;
            Vector3 newPos = transform.position + dirToEnemy;
            agent.SetDestination(newPos);
            fleeCountDown -= Time.deltaTime;
            if (fleeCountDown <= 0f)
            {
                enemyTarget = null;
                fleeCountDown = 3f;
            }
        }
        underAttack = false;
    }

    protected virtual void WanderAction()
    {
        if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
        {
            timer += Time.deltaTime;

            if (timer >= 1f / currentClockSpeed)
            {
                Vector3 newPos = RandomNavmeshLocation(detectionRange);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }

    protected virtual void MoveToItemAction()
    {
        if (itemTarget)
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
        Robomealy robomealy;
        if (TryGetComponent<Robomealy>(out robomealy))
        {
            robomealy.PickItem();
        }

        //Item item = itemTarget.GetComponent<Item>();
        switch (item.itemType)
        {
            case ItemType.Armor:
                if (!currentEquipment.armor)
                {
                    AddArmorToEquipment((Armor)item);
                    itemTarget = null;
                    return true;
                }
                else
                {
                    if (item.utility > currentEquipment.armor.utility)
                    {
                        AddArmorToEquipment((Armor)item);
                        itemTarget = null;
                        return true;
                    }
                    else
                    {
                        visitedItems.Add(item);
                        return false;
                    }
                }
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
                        itemTarget = null;
                        return true;
                    }
                    else
                    {
                        visitedItems.Add(item);
                        itemTarget = null;
                        return false;
                    }
                }
            case ItemType.Weapon:
                if (!currentEquipment.weapon)
                {
                    AddWeaponToEquipment((Weapon)item);
                    itemTarget = null;
                    return true;
                }
                else
                {

                    if (item.utility > currentEquipment.weapon.utility)
                    {
                        AddWeaponToEquipment((Weapon)item);
                        itemTarget = null;
                        return true;
                    }
                    else
                    {
                        visitedItems.Add(item);
                        itemTarget = null;
                        return false; ;
                    }
                }

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
        detectionRange += newProcessor.detectionRangeBoost;
        fov.UpdateViewRange(detectionRange);
    }

    public float GetHp()
    {
        return currentHP;
    }

    public Equipment GetEquipment()
    {
        return currentEquipment;
    }

    public void HurtEnemy()
    {
        if (enemyTarget.TryGetComponent<Robot>(out Robot r))
        {
            r.currentHP -= GetEquipment().weapon.damage;
            r.underAttack = true;
        }
    }
}

[System.Serializable]
public class Equipment
{

    public Weapon weapon;
    public Armor armor;
    public Processor processor;

    [Range(0f, 1f)]
    public float weaponValue;
    [Range(0f, 1f)]
    public float armorValue;
    [Range(0f, 1f)]
    public float processorValue;


    public bool CheckIfItemIsBetterThanCurrent(GameObject item)
    {
        Weapon w;
        if (item.TryGetComponent<Weapon>(out w))
        {
            return w.utility > weapon.utility;
        }

        Armor a;
        if (item.TryGetComponent<Armor>(out a))
        {
            return a.utility > armor.utility;
        }

        Processor p;
        if (item.TryGetComponent<Processor>(out p))
        {
            return p.utility > processor.utility;
        }

        return false;
    }
}




