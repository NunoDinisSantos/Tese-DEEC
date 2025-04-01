using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int inventorySpace;
    public PlayerProgress playerProgress;

    void Start()
    {
        switch(playerProgress.shipStorageModule)
        {
            case 0:
                inventorySpace = 10;
                break;
            case 1:
                inventorySpace = 50;
                break;
            case 2:
                inventorySpace = 150;
                break;
        }
    }
}
