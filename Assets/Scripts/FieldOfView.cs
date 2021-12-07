using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask enemyAndItemTargetMask;
    //public LayerMask itemTargetMask;
    public LayerMask obstacleMask;

    //[HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public event Action<ItemContainer> OnItemDetected;

    public event Action<Robot> OnEnemyDetected;

    private void Start()
    {
        StartCoroutine("FindTargetsWithDelay", 0f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position,viewRadius, enemyAndItemTargetMask);

       

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle/2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position,dirToTarget,dstToTarget,obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }

        visibleTargets.Remove(transform);

        CheckPriorities();
    }

    private void CheckPriorities()
    {
        foreach (Transform visibleTarget in visibleTargets)
        {
            if (visibleTarget.gameObject.TryGetComponent<Robot>(out Robot robot))
            {
                OnEnemyDetected?.Invoke(robot);
            }
            else if (visibleTarget.gameObject.TryGetComponent<ItemContainer>(out ItemContainer itemContainer))
            {
                OnItemDetected?.Invoke(itemContainer);
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void UpdateViewAngle(float newAngle)
    {
        viewAngle = newAngle;
    }

    public void UpdateViewRange(float newRange)
    {
        viewRadius = newRange;
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
        Vector3 ViewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 ViewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + ViewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + ViewAngleB * fow.viewRadius);

        foreach (Transform visibleTarget in fow.visibleTargets)
        {
            if (visibleTarget.gameObject.TryGetComponent<Robot>(out Robot robot))
            {
                Handles.color = Color.black;
                Handles.DrawLine(fow.transform.position, visibleTarget.position);
            }
            else if(visibleTarget.gameObject.TryGetComponent<ItemContainer>(out ItemContainer itemContainer))
            {
                Handles.color = Color.blue;
                Handles.DrawLine(fow.transform.position, visibleTarget.position);
            }

        }
    }
}
#endif
