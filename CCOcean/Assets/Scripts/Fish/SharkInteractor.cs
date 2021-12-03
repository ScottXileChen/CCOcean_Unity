using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnFish))]
[RequireComponent(typeof(WanderBehavior))]
public class SharkInteractor : MonoBehaviour
{
    enum GroupBehavior
    {
        target,
        wander
    }

    private GroupBehavior currentBehavior = GroupBehavior.wander;
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

    private WanderBehavior wanderBehavior = null;

    private void Start()
    {
        spawnFish = GetComponent<SpawnFish>();
        wanderBehavior = GetComponent<WanderBehavior>();
    }

    private void OnChangeBehavior(GroupBehavior behavior)
    {
        switch (behavior)
        {
            case GroupBehavior.target:
                wanderBehavior.enabled = false;
                spawnFish.AvoidanceUnitDist = 3f;
                spawnFish.AlinmentUnitDist = 10f;
                spawnFish.AlinmentUnitWeight = 10f;
                spawnFish.AvoidanceUnitWeight = 3f;
                spawnFish.CohesionUnitWeight = 1f;
                spawnFish.CohesionUnitDist = 1f;
                spawnFish.BoundUnitDist = 10f;
                spawnFish.BoundUnitWeight = 100f;
                for(int i=0; i<spawnFish.units.Length;++i)
                {
                    spawnFish.units[i].InitializeSpeed(5);
                }
                break;
            case GroupBehavior.wander:
                wanderBehavior.enabled = true;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Meat")
        {
            CurrentBehavior = GroupBehavior.target;
            spawnFish.transform.position = other.transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Meat" && CurrentBehavior == GroupBehavior.target)
        {
            spawnFish.transform.position = other.transform.position;
        }
    }
}
