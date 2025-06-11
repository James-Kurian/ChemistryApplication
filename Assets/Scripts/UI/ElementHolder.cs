using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementHolder : MonoBehaviour
{
    public GameObject ElementHolderDisplay;
    public GameObject ElementPurityPrefab;
    ElementPutiryManager[] scripts;
    public void ToggleElementPanel(){
        ElementHolderDisplay.SetActive(!ElementHolderDisplay.activeInHierarchy);
    }
    public void FillWithElements(Compound c){
        HashSet<Element> elements = c.GetElements();
        scripts = new ElementPutiryManager[elements.Count];
        int index = 0;
        foreach (Element element in elements){
    
            GameObject newElementController = Instantiate(ElementPurityPrefab, transform);
            ElementPutiryManager script = newElementController.GetComponent<ElementPutiryManager>();
            scripts[index] = script;
            index++;
            script.Init(element, c);      
        }
    }
    public ElementPutiryManager[] GetScripts(){
        return scripts;
    }

    internal ElementPutiryManager[] GetElementPurityManagers(){
        return scripts;
    }

}
