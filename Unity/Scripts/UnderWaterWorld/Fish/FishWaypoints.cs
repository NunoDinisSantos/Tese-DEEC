using System.Collections;
using UnityEngine;

public class FishWaypoints : MonoBehaviour
{
    public Transform SpawnParent;
    [HideInInspector][SerializeField] private bool goingToWaypoint = false;
    [HideInInspector][SerializeField] private float arrivedWaypointThreshold = 0.5f;
    [HideInInspector][Range(100,200)] private float speed;
    [HideInInspector][SerializeField] private float rotationSpeed = 19;
    [HideInInspector][SerializeField] private int waterLevel = 100;

    [HideInInspector][SerializeField] private Transform[] Boundaries;
    [HideInInspector][SerializeField] private Vector3 Target;
    [HideInInspector] public Vector3 spawnedPosition;

    [SerializeField] private float maxFishSpeed = 200;
    [SerializeField] private float minFishSpeed = 100;

    float maxX;
    float maxZ;
    float maxY;
    float minX;
    float minY;
    float minZ;

    private bool spawned = false;

    private void OnEnable()
    {
        if (spawned)
            return;

        if(SpawnParent == null)
        {
            SpawnParent = transform.parent.parent;
        }

        spawned = true;
        minY = SpawnParent.GetChild(1).transform.position.y;
        maxY = SpawnParent.GetChild(3).transform.position.y;
        maxX = SpawnParent.GetChild(6).transform.position.x;
        minX = SpawnParent.GetChild(4).transform.position.x;
        maxZ = SpawnParent.GetChild(5).transform.position.z;
        minZ = SpawnParent.GetChild(4).transform.position.z;

        SetRandomInitialPosition();
        StartCoroutine(MoveFish());
    }

    private void OnDisable()
    {
        spawned = false;    
    }

    private void SetRandomInitialPosition()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        float z = Random.Range(minZ, maxZ);

        Vector3 newWaypoint = new Vector3(x, y, z);
        transform.position = newWaypoint;
    }

    private IEnumerator MoveFish()
    {
        if(!goingToWaypoint)
        {
            NewWaypoint();
        }

        float distance = Vector3.Distance(transform.position, Target);
        if(distance < arrivedWaypointThreshold)
        {
            goingToWaypoint = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, Target, speed*0.001f);
            RotateFish();
        }

        yield return new WaitForSeconds(0.034f);
        StartCoroutine(MoveFish());
    }

    private void NewWaypoint()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        float z = Random.Range(minZ, maxZ);

        Vector3 newWaypoint = new Vector3(x,y,z);
        
        Target = newWaypoint;
        goingToWaypoint = true;
        speed = Random.Range(minFishSpeed, maxFishSpeed);
    }

    private void RotateFish()
    {

        Vector3 newDirection = Target - transform.position;
        Quaternion rot = Quaternion.LookRotation(newDirection)*Quaternion.Euler(0, -90, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime*rotationSpeed);
    }

    private void OnDrawGizmos()
    {
        if (Target == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Target, 0.8f);  
        Gizmos.DrawLine(transform.position, Target);
    }
}
