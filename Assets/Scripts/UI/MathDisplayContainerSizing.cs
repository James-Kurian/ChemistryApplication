using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathDisplayContainerSizing : MonoBehaviour
{
    bool madeBig = false;
    public TMP_Text buttonText;
    public GameObject pureHolder;
    public GameObject impureHolder;
    public void MakeBig(){
        if (!madeBig){
            Vector2 scale = gameObject.GetComponent<RectTransform>().sizeDelta;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(scale.x, scale.y*4);
            madeBig = true;
            buttonText.text = "Collapse";
            pureHolder.SetActive(false);
            impureHolder.SetActive(false);
            
        }else{
            Vector2 scale = gameObject.GetComponent<RectTransform>().sizeDelta;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(scale.x, scale.y/4);
            madeBig = false;
            buttonText.text = "Expand";
            pureHolder.SetActive(true);
            impureHolder.SetActive(true);
            
        }
    }
    public void ToggleVis(){
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
