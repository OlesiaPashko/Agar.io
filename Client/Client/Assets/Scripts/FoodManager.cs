using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public Food foodPrefab;

    public static FoodManager instance;

    public List<Food> spawnedFood;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    void Start()
    {
        //StartCoroutine(InstantiateFood());
    }

    public void InstantiateFood(Vector2 position, int id)
    {
        //while (true)
        //{
        //float x = Random.Range(-50, 50);
        //float y = Random.Range(-50, 50);
        Food newFood = Instantiate(foodPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, gameObject.transform);
        newFood.id = id;
        spawnedFood.Add(newFood);
          //  yield return new WaitForSeconds(0.1f);
        //}
    }
}
