using System.Collections;
using UnityEngine;

public class FishWaypoints : MonoBehaviour
{
    public Transform SpawnParent;
    [SerializeField] private bool goingToWaypoint = false;
    [SerializeField] private float arrivedWaypointThreshold = 0.5f;
    [SerializeField][Range(100,300)] private float speed;
    [SerializeField] private float rotationSpeed = 19;
    [SerializeField] private int waterLevel = 100;

    [SerializeField] private Transform[] Boundaries;
    [SerializeField] private Vector3 Target;
    public Vector3 spawnedPosition;

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

        spawned = true;
        //int indexSpawn = SpawnParent.transform.childCount - 2;
        //SpawnParent = SpawnParent.GetChild(Random.Range(0,indexSpawn));
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
        speed = Random.Range(100, 300);
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
