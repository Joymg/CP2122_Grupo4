using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFactory : MonoBehaviour
{
    public int numberOfRobotsToSpawn;
    public int radius;
    public GameObject[] robots;

    public void CreateRobots()
    {
        int totalRobots = numberOfRobotsToSpawn * robots.Length;
        float angle = (2 * Mathf.PI) / totalRobots;
        for (int i = 0; i < totalRobots; i++)
        {
            /*Vector2 pos2D = radius * Random.insideUnitCircle;
            Vector3 pos = new Vector3(pos2D.x, 0, pos2D.y);*/

            Vector3 pos = new Vector3(radius * Mathf.Cos(i * angle), 0, radius * Mathf.Sin(i * angle));

            int id = Random.Range(0, robots.Length);
            GameObject.Instantiate(robots[id], pos, Quaternion.LookRotation(-transform.position, Vector3.up)).GetComponent<ItemContainer>();
        }
    }
}
