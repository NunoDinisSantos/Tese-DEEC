using UnityEngine;

public class GrabFish : MonoBehaviour
{
    [SerializeField] private HarpoonTrigger harpoonTrigger;

    private void OnTriggerEnter(Collider other)
    {
        harpoonTrigger.StartReeling(other.transform);
    }
}
