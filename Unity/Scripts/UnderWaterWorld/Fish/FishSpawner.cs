using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private FishControllerScript fishControllerScript;
    public bool currentSpawning = false;
    [SerializeField] private Transform FishPool;
    int fishIndexPool = 0;
    int maxFishIndexPool;
    int maxFishScene = 0;
    [SerializeField] private List<Transform> Areas = new();
    [SerializeField] private GameObject[] validPoolFishInZone;
    [SerializeField] private float percentagemFish1;
    [SerializeField] private float percentagemFish2;
    [SerializeField] private float percentagemFish3;
    [SerializeField] private float percentagemFish4;
    [SerializeField] private float percentagemFish5;

    [SerializeField] private string zone;

    [SerializeField] private int maxFishInBoundaries;
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
            maxFishInBoundaries += Areas[i].GetComponent<FishSpawnerBoundaries>().maxNumberOfFishInBoundary;
        }


        if(fishControllerScript != null)
        {
            maxFishScene = fishControllerScript.maxFishScene;
        }

        if (FishPool != null)
        {
            maxFishIndexPool = FishPool.transform.childCount;
        }

        if (currentSpawning)
        {
            StartCoroutine(CheckCurrentFish());
        }
    }

    IEnumerator CheckCurrentFish()
    {
        if(!currentSpawning)
        {
            yield return new();
        }


        if (fishIndexPool > maxFishIndexPool)
        {
            fishIndexPool = 0;
        }

        float timer = 5;


        if (fishControllerScript.currentFishScene < maxFishScene && fishIndexPool < maxFishIndexPool && currentFish < maxFishInBoundaries) // Atencao que o index++ n�o � solution porque o player pode apanhar um peixe de uma posi��o random!
        {
            int randomPool = Random.Range(0, 100);

            GameObject fish = FishPool.transform.GetChild(fishIndexPool).gameObject; // ...

            switch (zone)
            {
                case "safe":
                    if (randomPool > 30)
                    {
                        //fish = validPoolFishInZone[0].transform.GetChild(fishIndexPool).gameObject;

                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    else
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    break;

                case "coral":
                    if (randomPool > 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    if(randomPool > 20 && randomPool <= 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    else
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    break;

                case "hot":
                    if (randomPool > 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    if (randomPool > 20 && randomPool <= 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    else
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    break;

                case "ice":
                    if (randomPool > 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    if (randomPool > 20 && randomPool <= 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    else
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    break;

                case "temple":
                    if (randomPool > 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    if (randomPool > 20 && randomPool <= 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    else
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    break;

                case "color":
                    if (randomPool > 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    if (randomPool > 20 && randomPool <= 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    else
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    break;

                case "deep":
                    if (randomPool > 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    if (randomPool > 20 && randomPool <= 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    else
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    break;

                case "base":
                    if (randomPool > 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    if (randomPool > 20 && randomPool <= 60)
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    else
                    {
                        fish = FishPool.GetChild(fishIndexPool).gameObject;
                    }
                    break;
            }

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

            timer = 0.02f;
        }

        yield return new WaitForSeconds(timer);

        StartCoroutine(CheckCurrentFish());
    }

    public void StartSpawningZone()
    {
        currentSpawning = true;
        StartCoroutine(CheckCurrentFish());
    }
}
