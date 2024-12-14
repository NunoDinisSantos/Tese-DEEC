using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private FishControllerScript fishControllerScript;
    public bool currentSpawning = false;
    public int currentFish = 0;
    [SerializeField] private Transform FishPool;
    int fishIndexPool = 0;
    int maxFishIndexPool;
    int maxFishScene = 0;
    private List<Transform> Areas = new();
    [SerializeField] private GameObject[] validPoolFishInZone;
    [SerializeField] private float percentagemFish1;
    [SerializeField] private float percentagemFish2;
    [SerializeField] private float percentagemFish3;
    [SerializeField] private float percentagemFish4;
    [SerializeField] private float percentagemFish5;

    [SerializeField] private string zone;

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


        if (fishControllerScript.currentFishScene < maxFishScene && fishIndexPool < maxFishIndexPool) // Atencao que o index++ não é solution porque o player pode apanhar um peixe de uma posição random!
        {

            //Pick random zone valid fish to spawn pooler
            int randomPool = Random.Range(0, 100);
            //var fish;

            //GameObject fish = FishPool.GetChild(fishIndexPool).gameObject; // ...

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

            //var fish = FishPool.GetChild(fishIndexPool).gameObject; // ALTERAR AQUI!

            Transform chosenArea = transform.GetChild(Random.Range(0, transform.childCount-1));
            int randomSpawnOfChosenAreaIndex = Random.Range(0, chosenArea.GetChild(0).childCount);
            fish.transform.position = chosenArea.GetChild(0).GetChild(randomSpawnOfChosenAreaIndex).position;
            fish.GetComponent<FishWaypoints>().spawnedPosition = new Vector3(fish.transform.position.x,fish.transform.position.y,fish.transform.position.z);
            fish.GetComponent<FishWaypoints>().SpawnParent = chosenArea;
            fish.SetActive(true);

            fishIndexPool++;
            fishControllerScript.currentFishScene++;
            timer = 0.1f;
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
