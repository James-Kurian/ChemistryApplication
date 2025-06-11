using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject SpeciesDisplay;
    public GameObject Flask;
    public static int SigFigs = 2; 
    public void ShowSelectionPanel(){
        SpeciesDisplay.SetActive(true);
    }
    public void CloseSelectionPanel(){
        SpeciesDisplay.SetActive(false);
    }
    public void Quit(){
        Application.Quit();
    }
    public void UpdateSigFigs(string str){
        if (str != ""){
            SigFigs = int.Parse(str);
            Flask.GetComponent<FlaskManager>().UpdateBasedOnPrevious();
        }
    }
}
