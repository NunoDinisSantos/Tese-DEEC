using System.Collections;
using UnityEngine;
using TMPro;
public class ShowPlayerMessageScript : MonoBehaviour
{
    [SerializeField] private TMP_Text messageObject;
    public void ShowMessage(string message)
    {
        StartCoroutine(ShowMessageRoutine(message));
    }

    IEnumerator ShowMessageRoutine(string message)
    {
        messageObject.text = message;
        yield return new WaitForSeconds(5);
        messageObject.text = string.Empty;
        gameObject.SetActive(false);
    }
}
