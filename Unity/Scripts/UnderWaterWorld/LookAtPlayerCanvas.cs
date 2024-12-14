using System.Collections;
using TMPro;
using UnityEngine;

public class LookAtPlayerCanvas : MonoBehaviour
{
    public Transform player;
    private TMP_Text distanceText;
    public Transform safeTransform;
    private void Start()
    {
        distanceText = GetComponent<TMP_Text>();
        StartCoroutine(MyUpdate());
    }

    IEnumerator MyUpdate()
    {
        float distance = Vector3.Distance(safeTransform.position, player.position);
        distance = Mathf.RoundToInt(distance);
        transform.LookAt(player);       
        distanceText.text = "Base: " + distance + " m";
        yield return new WaitForSeconds(1);
        StartCoroutine(MyUpdate());
    }
}
