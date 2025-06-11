using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[System.Serializable]
public class SelectionManager : MonoBehaviour
{

    public GameObject ButtonPrefab;
    public TMP_Text inputCompoundText;
    ArrayList compoundButtons = new ArrayList();
    void Start()
    {


    }
    public void Init(){
        TextAsset elemData = Resources.Load<TextAsset>("DefaultSpecies");
        string[] defaultCompoundsAsText = elemData.text.Split(",");
        // compoundSelection = new Compound[defaultCompoundsAsText.Length];
        for (int i = 0; i < defaultCompoundsAsText.Length; i++){
            Compound compound = Compound.GetCompoundFromString(defaultCompoundsAsText[i].Trim());        
            GameObject newButton = Instantiate(ButtonPrefab, gameObject.transform);
            DynamicButtonManager script = newButton.GetComponent<DynamicButtonManager>(); 
            script.Init(compound.properName, compound);
            compoundButtons.Add(compound);
            
            }

    }
    public void addButton(){
        string compText = inputCompoundText.text;
        if (compText != ""){
            Compound compound = Compound.GetCompoundFromString(compText.Trim());
            if (compound != null && !compoundButtons.Contains(compound)){
                GameObject newButton = Instantiate(ButtonPrefab, gameObject.transform);
                DynamicButtonManager script = newButton.GetComponent<DynamicButtonManager>(); 
                script.Init(compound.properName, compound);
                compoundButtons.Add(compound);
            }
        }
    }
    

}
