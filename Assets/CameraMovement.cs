using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement instance;

    public float distanceToTarget = 20;
    public float cameraHeight = 20;
    Vector3 offset;

    Vector3 currentRotation;
    public float smoothTime = 0.2f;
    private Vector3 smoothVelocity = Vector3.zero;
    private Vector3 smoothRotationVelocity = Vector3.zero;

    Vector3 defaultPos;
    Quaternion defaultRot;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        offset = new Vector3(distanceToTarget, cameraHeight, -distanceToTarget);
        defaultPos = transform.position;
        defaultRot = transform.rotation;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentRotation = transform.eulerAngles;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        Robot currentRobot = GameManager.instance.GetCurrenRobot();

        if (!currentRobot)
        {
            transform.position = Vector3.SmoothDamp(transform.position, defaultPos, ref smoothVelocity, smoothTime);
            currentRotation = Vector3.SmoothDamp(currentRotation, defaultRot.eulerAngles, ref smoothRotationVelocity, smoothTime);
            transform.localEulerAngles = currentRotation;
            return;
        }

        transform.position = Vector3.SmoothDamp(transform.position, currentRobot.transform.position + offset, ref smoothVelocity, smoothTime);

        Vector3 dir = (currentRobot.transform.position - transform.position).normalized;
        Vector3 nextRotation = Quaternion.LookRotation(dir, Vector3.up).eulerAngles;
        currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothRotationVelocity, smoothTime);

        transform.localEulerAngles = currentRotation;
    }


    private void OnValidate()
    {
        offset = new Vector3(distanceToTarget, cameraHeight, -distanceToTarget);
    }
}
