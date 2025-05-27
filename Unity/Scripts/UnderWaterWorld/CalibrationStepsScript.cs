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

    void Update()
    {
        if(tPose && armDir)
        {
            holder.gameObject.SetActive(false);
            return;
        }

        if(!tPose)
        {
            holder.gameObject.SetActive(true);
            calibrateText.text = "Faz uma T-POSE. Tem em conta desvios!";
            calibrateText.color = Color.red;
            CalibrateImage.sprite = calibrateIcons[0];
            return;
        }

        if (!armDir)
        {
            calibrateText.text = "Coloca o braço direito a apontar para a câmara numa posição confortável e central. Quando achares que está bem, confirma esticando o braço esquerdo acima da cabeça e para o lado esquerdo.";
            calibrateText.color = Color.yellow;
            CalibrateImage.sprite = calibrateIcons[1];
        }
    }
}
