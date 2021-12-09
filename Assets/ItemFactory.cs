using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemFactory : MonoBehaviour
{
    public static ItemFactory instance;
    public GameObject itemContainerPrefab;
    public int numberOfItemToSpawn;
    public int radius;
    public Item[] items;

    private void Awake()
    {
        if (instance)
            Destroy(gameObject);
        else
            instance = this;
    }

    public void CreateItems()
    {
        for (int i = 0; i < numberOfItemToSpawn; i++)
        {
            Vector2 pos2D = radius * Random.insideUnitCircle;
            Vector3 pos = new Vector3(pos2D.x, 0, pos2D.y);
            ItemContainer itemContainer = GameObject.Instantiate(itemContainerPrefab, pos, Random.rotationUniform).GetComponent<ItemContainer>();
            int id = Random.Range(0, items.Length);
            itemContainer.item = items[id];
        }
    }

    public void DropItem(Item item, Vector3 pos)
    {
        if (!item)
            return;

        ItemContainer itemContainer = GameObject.Instantiate(itemContainerPrefab, pos, Random.rotationUniform).GetComponent<ItemContainer>();
        itemContainer.item = item;
    }

    public void SpawnOnLocation(Vector3 location, Item item)
    {
        ItemContainer itemContainer = GameObject.Instantiate(itemContainerPrefab, location, Random.rotationUniform).GetComponent<ItemContainer>();
        itemContainer.item = item;
    }
}
