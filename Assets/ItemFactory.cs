using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    public GameObject itemContainerPrefab;
    public int numberOfItemToSpawn;
    public int radius;
    public Item[] items;


    void Start()
    {
        for (int i = 0; i < numberOfItemToSpawn; i++)
        {
            Vector2 pos2D = radius * Random.insideUnitCircle;
            Vector3 pos = new Vector3(pos2D.x, 0, pos2D.y);
            ItemContainer itemContainer =  GameObject.Instantiate(itemContainerPrefab, pos, Random.rotationUniform).GetComponent<ItemContainer>();
            int id = Random.Range(0, items.Length);
            itemContainer.item = items[id];
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
