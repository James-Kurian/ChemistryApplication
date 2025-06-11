using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DynamicButtonManager : MonoBehaviour
{
    private Compound compound;
    public TMP_Text display;
    public void Triggered(){
        GameObject.FindGameObjectWithTag("DynamicButtonTarget").GetComponent<PercentPanelHolderManager>().AddHolder(compound);
        GameObject.FindGameObjectWithTag("Flask").GetComponent<FlaskManager>().SetPureOrImpureCompound(compound);
        GameObject.FindGameObjectWithTag("SpeciesSelector").SetActive(false);
        GameObject.FindGameObjectWithTag("Flask").GetComponent<FlaskManager>().UpdateBasedOnPrevious();

    }
    public void Init(string formula, Compound comp){
        compound = comp;
        display.text = formula;
        
    }
    
}
