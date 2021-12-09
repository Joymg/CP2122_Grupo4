using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Robot> robots = new List<Robot>();
    public int currentRobotId = 0;
    public Robot currentRobot;

    public GameObject cursor;

    [Range(0f, 50f)]
    public float gameSpeed = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        Time.timeScale = gameSpeed;

        if (!cursor)
            cursor = GameObject.CreatePrimitive(PrimitiveType.Cube);

        cursor.SetActive(false);
    }

    private void LateUpdate()
    {
        /*if (Input.GetButtonDown("Fire1"))
        {
            Next();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Previous();
        }*/

        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.TryGetComponent<Robot>(out Robot robot))
                {
                    currentRobotId = robots.IndexOf(robot);
                    currentRobot = robot;
                    cursor.SetActive(true);
                }
            }
        }

        if (Input.GetButtonDown("Fire2")){
            currentRobot = null;
            cursor.SetActive(false);
        }

        if (currentRobot)
            cursor.transform.position = currentRobot.transform.position + Vector3.up;

        if (robots.Count <= 1)
        {
            FindObjectOfType<RobotFactory>().CreateRobots();
            FindObjectOfType<ItemFactory>().CreateItems();
            currentRobotId = 0;
        }

        Time.timeScale = gameSpeed;
    }

    public void Next()
    {
        currentRobotId++;
        if (currentRobotId >= robots.Count)
            currentRobotId = 0;
    }

    public void Previous()
    {
        currentRobotId--;
        if (currentRobotId < 0)
            currentRobotId = robots.Count - 1;
    }

    public void AddRobot(Robot r)
    {
        robots.Add(r);
    }

    public void RemoveRobot(Robot r)
    {
        bool removingCurrentRobot = r == currentRobot;

        robots.Remove(r);
        if (removingCurrentRobot)
        {
            Debug.Log(";(");
            currentRobot = null;
            cursor.SetActive(false);
        }
    }

    public Robot GetCurrenRobot()
    {
        return currentRobot;
    }
}
