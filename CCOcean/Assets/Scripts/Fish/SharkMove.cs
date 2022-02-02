using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkMove : MonoBehaviour
{
    [SerializeField] private float smoothDamp;
    private SpawnFish getSpawnFish;
    private Vector3 currentVelocity;
    private Vector3 currentDirectionVector;
    private float speed = 5.0f;
    // Start is called before the first frame update
    public Transform unitTransform { get; set; }
    private void Awake()
    {
        unitTransform = transform;
    }

    public void GetSpawnFish(SpawnFish spawnFish)
    {
        getSpawnFish = spawnFish;
    }
    // Update is called once per frame
    public void MoveUnit()
    {
        var centerOffset = getSpawnFish.transform.position - unitTransform.position;
        bool isNearCenter = (centerOffset.magnitude >= getSpawnFish.BoundUnitDist * 0.9f);
        var boundVector= isNearCenter ? centerOffset.normalized : Vector3.zero; 
        boundVector*= getSpawnFish.BoundUnitWeight;


        var moveVector = boundVector;
        //Vector3.SmoothDamp use for Smoothly move, common use is for smoothing a follow camera
        //c# ref is indicates that a value is passed by reference
        moveVector = Vector3.SmoothDamp(unitTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;

        if (moveVector == Vector3.zero)
            moveVector = transform.forward;

        unitTransform.forward = moveVector;
        unitTransform.position += moveVector * Time.deltaTime;
    }
}
