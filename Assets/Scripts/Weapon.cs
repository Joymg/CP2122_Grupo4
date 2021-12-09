using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/New Weapon", order = 1)]
public class Weapon : Item
{

    public GameObject model; //(?)
    public GameObject bullet;
    public float damage;
    public float range;
    public float fireRate;

    public enum WeaponType
    {
        Flamethrower = 0,
        Machinegun = 1
    }
    public WeaponType weaponType;

    public void Attack(Transform firepoint, GameObject target, GameObject self)
    {
        switch (weaponType)
        {
            case WeaponType.Flamethrower:
                GameObject.Instantiate(bullet, firepoint);

                RaycastHit[] hits = Physics.BoxCastAll(firepoint.position + (range * firepoint.forward), new Vector3(range / 2, range, range), firepoint.forward, Quaternion.LookRotation(firepoint.forward));
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject == self)
                        continue;

                    if (hit.collider.TryGetComponent<Robot>(out Robot r))
                        r.Hurt(damage);
                }

                break;
            case WeaponType.Machinegun:
                Bullet b = GameObject.Instantiate(bullet, firepoint.position, firepoint.rotation).GetComponent<Bullet>();
                b.transform.parent = null;
                b.target = target.transform;
                b.damage = damage;
                break;
            default:
                break;
        }
    }

    private void OnEnable()
    {

        //se ajustan las estadisticas para que no sobrepasen el maximo
        damage = damage > damageCap ? damageCap : damage;
        range = range > rangeCap ? rangeCap : range;
        fireRate = fireRate > fireRateCap ? fireRateCap : fireRate;

        itemType = ItemType.Weapon;
        utility = (damage / damageCap) * .4f + range / rangeCap * .3f + fireRate / fireRateCap * .3f;
    }
}




