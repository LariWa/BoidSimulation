using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
  public void OnTriggerEnter(Collider col)
  {
        if (col.gameObject.layer==8) // Boid
        {
            Destroy(gameObject);
        }
  }
}
