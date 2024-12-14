using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UnderWaterColorController : MonoBehaviour
{
    [SerializeField] Volume volume;

    //0 - 0.35 - 0.57 - blue
    //0.05 - 0.57 - 0.40 - Green
    private void Start()
    {
       //var lift = volume.GetComponent<LiftGammaGain>();
        //lift.gamma.SetValue();
    }
    // Update is called once per frame
    void Update()
    {
        // change Color adjustment Lift Gamma Gain - Gamma values
    }
}
