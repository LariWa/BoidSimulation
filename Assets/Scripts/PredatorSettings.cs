using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PredatorSettings : ScriptableObject {
    // Settings
    public float minSpeed = 2;
    public float maxSpeed = 3;
    public float maxSteerForce = 3;



    [Header ("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 20;
    public float collisionAvoidDst = 3;

   
}