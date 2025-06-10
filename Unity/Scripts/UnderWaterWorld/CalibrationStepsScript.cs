using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationStepsScript : MonoBehaviour
{
    public bool tPose = false;
    public bool armDir = false;
    public bool calibrated = false;

    [SerializeField] private Sprite[] calibrateIcons;
    [SerializeField] private TMP_Text calibrateText;
    [SerializeField] private Image CalibrateImage;
    [SerializeField] private Transform holder;

    [SerializeField] private UnityClientProxy proxy;
    bool needToEnable = true;
    bool needToDisable = true;

    public GameObject ArmDirsImages;
    private void Start()
    {
        StartCoroutine("MyUpdate");
    }

    IEnumerator MyUpdate()
    {
        if (tPose && armDir)
        {
            ArmDirsImages.SetActive(false);
            holder.gameObject.SetActive(false);
            if (needToDisable)
            {
                StartCoroutine(proxy.DisableImageStream());
                needToDisable = false;
                needToEnable = true;
            }
        }

        else
        {
            if (needToEnable)
            {
                StartCoroutine(proxy.EnableImageStream());
                needToDisable = true;
                needToEnable = false;
            }

            StartCoroutine(proxy.GetImageCoroutineNew());

            if (!tPose)
            {
                holder.gameObject.SetActive(true);
                CalibrateImage.gameObject.SetActive(true);

                //calibrateText.text = "Faz uma T-POSE. Tem em conta desvios!";
                calibrateText.text = "Do a T-POSE. Have in mind deviations!";

                calibrateText.color = Color.red;
                //CalibrateImage.sprite = calibrateIcons[0];
            }

            if (tPose && !armDir)
            {
                //calibrateText.text = "Coloca o braço direito a apontar para a câmara numa posição confortável e central (figura 1). Quando achares que está bem, confirma colocando o pulso esquerdo em cima do pulso direito (figura 2).";
                calibrateText.text = "Point your right arm to the camera in a central and comfortable position (figure 1). When you think it's fine, confirm it by putting your left wrist on top of your right wrist (figure 2).";
                calibrateText.color = Color.yellow;
                //CalibrateImage.sprite = calibrateIcons[1];
                CalibrateImage.gameObject.SetActive(false);
                ArmDirsImages.SetActive(true);
            }
        }

        yield return new WaitForSeconds(0.1f);

        StartCoroutine("MyUpdate");
    }
}
