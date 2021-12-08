using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public Transform target;
    // Update is called once per frame
    void FixedUpdate()
    {
        /* if (target)
             transform.LookAt(target);*/
        transform.parent = null;
        transform.Translate(transform.forward * speed * Time.fixedDeltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        Destroy(gameObject);

    }
}
