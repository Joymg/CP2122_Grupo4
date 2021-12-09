
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(FieldOfView))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class Robot : MonoBehaviour
{
    protected String debugText = "";


    /// <summary>
    /// Starting Health points 
    /// </summary>
    private float _initHP = 100f;

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

    protected NavMeshAgent agent;
    private Rigidbody body;
    public float timer = 0;

    public float repairTimer = 0;
    public float reparationAmount = 3f;

    public float fleeCountDown = 3f;

    [SerializeField]
    protected float attackTimer = 0;

    protected GameObject cannon;
    protected Transform firePoint;

    [Header("Particles")]
    public GameObject smoke;
    public GameObject deathParticles;

    [Header("Targets")]
    public GameObject enemyTarget;
    protected float danger;
    public GameObject itemTarget;

    public bool IsItemDetected => itemTarget;

    public Equipment currentEquipment;
    protected FieldOfView fov;

    protected List<Item> visitedItems = new List<Item>();

    protected bool Alive => currentHP > 0;
    public bool dead = false;

    public virtual void OnEnable()
    {
        fov.OnItemDetected += FieldOfView_OnItemDetected;
        fov.OnEnemyDetected += FieldOfView_OnEnemyDetected;
    }

    protected void FieldOfView_OnEnemyDetected(Robot robot)
    {
        if (!enemyTarget)
        {
            if (robot.gameObject != this.gameObject)
            {
                enemyTarget = robot.gameObject;
            }
        }

    }


    private void FieldOfView_OnItemDetected(ItemContainer itemContainer)
    {
        if (!itemTarget)
        {
            if (!visitedItems.Contains(itemContainer.item))
            {
                itemTarget = itemContainer.gameObject;
            }
        }
    }

    protected virtual void Awake()
    {
        maxCurrentHP = _initHP;
        currentHP = maxCurrentHP;
        currentMS = _initMS;

        currentClockSpeed = _initClockSpeed;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = currentMS;
        body = GetComponent<Rigidbody>();
        fov = GetComponent<FieldOfView>();
        timer = wanderTimer;

        cannon = transform.GetChild(0).gameObject;
        firePoint = cannon.transform.GetChild(0);
        cannon.SetActive(false);
    }

    private void Start()
    {
        fov.UpdateViewRange(detectionRange);
    }

    protected virtual void Update()
    {
        if (Alive)
        {
            agent.speed = currentMS;
            fov.UpdateViewRange(detectionRange);

            Vector3 direction = agent.destination - transform.position;
            body.rotation = Quaternion.LookRotation(agent.destination, Vector3.up);

            //transform.LookAt(agent.destination + Vector3.up * transform.position.y);
        }
    }


    protected void Die()
    {
        if (currentEquipment.weapon)
        {
            ItemFactory.instance.SpawnOnLocation(transform.position, currentEquipment.weapon);
        }
        Instantiate(deathParticles, transform.position, transform.rotation);
        dead = true;
        Destroy(gameObject);
    }

    protected virtual void RepairAction()
    {
        body.velocity = Vector3.zero;

        if (Time.time > repairTimer)
        {
            if (currentHP < maxCurrentHP)
            {
                currentHP += reparationAmount;
                repairTimer = Time.time + (1f / currentClockSpeed);
                currentHP = currentHP > maxCurrentHP ? maxCurrentHP : currentHP;
            }
        }
    }

    protected virtual void AttackAction()
    {
        if (currentEquipment.weapon == null)
        {
            Debug.Log("Can't attack without a weapon");
            return;
        }

        if (Time.time > attackTimer)
        {
            attackTimer = Time.time + (1 / currentEquipment.weapon.fireRate);
            if (!enemyTarget)
                return;
            transform.LookAt(enemyTarget.transform.position, Vector3.up);
            currentEquipment.weapon.Attack(firePoint, enemyTarget, gameObject);
            //HurtEnemy();
        }
    }

    protected virtual void ChaseAction()
    {
        if (enemyTarget)
        {
            if (agent)
                agent.SetDestination(enemyTarget.transform.position);
            if (currentEquipment.weapon != null && Vector3.Distance(enemyTarget.transform.position, transform.position) <= currentEquipment.weapon.range)
            {
                AttackAction();
            }
        }
    }

    protected virtual void FleeAction()
    {
        if (enemyTarget)
        {
            Vector3 dirToEnemy = transform.position - enemyTarget.transform.position;
            //enemyTarget = Vector3.Distance(enemyTarget.transform.position,transform.position) > detectionRange ? null : enemyTarget;
            Vector3 newPos = transform.position + dirToEnemy;
            if (agent)
                agent.SetDestination(newPos);
            fleeCountDown -= Time.fixedDeltaTime;
            if (fleeCountDown <= 0f)
            {
                enemyTarget = null;
                fleeCountDown = 3f;
            }
        }
    }

    protected virtual void WanderAction()
    {
        /*if (agent.pathStatus != NavMeshPathStatus.PathComplete || agent.remainingDistance > 0.5f)
            return;*/

        if (agent)
        {
            if (Time.time > timer && agent.remainingDistance < 0.5f)
            {
                enemyTarget = null;
                itemTarget = null;

                timer = Time.time + (1f / wanderTimer);
                Vector3 newPos = RandomNavmeshLocation(detectionRange);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }

    protected virtual void MoveToItemAction()
    {
        if (dead)
            return;

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
                    cannon.SetActive(true);
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
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
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
    public void SetHP(float newHP)
    {
        currentHP = newHP;
    }

    //No es necesario, la variable current equipment es publica, 
    public Equipment GetEquipment()
    {
        return currentEquipment;
    }

    public void HurtEnemy()
    {
        //TODO: Añadir animacion de disparo
        if (enemyTarget.TryGetComponent<Robot>(out Robot r))
        {
            r.SetHP(r.GetHp() - currentEquipment.weapon.damage);
        }
    }

    public void Hurt(float damage)
    {
        currentHP -= damage;
    }

    private void OnDrawGizmos()
    {
        GUI.color = Color.black;
        Handles.Label(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), debugText);
    }

    protected bool CheckIfBetter(Robot other)
    {
        return currentEquipment.weapon && !CheckIfLowHealth() &&
            (other.CheckIfLowHealth() ||
            currentEquipment.IsBetterThan(other.GetEquipment()));
    }

    public bool CheckIfLowHealth()
    {
        return currentHP < (maxCurrentHP * 0.25f);
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

    private float minDifferenceToBeConsideredWorse = 1;

    public bool IsBetterThan(Equipment other)
    {
        if (weaponValue == 0)
            return false;

        if (other.weaponValue == 0)
            return true;

        float value = weaponValue + armorValue + processorValue;
        float otherValue = other.weaponValue + other.armorValue + other.processorValue;

        float difference = value - otherValue;

        if (value < -minDifferenceToBeConsideredWorse)
            return false;

        return true;
    }


}




