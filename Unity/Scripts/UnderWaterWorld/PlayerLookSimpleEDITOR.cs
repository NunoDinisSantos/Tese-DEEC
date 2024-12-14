using UnityEngine;

public class PlayerLookSimpleEDITOR : MonoBehaviour
{
    [SerializeField] float sensibilidadeX = 70f;
    [SerializeField] float sensibilidadeY = 70f;
    float xRotation;
    float yRotation;
    public Transform orientation;

    // Update is called once per frame
    void Update()
    {
        PlayerLook();

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
