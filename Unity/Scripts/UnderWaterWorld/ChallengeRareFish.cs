using System.Collections;
using UnityEngine;

public class ChallengeRareFish : MonoBehaviour
{
    [SerializeField] private GameObject[] RareFishPrefabs;
    [Header("Safe 0 | Coral 1 | Ice 2 | Ship 3 | Lava 4 | Base 5 | Temple 6")]
    [SerializeField] private Transform[] RareFishTransforms;
    [SerializeField] private Transform[] RareFishTransformsParents;

    private int fishIndex = -1;
    private int challengeType = -1;
    private int zoneIndex = -1;

    [SerializeField] private DataBaseLoaderScript db;

    [SerializeField] private bool RareFishChallenge = false;
    [SerializeField] private bool spawned = false;

    [SerializeField] private int maxProbabilityNumber = 3000;
    [SerializeField] private int jackpotNumber = 500;

    [SerializeField] private bool IAmACheater = false;

    [SerializeField] private GameObject FishDetectedWarning;
    void Start()
    {
        //SetValues();
        StartCoroutine("WaitForData");
    }

    IEnumerator WaitForData()
    {
        yield return new WaitForSeconds(10);
        SetValues();
    }

    private void SetValues()
    {
        challengeType = db.ChallengeType;
        Debug.Log("Loading challenge: " + challengeType);

        if (challengeType != 14)
        {
            return;
        }

        else
        {
            RareFishChallenge = true;
            zoneIndex = db.FishZoneIndex; // get from db
            fishIndex = db.FishIndex; // get from db
            Debug.Log("Setting values for rare fish: " + fishIndex + " " + zoneIndex);

            StartCoroutine("MyUpdate");
        }
    }

    private void InstantiateFish()
    {
        spawned = true;
        int volumeIndex = Random.Range(0, RareFishTransformsParents[zoneIndex].parent.childCount - 1);
        Transform volumeTransform = RareFishTransformsParents[zoneIndex].parent.GetChild(volumeIndex);
        var fish = Instantiate(RareFishPrefabs[fishIndex], RareFishTransforms[zoneIndex].position, Quaternion.identity, volumeTransform);
        fish.GetComponent<FishWaypoints>().SpawnParent = volumeTransform;
        fish.GetComponent<FishWaypoints>().enabled = true;
        FishDetectedWarning.SetActive(true);

        StartCoroutine("RareFishWarning");
    }

    IEnumerator RareFishWarning()
    {
        yield return new WaitForSeconds(5);
        FishDetectedWarning.SetActive(false);
    }

    IEnumerator MyUpdate()
    {
        int rareFishProbability = Random.Range(0, maxProbabilityNumber);
        
        if( rareFishProbability == jackpotNumber || IAmACheater)
        {
            InstantiateFish();
        }

        if (!spawned)
        {
            yield return new WaitForSeconds(1);
            StartCoroutine("MyUpdate");
        }
    }
}
