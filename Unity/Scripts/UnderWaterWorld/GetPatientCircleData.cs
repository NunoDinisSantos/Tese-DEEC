using System.Collections.Generic;
using UnityEngine;

public class GetPatientCircleData : MonoBehaviour
{
    /*public List<Vector2> movementCoordinates = new List<Vector2>();
    public const int arraySize = 100;
    //public Vector2[] movementCoordinates = new Vector2[arraySize];
    int index = 0;

    // Sampling variables
    private float samplingInterval = 0.1f; 
    private float timeSinceLastSample = 0f;

    // Circle detection and tracking
    private Vector2 startPoint;
    private float expectedRadius = 1.0f;    

    // Metrics storage for multiple circles
    private List<float> radiusDeviations = new List<float>();
    private List<float> completionTimes = new List<float>();
    private List<float> circularityErrors = new List<float>();

    // Circle completion variables
    private float totalRadiusDeviation = 0f;   
    private float circleStartTime;
    private bool isTrackingCircle = false;
    private float circleCompletionThreshold = 0.1f;

    public int numSegments = 16; // Number of circle segments (e.g., 8 parts)
    private float segmentAngle; // Angle covered by each segment

    void StartCircle(Vector2 startPos)
    {
        startPoint = startPos;
        movementCoordinates.Clear();
        totalRadiusDeviation = 0f;
        isTrackingCircle = true;
        circleStartTime = Time.time; 
    }

    public bool TrackPlayerMovement(Vector2 currentPosition)
    {
        timeSinceLastSample += Time.deltaTime;

        if (timeSinceLastSample < samplingInterval)
        {
            return false;
        }

        timeSinceLastSample = 0f;
        //Vector2 currentPosition = new Vector2(transform.position.x, transform.position.z);

        // Calculate the angle between the player's current position and the circle center
        float angle = CalculateAngleFromCenter(currentPosition);

        // Determine which segment the player is currently in
        int currentSegment = Mathf.FloorToInt(angle / segmentAngle);

        if (!segmentsCompleted[currentSegment])
        {
            segmentsCompleted[currentSegment] = true;
            ReelFishIn(currentSegment);
        }

        if (!isTrackingCircle)
        {
            StartCircle(currentPosition);  // Start tracking a new circle
        }

        movementCoordinates.Add(currentPosition);

        // Calculate radius from the start point (assumed center)
        float currentRadius = Vector2.Distance(startPoint, currentPosition);
        totalRadiusDeviation += Mathf.Abs(currentRadius - expectedRadius);

        // Check if the player has completed a circle (returns close to the start point)
        if (Vector2.Distance(startPoint, currentPosition) < circleCompletionThreshold && movementCoordinates.Count > 10)
        {
            CompleteCircle();
        }
    }

    // Finalize the tracking of the current circle and store metrics
    void CompleteCircle()
    {
        float completionTime = Time.time - circleStartTime;  // Calculate the time taken to complete the circle
        float averageRadiusDeviation = totalRadiusDeviation / movementCoordinates.Count;

        // Calculate the error in circularity
        float circularityError = CalculateCircularityError(movementCoordinates, expectedRadius);

        // Store the calculated metrics for the circle
        radiusDeviations.Add(averageRadiusDeviation);
        completionTimes.Add(completionTime);
        circularityErrors.Add(circularityError);

        // Debug the data
        Debug.Log($"Circle completed! Time: {completionTime}, Avg Deviation: {averageRadiusDeviation}, Circularity Error: {circularityError}");

        // Reset for next circle
        ResetTracking();
    }

    // Reset tracking variables for the next circle
    void ResetTracking()
    {
        movementCoordinates.Clear();
        totalRadiusDeviation = 0f;
        isTrackingCircle = false;
    }

    // Calculate the error in circularity
    float CalculateCircularityError(List<Vector2> positions, float expectedRadius)
    {
        float circularityError = 0f;
        Vector2 center = startPoint;  // Assuming the start point is the center of the circle

        foreach (Vector2 pos in positions)
        {
            float currentRadius = Vector2.Distance(center, pos);
            float error = Mathf.Abs(currentRadius - expectedRadius);
            circularityError += error;
        }

        // Average circularity error across the entire movement
        return circularityError / positions.Count;
    }


    public DataBaseLoaderScript database;


    public string GetMovementData()
    {
        return JsonUtility.ToJson(movementCoordinates);
    }

    private void SaveDataToDatabase()
    {
        var data = JsonUtility.ToJson(movementCoordinates);
        database.CallSaveDataCircle(PlayerDataScript.playerDataInstance.PatientId, data);
    }*/
}
