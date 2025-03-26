using System.Linq;
using UnityEngine;

public class PlayerLookSimpleEDITOR : MonoBehaviour
{
    [SerializeField] float sensibilidadeX = 70f;
    [SerializeField] float sensibilidadeY = 70f;
    float xRotation;
    float yRotation;

    private float verticalLook;
    private float horizontalLook;

    [SerializeField] private bool UnityEditor = true;

    // Update is called once per frame
    void Update()
    {
        if (UnityEditor)
        {
            PlayerLook();
            return;
        }

        PlayerLookProxy();
    }

    public void ProxyPlayerLook(float h, float v)
    {
        horizontalLook = h;
        verticalLook = v;
    }

    private void PlayerLookProxy()
    {
        float mouseX = horizontalLook;
        float mouseY = verticalLook;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(-xRotation, yRotation, 0);
    }

    private void PlayerLook()
    {

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensibilidadeX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * -sensibilidadeY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(-xRotation, yRotation, 0);
    }
}
