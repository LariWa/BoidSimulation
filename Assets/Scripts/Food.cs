using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public LayerMask eaters;
    public int eatThreashold =10;
    int eatNumber=0;
    private void Update()
    {
        if (transform.position.y < 0)
            Destroy(gameObject);

    }
    public void OnTriggerEnter(Collider col)
  {
        if (eaters == (eaters | (1 << col.gameObject.layer)))// Boid
        {
            eatNumber++;
            if (eatNumber > eatThreashold)
            {
                eatThreashold = 0;
                Destroy(gameObject);
            }
        }
  }
}
