using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFish : MonoBehaviour
{
    enum GroupBehavior
    {
        stack_and_move_around = 0,
        doge_and_swim_far = 1,
    };

    //spawn setup
    [SerializeField] private FishUnit unitGameObject;
    [SerializeField] private int spawnNum;
    [SerializeField] private Vector3 SpawnArea;

    //speed
    [Range(0,10)]
    [SerializeField] private float minSpeed;
    [Range(0, 10)]
    [SerializeField] private float maxSpeed;

    // Detection Dist
    [Range(0, 10)]
    [SerializeField] private float cohesionUnitDist;
    [Range(0, 10)]
    [SerializeField] private float avoidanceUnitDist;
    [Range(0, 10)]
    [SerializeField] private float alinmentUnitDist;
    [Range(0, 100)]
    [SerializeField] private float boundUnitDist;
    [Range(0, 10)]
    [SerializeField] private float obstacleUnitDist;

    //behaviour weight
    [Range(0, 10)]
    [SerializeField] private float cohesionUnitWeight;
    [Range(0, 10)]
    [SerializeField] private float avoidanceUnitWeight;
    [Range(0, 10)]
    [SerializeField] private float alinmentUnitWeight;
    [Range(0, 10)]
    [SerializeField] private float boundUnitWeight;
    [Range(0, 100)]
    [SerializeField] private float obstacleUnitWeight;

    public FishUnit[] units { get; set; }
    public float MinSpeed { get => minSpeed; set => minSpeed = value; }
    public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
    public float CohesionUnitDist { get => cohesionUnitDist; set => cohesionUnitDist = value; }
    public float AvoidanceUnitDist { get => avoidanceUnitDist; set => avoidanceUnitDist = value; }
    public float AlinmentUnitDist { get => alinmentUnitDist; set => alinmentUnitDist = value; }
    public float CohesionUnitWeight { get => cohesionUnitWeight; set => cohesionUnitWeight = value; }
    public float AvoidanceUnitWeight { get => avoidanceUnitWeight; set => avoidanceUnitWeight = value; }
    public float AlinmentUnitWeight { get => alinmentUnitWeight; set => alinmentUnitWeight = value; }
    public float BoundUnitDist { get => boundUnitDist; set => boundUnitDist = value; }
    public float BoundUnitWeight { get => boundUnitWeight; set => boundUnitWeight = value; }
    public float ObstacleUnitDist { get => obstacleUnitDist; set => obstacleUnitDist = value; }
    public float ObstacleUnitWeight { get => obstacleUnitWeight; set => obstacleUnitWeight = value; }

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

    void Start()
    {
        UnitsSpawn();
    }

    private void Update()
    {
        for(int i = 0; i< units.Length;++i)
        {
            units[i].MoveUnit();
        }
    }

    void UnitsSpawn()
    {
        units = new FishUnit[spawnNum];
        for(int i =0;i<spawnNum;++i)
        {
            //random position in sphere
            var randomPosition = Random.insideUnitSphere;
            randomPosition = new Vector3(randomPosition.x * SpawnArea.x, randomPosition.y * SpawnArea.y, randomPosition.z * SpawnArea.z);
            var spawnPosition = transform.position + randomPosition;
            var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            units[i] = Instantiate(unitGameObject, spawnPosition, rotation);
            units[i].GetSpawnFish(this);
            units[i].InitializeSpeed(Random.Range(MinSpeed, MaxSpeed));
        }
    }

    private void OnChangeBehavior(GroupBehavior behavior)
    {
        switch (behavior)
        {
            case GroupBehavior.stack_and_move_around:
                CohesionUnitDist = 2;
                AvoidanceUnitDist = 3;
                alinmentUnitDist = 5;
                cohesionUnitWeight = 1;
                avoidanceUnitWeight = 3;
                alinmentUnitWeight = 10;
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
