using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderBehavior : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed = 10f;
    [SerializeField]
    private float wanderRadius = 30f;
    [SerializeField]
    private float wanderDistance = 0f;
    [SerializeField]
    private float wanderJitter = 3f;

    private Vector3 localWanderTarget = Vector3.zero;
    private Vector3 oldPosition = Vector3.zero;

    private void Start()
    {
        oldPosition = transform.position;
    }

    private void Update()
    {
        transform.position += CalculateWander() * Time.deltaTime;
        oldPosition = transform.position;
    }

    public Vector3 CalculateWander()
    {
        Vector3 randomForce = Random.insideUnitSphere * wanderJitter;
        localWanderTarget += randomForce;
        localWanderTarget = localWanderTarget.normalized * wanderRadius;
        Vector3 newPosition = transform.position + localWanderTarget;
        Vector3 toTarget = newPosition - oldPosition;
        Vector3 desiredVelocity = toTarget.normalized * maxSpeed;

        return desiredVelocity;
    }
}
