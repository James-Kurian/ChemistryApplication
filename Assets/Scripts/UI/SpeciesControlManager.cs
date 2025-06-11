using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeciesControlManager : MonoBehaviour
{

    public Compound Species;
    public TMP_Text TextArea;
    public TMP_Text purityDisplay;
    public GameObject ElementHolder;
    private ElementHolder script;
    private string DisplayName;
    PercentPanelHolderManager parent;
    public void Init(Compound c, PercentPanelHolderManager p){
        Species = c;
        parent = p;
        TextArea.text = Species.properName;
        DisplayName = Species.properName;
        script = ElementHolder.GetComponent<ElementHolder>(); 
        script.FillWithElements(Species);

    }

    internal ElementPutiryManager[] GetElementPurityManagers(){
        return script.GetElementPurityManagers();
    }
    internal void SetCompoundPurity(double d){
        purityDisplay.text = Math.Round(d*100,2) + "%";
    }

    public void Remove(){
        GameObject.FindGameObjectWithTag("Flask").GetComponent<FlaskManager>().Remove(parent.isPureHolder);
        parent.EnableButton();
        Destroy(gameObject);

    }

}
