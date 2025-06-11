using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public Slider slider;
    public GameObject elementPurity;
    private static bool locked;

    internal void SetValue(string str)
    {
        slider.value = float.Parse(str);
    }

    void Start(){
        slider.onValueChanged.AddListener((v) => {
            if (!locked){
                ElementPutiryManager script = elementPurity.GetComponent<ElementPutiryManager>();
                script.SetInputText(v.ToString());
                script.PurityChanged(v.ToString());
            }
        });
    }
    public void Lock(){
        locked = true;
    }
    public void Unlock(){
        locked = false;
    }
    
}
