using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnFish))]
public class SardineInteractor : MonoBehaviour
{
    enum GroupBehavior
    {
        origin = 0,
        stack_and_move_around = 1,
        doge_and_swim_far = 2,
    };
    
    private float currentTime = 0;
    [SerializeField]
    private float delyTime = 0;

    private GroupBehavior currentBehavior = GroupBehavior.stack_and_move_around;
    private GroupBehavior CurrentBehavior
    {
        get => currentBehavior;
        set
        {
            currentBehavior = value;
            OnChangeBehavior(currentBehavior);
        }
    }

    private SpawnFish spawnFish = null;

    private void Start()
    {
        spawnFish = GetComponent<SpawnFish>();
        currentTime = delyTime;
    }

    //private void Update()
    //{
    //    //if (currentBehavior != GroupBehavior.origin)
    //    //{
    //    //    if (currentTime <= 0)
    //    //    {
    //    //        OnChangeBehavior(GroupBehavior.origin);
    //    //        currentTime = delyTime;
    //    //    }
    //    //    currentTime -= Time.deltaTime;
    //    //}
    //}

    private void OnChangeBehavior(GroupBehavior behavior)
    {
        switch (behavior)
        {
            case GroupBehavior.origin:
                spawnFish.CohesionUnitDist = 5;
                spawnFish.AvoidanceUnitDist = 2;
                spawnFish.AlinmentUnitDist = 2;
                spawnFish.CohesionUnitWeight =5;
                spawnFish.AvoidanceUnitWeight = 2;
                spawnFish.AlinmentUnitWeight = 5;
                break;
            case GroupBehavior.stack_and_move_around:
                spawnFish.CohesionUnitDist = 2;
                spawnFish.AvoidanceUnitDist = 3;
                spawnFish.AlinmentUnitDist = 5;
                spawnFish.CohesionUnitWeight = 1;
                spawnFish.AvoidanceUnitWeight = 3;
                spawnFish.AlinmentUnitWeight = 10;
                break;
            case GroupBehavior.doge_and_swim_far:
                transform.position += GetRandomPositionAroundFishGroup(5f);
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Rock")
        {
            int a = Random.Range(0, 1);
            CurrentBehavior = (GroupBehavior)a;
        }
    }

    private Vector3 GetRandomPositionAroundFishGroup(float radius)
    {
        return Random.insideUnitSphere * radius;
    }
}
