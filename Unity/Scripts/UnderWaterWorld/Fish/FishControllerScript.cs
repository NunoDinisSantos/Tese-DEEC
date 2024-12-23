using UnityEngine;

public class FishControllerScript : MonoBehaviour
{
    public int maxFishScene = 50;
    public int currentFishScene = 0;
    [SerializeField] private Transform[] ZonesSpawner;
    [SerializeField] private string currentZone;
    public Collider playerZoneCollider;
    int indexSpawner = 0;

    private void DeactivateSpawners()
    {
        if(ZonesSpawner == null)
        {
            return;
        }

        foreach(var spawner in ZonesSpawner)
        {
            spawner.gameObject.GetComponent<FishSpawner>().currentSpawning = false;
            spawner.gameObject.SetActive(false);
        }

        currentFishScene = 0;
    }

    public void ActivateSpawner(string zoneName)
    {
        foreach (var zone in ZonesSpawner)
        {
            zone.gameObject.SetActive(false);
        }
        
        currentFishScene = 0;

        switch (zoneName)
        {
            case "Safe":
                ZonesSpawner[0].gameObject.SetActive(true);
                currentZone = "Safe";
                indexSpawner = 0;
                break;
            case "Green":
                currentZone = "Green";
                indexSpawner = 1;
                break;
            case "Deep":
                currentZone = "Deep";
                indexSpawner = 2;
                break;
            case "Temple":
                currentZone = "Temple";
                indexSpawner = 3;
                break;
            case "Hot":
                currentZone = "Hot";
                indexSpawner = 5;
                break;
            case "Ice":
                currentZone = "Ice";
                indexSpawner = 4;
                break;
            case "Base":
                currentZone = "Base";
                indexSpawner = 6;
                break;
        }

        ZonesSpawner[indexSpawner].gameObject.SetActive(true);
        ZonesSpawner[indexSpawner].gameObject.GetComponent<FishSpawner>().StartSpawningZone();
    }

    public void MaintainMaxFishPopulation()
    {
        currentFishScene--;

        ZonesSpawner[indexSpawner].gameObject.SetActive(true);
        ZonesSpawner[indexSpawner].gameObject.GetComponent<FishSpawner>().StartSpawningZone();
    }
}
