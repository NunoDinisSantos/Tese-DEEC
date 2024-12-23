using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreColliderScript : MonoBehaviour
{
    public bool inStore = false;
    public PlayerMovementWater playerMovement;
    public Collider playerStoreCollider;
    public Transform playerTransform;
    public Transform[] TargetTransf;

    public float threshold = 0.5f;
    public float speed = 0.1f;
    public Animation canvasAnimation;
    public GameObject canvasStore;
    public GameObject shopCamera;
    public ShopManager shopManager;
    [SerializeField] private Proxy proxy;
    [SerializeField] private GameObject storeSub;
    [SerializeField] private GameObject miscStoreObjects;

    public GameObject ConfirmBackToMenu;

    private void OnTriggerEnter(Collider other)
    {
        if(!inStore)
        {
            inStore = true;
            playerMovement.speed = 0.0f; // alterar para 10 depois
            playerMovement.ByPassWater = true;  // alterar para false depois
            playerMovement.lookLocked = true; 
            playerTransform.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
            playerTransform.rotation = Quaternion.Euler(0, 180, 0);
            StartCoroutine(StoreAnimation());
        }
    }

    IEnumerator StoreAnimation()
    {
        Vector3 targetPos = TargetTransf[0].position;
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPos, speed);

        yield return new WaitForSeconds(0.03f);
        if (Vector3.Distance(playerTransform.position, targetPos) > threshold)
            StartCoroutine(StoreAnimation());
        else
            StartCoroutine(StoreAnimationSecond());
    }

    IEnumerator StoreAnimationSecond()
    {
        Vector3 targetPos = TargetTransf[1].position;
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPos, speed);
        yield return new WaitForSeconds(0.03f);
        if (Vector3.Distance(playerTransform.position, targetPos) > threshold)
            StartCoroutine(StoreAnimationSecond());
        else
            StartOpenStore();
    }

    private void StartOpenStore()
    {
        proxy.inShop = true;
        //playerMovement.gameObject.transform.GetChild(0).GetComponent<Animation>().Play("CameraAnimationStore");
        playerMovement.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        shopManager.UpdateShop();
        shopCamera.SetActive(true);
        //shopCamera.GetComponent<Animation>().Play("CameraAnimationStore");
        canvasStore.SetActive(true);
        storeSub.SetActive(true);
        miscStoreObjects.SetActive(true);
    }

    public void BackToMenu(int x)
    {
        if (x == 0)
        {
            ConfirmBackToMenu.SetActive(false);
        }

        if (x == 1)
        {
            ConfirmBackToMenu.SetActive(true);
        }

        if (x == 2)
        {
            ConfirmBackToMenu.SetActive(false);
            SceneManager.LoadScene(0);
        }
    }

    public void BackToWater()
    {
        proxy.inShop = false;
        //canvasAnimation.Play("CloseMenuStore");
        canvasStore.SetActive(false);
        playerMovement.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        //shopCamera.GetComponent<Animation>().Play("CameraAnimationStoreReset");
        shopCamera.SetActive(false);
        storeSub.SetActive(false);
        miscStoreObjects.SetActive(false);
        //playerMovement.gameObject.transform.GetChild(0).GetComponent<Animation>().Play("CameraAnimationStoreReset");
        playerStoreCollider.enabled = false;
        StartCoroutine(StoreAnimationOut());
    }

    IEnumerator StoreAnimationOut()
    {
        Vector3 targetPos = TargetTransf[0].position;
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPos, speed);
        yield return new WaitForSeconds(0.03f);
        if (Vector3.Distance(playerTransform.position, targetPos) > threshold)
            StartCoroutine(StoreAnimationOut());
        else
            StartCoroutine(StoreAnimationOutSecond());
    }
    IEnumerator StoreAnimationOutSecond()
    {
        Vector3 targetPos = TargetTransf[2].position;
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPos, speed*2);

        yield return new WaitForSeconds(0.03f);
        if (Vector3.Distance(playerTransform.position, targetPos) > threshold)
            StartCoroutine(StoreAnimationOutSecond());
        else
            StartCoroutine(BackToWaterEnum());
    }


    private IEnumerator BackToWaterEnum()
    {
        yield return new WaitForSeconds(0.5f);
        //playerMovement.gameObject.transform.GetChild(0).GetComponent<Animation>().Play("CameraAnimationStoreReset");
        inStore = false;
        playerMovement.speed = 15.0f;
        playerMovement.ByPassWater = false;
        playerMovement.lookLocked = false;
        playerMovement.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
        playerMovement.enabled = true;
        playerStoreCollider.enabled = true;
    }
}
