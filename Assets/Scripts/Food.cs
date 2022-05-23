using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
  public void OnTriggerEnter(Collider col)
  {
      if (col.GetComponent<Collider>().gameObject.name == "Boid(Clone)") // Boid
        Destroy(gameObject);
  }
}
