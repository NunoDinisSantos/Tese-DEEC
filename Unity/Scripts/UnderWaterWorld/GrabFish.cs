using UnityEngine;

public class GrabFish : MonoBehaviour
{
    //[SerializeField] private Fish fish;
    //[SerializeField] private int fishValue;
    [SerializeField] private HarpoonTrigger harpoonTrigger;

    private void OnTriggerEnter(Collider other)
    {
        harpoonTrigger.StartReeling(other.transform);
        /*if (other.GetComponent<Fish>() != null)
        {
            fish = other.GetComponent<Fish>();
            fishValue = fish.moneyWorth;
            harpoonTrigger.StartReeling(fish);
        }*/
    }
}
