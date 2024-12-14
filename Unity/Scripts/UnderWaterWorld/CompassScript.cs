using UnityEngine;

public class CompassScript : MonoBehaviour
{
    public Transform playerTransform;
    Vector3 dir;

    void Update()
    {
        dir.z = playerTransform.eulerAngles.y;
        transform.localEulerAngles = dir;
    }
}
