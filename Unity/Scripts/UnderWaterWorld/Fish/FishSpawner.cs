using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private FishControllerScript fishControllerScript;
    public bool currentSpawning = true;
    [HideInInspector][SerializeField] private Transform FishPool;
    int fishIndexPool = 0;
    int maxFishIndexPool;
    private List<Transform> Areas = new();

    [HideInInspector][SerializeField] private string zone;

    [Header("ONLY FOR INFORMATION")]
    [SerializeField] private int sumOfFishInZone;
    public int currentFish = 0;

    private void OnDisable()
    {
        currentSpawning = false;
    }

    void Start()
    {
        if (transform.childCount == 0)
            return;

        FishPool = transform.GetChild(transform.childCount-1);

        for(int i = 0; i < transform.childCount-1; i++)
        {
            Areas.Add(transform.GetChild(i));
            sumOfFishInZone += Areas[i].GetComponent<FishSpawnerBoundaries>().maxNumberOfFishInBoundary;
        }

        if (FishPool != null)
        {
            maxFishIndexPool = FishPool.transform.childCount;
        }

        if (currentSpawning)
        {
            StartCoroutine("CheckCurrentFish");
        }

        StartSpawningZone();
    }

IEnumerator CheckCurrentFish()
    {
        if (currentFish < sumOfFishInZone)
        {
            float timer = 0.001f;

            if (fishIndexPool < maxFishIndexPool)
            {
                int randomPool = Random.Range(0, 100);

                GameObject fish = FishPool.transform.GetChild(fishIndexPool).gameObject; // ...

                int areaChosenIndex = Random.Range(0, Areas.Count);
                Transform chosenArea = Areas[areaChosenIndex];
                var spawned = chosenArea.GetComponent<FishSpawnerBoundaries>().WillSpawnFish();
                if (spawned)
                {
                    int randomSpawnOfChosenAreaIndex = Random.Range(0, chosenArea.GetChild(0).childCount);
                    var fishSpawnPoint = chosenArea.GetChild(0).GetChild(randomSpawnOfChosenAreaIndex).position;
                    fish.GetComponent<FishWaypoints>().spawnedPosition = new Vector3(fishSpawnPoint.x, fishSpawnPoint.y, fishSpawnPoint.z);
                    fish.GetComponent<FishWaypoints>().SpawnParent = chosenArea;
                    fish.SetActive(true);

                    fishIndexPool++;
                    fishControllerScript.currentFishScene++;
                    currentFish++;
                }
                else
                {
                    Areas.RemoveAt(areaChosenIndex); // no longer accepting fish
                }



                yield return new WaitForSeconds(timer);
                StartCoroutine("CheckCurrentFish");
            }

            else
            {
                currentSpawning = false;
            }
        }
    }

    public void StartSpawningZone()
    {
        currentSpawning = true;
        StartCoroutine(CheckCurrentFish());
    }
}
