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
            calibrateText.text = "Coloca o bra�o direito a apontar para a frente numa posi��o confort�vel. Quando achares que est� bem, confirma levantando o bra�o esquerdo esticado acima da cabe�a e para o lado.";
            calibrateText.color = Color.yellow;
            CalibrateImage.sprite = calibrateIcons[1];
        }
    }
}
