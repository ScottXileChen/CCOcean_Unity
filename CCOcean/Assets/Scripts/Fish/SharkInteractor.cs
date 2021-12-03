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
                spawnFish.AvoidanceUnitWeight = 0f;
                spawnFish.CohesionUnitWeight = 6f;
                spawnFish.CohesionUnitDist = 10f;
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
}
