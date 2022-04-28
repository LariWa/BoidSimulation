using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Boid : MonoBehaviour {

    BoidSettings settings;

    // State
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    Vector3 velocity;

    // To update:
    Vector3 acceleration;
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    //[HideInInspector]
    public int numPerceivedFlockmates;
    public float totalFear=0;
    public float fear;

    // Cached
    Material material;
    Transform cachedTransform;
    Transform target;

    //Threat Behaviour
    bool isThreatNearBy;
    List<Collider> threatColliders = new List<Collider>();

    void Awake () {
        material = transform.GetComponentInChildren<MeshRenderer> ().material;
        cachedTransform = transform;       
    }

    public void Initialize (BoidSettings settings, Transform target) {
        this.target = target;
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;

        GetComponent<SphereCollider>().radius =settings.threatDetectionRadius;

    }

    public void SetColour (Color col) {
        if (material != null) {
            material.color = col;
        }
    }

    public void UpdateBoid () {
        Vector3 acceleration = Vector3.zero;
        fear = 0;
        if (target != null) {
            Vector3 offsetToTarget = (target.position - position);
            acceleration = SteerTowards (offsetToTarget) * settings.targetWeight;
        }

        if (numPerceivedFlockmates != 0) {
            centreOfFlockmates /= numPerceivedFlockmates;
            totalFear /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            var alignmentForce = SteerTowards (avgFlockHeading) * settings.alignWeight;
            var cohesionForce = SteerTowards (offsetToFlockmatesCentre) * (settings.cohesionWeight + getCohesionThreatWeight());
            var seperationForce = SteerTowards (avgAvoidanceHeading) * settings.seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        if (IsHeadingForCollision ()) {
            Vector3 collisionAvoidDir = ObstacleRays ();
            Vector3 collisionAvoidForce = SteerTowards (collisionAvoidDir) * settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }
        else if (isThreatNearBy)
        {
            float currentFear = totalFear / 2;
            Vector3 average = Vector3.zero;
            foreach (Collider collider in threatColliders)
            {
                average += collider.ClosestPoint(cachedTransform.position) - cachedTransform.position;
                currentFear += Mathf.Clamp(1-Vector3.Distance(collider.ClosestPoint(cachedTransform.position), cachedTransform.position) / settings.threatDetectionRadius, 0, 1);

            }
            fear = currentFear;
            Vector3 threatDirection = average / threatColliders.Count;
            Vector3 threatAvoidForce = SteerTowards(-threatDirection) * settings.avoidThreatWeight * (fear);
            acceleration += threatAvoidForce;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        if (isThreatNearBy)
        {
            speed = Mathf.Clamp(speed, settings.minSpeed, settings.escapeSpeed);
        }
        else
            speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }
   
        float getCohesionThreatWeight()
    {

        return totalFear * settings.cohesionThreatWeight;
    }
    bool IsHeadingForCollision () {
        RaycastHit hit;
        if (Physics.SphereCast (position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        } else { }
        return false;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))
            {
                return dir;
            }
        }
        Debug.Log("no way out found");
        return -forward;
    }

    Vector3 SteerTowards (Vector3 vector) {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude (v, settings.maxSteerForce);
    }



    //Collision Detection for Threat Behaviour
    public void OnTriggerEnter(Collider collider)
    {
        //if object collided with is a Threat
        if (isLayerInMask(settings.threatMask, collider.gameObject.layer))
        {
            isThreatNearBy = true;
            if(!threatColliders.Contains(collider))
            threatColliders.Add(collider);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (isLayerInMask(settings.threatMask, collider.gameObject.layer))
        {
            if (threatColliders.Contains(collider))
                threatColliders.Remove(collider);
        }
    }
    public bool isLayerInMask(LayerMask layermask, int layer)
    {
        return layermask == (layermask | (1 << layer));
    }
}