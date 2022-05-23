using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGenerator : MonoBehaviour
{
    public float length = 18;
    public float width = 18;
    public float height = 7;
    public GameObject foodPrefab;

    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
      generateFood();
    }

    void Update()
    {
      count = GameObject.FindGameObjectsWithTag("Goal").Length;
      if (count == 0)
        generateFood();
    }

    public void generateFood()
    {
      Vector3 pos = new Vector3(Random.Range(-length/2, length/2), Random.Range(0, height), Random.Range(-width/2, width/2));
      GameObject food = Instantiate (foodPrefab);
      food.transform.position = pos;
    }
}
