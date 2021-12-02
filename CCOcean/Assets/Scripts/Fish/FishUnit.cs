using System.Collections.Generic;
using UnityEngine;

public class FishUnit : MonoBehaviour
{
    //FOV = Field of view
    [SerializeField] private float FOVangle;
    [SerializeField] private float smoothDamp;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask playerBodyMask;
    [SerializeField] private Vector3[] directionsToCheck;

    private List<FishUnit> cNeighbours = new List<FishUnit>();
    private List<FishUnit> avNeighbours = new List<FishUnit>();
    private List<FishUnit> alNeighbours = new List<FishUnit>();
    private SpawnFish getSpawnFish;
    private Vector3 currentVelocity;
    private Vector3 currentDirectionVector;
    private float speed;

    public Transform unitTransform { get; set; }

    private void Awake()
    {
        unitTransform = transform;
    }

    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
    }

    public void GetSpawnFish(SpawnFish spawnFish)
    {
        getSpawnFish = spawnFish;
    }

    public void MoveUnit()
    {
        FindNeighbors();
        var cohesionVector = CohesionCalculate() * getSpawnFish.CohesionUnitWeight;
        var avoidanceVector = AvoidanceCalculate() * getSpawnFish.AvoidanceUnitWeight;
        var alinmentVector = AlinmentCalculate() * getSpawnFish.AlinmentUnitWeight;
        var boundVector = BoundCalculate() * getSpawnFish.BoundUnitWeight;
        var obstacleVector = ObstacleCalculate() * getSpawnFish.ObstacleUnitWeight;

        var moveVector = cohesionVector + avoidanceVector + alinmentVector + boundVector + obstacleVector;
        //Vector3.SmoothDamp use for Smoothly move, common use is for smoothing a follow camera
        //c# ref is indicates that a value is passed by reference
        moveVector = Vector3.SmoothDamp(unitTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;

        if (moveVector == Vector3.zero)
            moveVector = transform.forward;

        unitTransform.forward = moveVector;
        unitTransform.position += moveVector * Time.deltaTime;
    }

    void FindNeighbors()
    {
        cNeighbours.Clear();
        alNeighbours.Clear();
        avNeighbours.Clear();
        var units = getSpawnFish.units;
        for (int i = 0; i < units.Length; ++i)
        {
            var currenUnit = units[i];
            if (currenUnit != this)
            {
                float currentNeighborDist = Vector3.SqrMagnitude(currenUnit.transform.position - transform.position);
                if (currentNeighborDist <= getSpawnFish.CohesionUnitDist * getSpawnFish.CohesionUnitDist)
                {
                    cNeighbours.Add(currenUnit);
                }
                if (currentNeighborDist <= getSpawnFish.AlinmentUnitDist * getSpawnFish.AlinmentUnitDist)
                {
                    alNeighbours.Add(currenUnit);
                }
                if (currentNeighborDist <= getSpawnFish.AvoidanceUnitDist * getSpawnFish.AvoidanceUnitDist)
                {
                    avNeighbours.Add(currenUnit);
                }
            }
        }
    }

    void CalculateSpeed()
    {
        speed = 0;
        for (int i = 0; i < cNeighbours.Count; ++i)
        {
            speed += cNeighbours[i].speed;
        }
        speed /= cNeighbours.Count;
        speed = Mathf.Clamp(speed, getSpawnFish.MinSpeed, getSpawnFish.MaxSpeed);
    }

    Vector3 CohesionCalculate()
    {
        var cohesionVector = Vector3.zero;
        if (cNeighbours.Count == 0)
            return cohesionVector;
        int neighborInFovNums = 0;
        for (int i = 0; i < cNeighbours.Count; ++i)
        {
            if (IsInFOV(cNeighbours[i].unitTransform.position))
            {
                neighborInFovNums++;
                cohesionVector += cNeighbours[i].unitTransform.position;
            }
        }
        if (neighborInFovNums == 0)
            return cohesionVector;
        //the computation vector is divided by the neighbor count, resulting is the position of the center of mass
        //we want the direction towards the center of mass, so cohesionVector -= unitTransform.position;
        //and normalize it.
        cohesionVector /= neighborInFovNums;
        cohesionVector -= unitTransform.position;
        cohesionVector = Vector3.Normalize(cohesionVector);
        return cohesionVector;
    }

    Vector3 AvoidanceCalculate()
    {
        var avoidVector = Vector3.zero;
        if (avNeighbours.Count == 0)
            return Vector3.zero;
        int neighborInFovNums = 0;
        for (int i = 0; i < avNeighbours.Count; ++i)
        {
            if (IsInFOV(avNeighbours[i].unitTransform.position))
            {
                neighborInFovNums++;
                avoidVector += (unitTransform.position - avNeighbours[i].unitTransform.position);
            }
        }
        if (neighborInFovNums == 0)
            return Vector3.zero;
        avoidVector /= neighborInFovNums;
        avoidVector = avoidVector.normalized;
        return avoidVector;
    }

    Vector3 AlinmentCalculate()
    {
        var alinmentVector = unitTransform.forward;
        if (alNeighbours.Count == 0)
            return unitTransform.forward;
        int neighborInFovNums = 0;
        for (int i = 0; i < alNeighbours.Count; ++i)
        {
            if (IsInFOV(alNeighbours[i].unitTransform.position))
            {
                neighborInFovNums++;
                alinmentVector += alNeighbours[i].unitTransform.forward;
            }
        }
        if (neighborInFovNums == 0)
            return unitTransform.forward;
        alinmentVector /= neighborInFovNums;
        alinmentVector = alinmentVector.normalized;
        return alinmentVector;
    }

    //limit fish in an area
    Vector3 BoundCalculate()
    {
        var centerOffset = getSpawnFish.transform.position - unitTransform.position;
        bool isNearCenter = (centerOffset.magnitude >= getSpawnFish.BoundUnitDist * 0.9f);
        return isNearCenter ? centerOffset.normalized : Vector3.zero;
    }

    Vector3 ObstacleCalculate()
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;
        if(Physics.Raycast(unitTransform.position,unitTransform.forward, out hit, getSpawnFish.ObstacleUnitDist, obstacleMask) || Physics.Raycast(unitTransform.position, unitTransform.forward, out hit, getSpawnFish.ObstacleUnitDist, playerBodyMask))
        {
            if (hit.transform.gameObject.name == "LeftHand")
            {
                Debug.Log("Touch");
            }
            Debug.Log(hit.transform.gameObject.name);
            obstacleVector = FindBestDirectionToAvoidObstacle();
        }
        else
        {
            currentDirectionVector = Vector3.zero;
        }
        return obstacleVector;
    }

    Vector3 FindBestDirectionToAvoidObstacle()
    {
        if (currentDirectionVector != Vector3.zero)
        {
            RaycastHit hit;
            if (!Physics.Raycast(unitTransform.position, unitTransform.forward, out hit, getSpawnFish.ObstacleUnitDist, obstacleMask) || Physics.Raycast(unitTransform.position, unitTransform.forward, out hit, getSpawnFish.ObstacleUnitDist, playerBodyMask))
            {
                return currentDirectionVector;
            }
        }
        float maxDist = int.MinValue;
        var selectdirection = Vector3.zero;
        for (int i = 0; i < directionsToCheck.Length; ++i)
        {
            RaycastHit hit;
            var direction = unitTransform.TransformDirection(directionsToCheck[i].normalized);
            if(Physics.Raycast(unitTransform.position, direction, out hit, getSpawnFish.ObstacleUnitDist, obstacleMask) || Physics.Raycast(unitTransform.position, unitTransform.forward, out hit, getSpawnFish.ObstacleUnitDist, playerBodyMask))
            {
                float currentDist = (hit.point - unitTransform.position).sqrMagnitude;
                if(currentDist > maxDist)
                {
                    maxDist = currentDist;
                    selectdirection = direction;
                }
            }
            else
            {
                selectdirection = direction;
                currentDirectionVector = direction.normalized;
                return selectdirection.normalized;
            }
        }
        return selectdirection.normalized;
    }

    bool IsInFOV(Vector3 position)
    {
        return Vector3.Angle(unitTransform.forward, position - unitTransform.position) <= FOVangle;
    }
}
