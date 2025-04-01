using System.Collections;
using UnityEngine;

public class FishControllerScript : MonoBehaviour
{
    public int maxFishScene = 50;
    public int currentFishScene = 0;
    [SerializeField] private Transform[] ZonesSpawner;
    [HideInInspector][SerializeField] private string currentZone;
    public Collider playerZoneCollider;
    int indexSpawner = 0;
    private bool started = false;

    private void Start()
    {
        SpawnFishFirst();
    }

    private void SpawnFishFirst()
    {
        currentFishScene = 0;

        if (!started)
        {
            for (int i = 0; i < ZonesSpawner.Length; i++)
            {
                ZonesSpawner[i].gameObject.SetActive(true);
                ZonesSpawner[i].gameObject.GetComponent<FishSpawner>().StartSpawningZone();
            }
            StartCoroutine("SpawnFishFirstTime");

            return;
        }
    }

    public void ActivateSpawner(string zoneName)
    {
        if(!started)
        {
            return;
        }

        switch (zoneName)
        {
            case "Safe":

                ZonesSpawner[0].gameObject.SetActive(true);
                ZonesSpawner[1].gameObject.SetActive(false);
                ZonesSpawner[2].gameObject.SetActive(false);
                ZonesSpawner[3].gameObject.SetActive(false);
                ZonesSpawner[4].gameObject.SetActive(false);
                ZonesSpawner[5].gameObject.SetActive(false);
                ZonesSpawner[6].gameObject.SetActive(false);

                currentZone = "Safe";
                break;
            case "Green":
                currentZone = "Green";
                ZonesSpawner[1].gameObject.SetActive(true);
                ZonesSpawner[0].gameObject.SetActive(false);
                ZonesSpawner[2].gameObject.SetActive(false);
                ZonesSpawner[3].gameObject.SetActive(false);
                ZonesSpawner[4].gameObject.SetActive(false);
                ZonesSpawner[5].gameObject.SetActive(false);
                ZonesSpawner[6].gameObject.SetActive(false);
                break;
            case "Deep":
                currentZone = "Deep";
                ZonesSpawner[2].gameObject.SetActive(true);
                ZonesSpawner[1].gameObject.SetActive(false);
                ZonesSpawner[0].gameObject.SetActive(false);
                ZonesSpawner[3].gameObject.SetActive(false);
                ZonesSpawner[4].gameObject.SetActive(false);
                ZonesSpawner[5].gameObject.SetActive(false);
                ZonesSpawner[6].gameObject.SetActive(false);
                break;
            case "Temple":
                currentZone = "Temple";
                ZonesSpawner[3].gameObject.SetActive(true);
                ZonesSpawner[1].gameObject.SetActive(false);
                ZonesSpawner[2].gameObject.SetActive(false);
                ZonesSpawner[0].gameObject.SetActive(false);
                ZonesSpawner[4].gameObject.SetActive(false);
                ZonesSpawner[5].gameObject.SetActive(false);
                ZonesSpawner[6].gameObject.SetActive(false);
                break;
            case "Hot":
                currentZone = "Hot";
                ZonesSpawner[5].gameObject.SetActive(true);
                ZonesSpawner[1].gameObject.SetActive(false);
                ZonesSpawner[2].gameObject.SetActive(false);
                ZonesSpawner[3].gameObject.SetActive(false);
                ZonesSpawner[4].gameObject.SetActive(false);
                ZonesSpawner[0].gameObject.SetActive(false);
                ZonesSpawner[6].gameObject.SetActive(false);
                break;
            case "Ice":
                currentZone = "Ice";
                ZonesSpawner[4].gameObject.SetActive(true);
                ZonesSpawner[1].gameObject.SetActive(false);
                ZonesSpawner[2].gameObject.SetActive(false);
                ZonesSpawner[3].gameObject.SetActive(false);
                ZonesSpawner[0].gameObject.SetActive(false);
                ZonesSpawner[5].gameObject.SetActive(false);
                ZonesSpawner[6].gameObject.SetActive(false);
                break;
            case "Base":
                currentZone = "Base";
                ZonesSpawner[6].gameObject.SetActive(true);
                ZonesSpawner[1].gameObject.SetActive(false);
                ZonesSpawner[2].gameObject.SetActive(false);
                ZonesSpawner[3].gameObject.SetActive(false);
                ZonesSpawner[4].gameObject.SetActive(false);
                ZonesSpawner[5].gameObject.SetActive(false);
                ZonesSpawner[0].gameObject.SetActive(false);
                break;
        }
    }

    public void MaintainMaxFishPopulation()
    {
        currentFishScene--;

        ZonesSpawner[indexSpawner].gameObject.SetActive(true);
        ZonesSpawner[indexSpawner].gameObject.GetComponent<FishSpawner>().StartSpawningZone();
    }

    private IEnumerator SpawnFishFirstTime()
    {
        yield return new WaitForSeconds(5);

        for (int i = 1; i < ZonesSpawner.Length; i++)
        {
            ZonesSpawner[i].gameObject.SetActive(false);
        }

        started = true;
    }
}
