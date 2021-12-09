using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 1f;
    public float speed = 10f;
    public Transform target;
    public GameObject impactParticles;

    Vector3 _speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        /* if (target)
             transform.LookAt(target);
         transform.parent = null;
         transform.Translate(transform.forward * speed * Time.fixedDeltaTime);*/

        if (target)
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref _speed, 1 / speed);
        else
            transform.Translate(_speed * Time.fixedDeltaTime);

        if (transform.position.sqrMagnitude > 50)
            Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (target && other.gameObject != target.gameObject)
            return;

        if (other.TryGetComponent<Robot>(out Robot robot))
            robot.Hurt(damage);

        if (impactParticles)
            GameObject.Instantiate(impactParticles, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
