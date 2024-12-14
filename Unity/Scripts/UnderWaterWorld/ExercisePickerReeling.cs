using System.ComponentModel;
using TMPro;
using UnityEngine;

public class ExercisePickerReeling : MonoBehaviour
{
    [SerializeField] private Sprite[] images;
    [SerializeField] private TMP_Text text;
    public int currentValueToShow = 0;
    private bool showing = false;

    [Header("Movimentos")]
    [Description("Decidir que movimentos podemos fazer para fazer reel aos peixes/objectos")]
    [SerializeField] private string[] Exercicios = { "Saltos!", "Agachamentos!" };

    void Update()
    {
        if (currentValueToShow == 0)
        {
            text.text = string.Empty;
            return;
        }

        HandleShowing();
    }

    private void HandleShowing()
    {
        text.text = Exercicios[currentValueToShow - 1];
        //Handle sprite animation
    }
}
