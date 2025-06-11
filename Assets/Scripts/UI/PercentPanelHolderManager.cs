using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PercentPanelHolderManager : MonoBehaviour
{
    public GameObject SpeciesControlPrefab;
    public GameObject button;
    private SpeciesControlManager script;
    public bool isPureHolder;

    public void AddHolder(Compound c){
        
       
       
        GameObject newController = Instantiate(SpeciesControlPrefab, transform);
        script = newController.GetComponent<SpeciesControlManager>();
        
        script.Init(c, this);
        gameObject.tag = "Untagged";
        DisableButton();

        
        
    }

    public void SetAsTarget(){
        //once you click the select button the speicies display will open
        //once you click on a species it does not know where to add it too
        //this changes the tags (as soon as you click the button this function is called) so it is the target for the species
        gameObject.tag = "DynamicButtonTarget";
    }
    public void UnSetAsTarget(){
        gameObject.tag = "Untagged";
    }

    internal ElementPutiryManager[] GetElementPurityManagers(){
        return script.GetElementPurityManagers();
    }
    internal void SetCompoundPurity(double d){
        script.SetCompoundPurity(d);
    }
    public void EnableButton(){
        button.SetActive(true);
    }
    public void DisableButton(){
        button.SetActive(false);
    }
}
