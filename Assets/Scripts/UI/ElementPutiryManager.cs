using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementPutiryManager : MonoBehaviour
{
    public TMP_Text TextArea;
    private Element element;
    private string DisplayName;
    public GameObject inputText;
    private Compound ParentCompound;
    public Slider slider;
    private static bool locked;

    
    public void Init(Element e, Compound c){
        element = e;
        ParentCompound = c;

        TextArea.text = element.symbol;
        DisplayName = element.symbol;
        

    }
    public void PurityChanged(string str){
        if (!locked){
            locked = true;
            slider.GetComponent<SliderManager>().Lock();
            ErrorManager.instance.Clear();
            if (str == "")
                str = "0";
            
            GameObject.FindGameObjectWithTag("Flask").GetComponent<FlaskManager>().UpdateFlasks(ParentCompound, element, double.Parse(str)/100);
            locked = false;
            slider.GetComponent<SliderManager>().Unlock();

        }
    }
    public Element GetElement(){
        return element;
    }
    public void SetInputText(string str){
        inputText.GetComponent<TMP_InputField>().text = str;
        slider.GetComponent<SliderManager>().SetValue(str);

    }
    public void Lock(){
        locked = true;
    }
    public void Unlock(){
        locked = false;
    }
    // public void PurityChangedSlider(string str){
    //     SetInputText(str);
    //     PurityChanged(str);
    // }

}
