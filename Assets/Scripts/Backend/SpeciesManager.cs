using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpeciesManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject buttonsScript;
    void Start()
    {
        TextAsset elemData = Resources.Load<TextAsset>("ElementDataset");
        string[] data = elemData.text.Split(new char[] {'\n'});
        Element.elements = new Element[data.Length - 2];
        for (int i = 1; i < data.Length - 1; i++){
            string[] row = data[i].Split(new char[] {','});
            Element e = new();
            e.name = row[0];
            e.symbol = row[1];
            if (Element.polyatomicElements.ContainsKey(e.symbol)){
                e.properSymbol = e.symbol + "<sub>" + Element.polyatomicElements[e.symbol] + "</sub>";
                e.isPolyatomic = true;
            }else{
                e.properSymbol = e.symbol;
            }
            if(!int.TryParse(row[2], out e.atomicNumber)){
                e.atomicNumber = -1;
            };
            if(!float.TryParse(row[3], out e.atomicWeight)){
                e.atomicWeight = float.NaN;
            };
            if(!float.TryParse(row[4], out e.density)){
                e.density = float.NaN;
            };
            e.phase = row[5];
            if(!float.TryParse(row[6], out e.electroNegativity)){
                e.electroNegativity = float.NaN;
            };
            if(!float.TryParse(row[7], out e.electronAffinity)){
                e.electronAffinity = float.NaN;
            };
            e.block = row[8];
            if(!int.TryParse(row[9], out e.group)){
                e.group = -1;
            };
            if(!int.TryParse(row[10], out e.period)){
                e.period = -1;
            };
            e.electronConfiguration = row[11];
            e.color = row[12];
            if(!float.TryParse(row[13], out e.atomicRadius)){
                e.atomicRadius = float.NaN;
            };
            Element.elements[i-1] = e;
        }
        buttonsScript.GetComponent<SelectionManager>().Init();
        
   
        
    }
    public Element GetElementBySymbol(string symbol){
        return Element.GetElementBySymbol(symbol);
    }
    public Compound GetCompoundFromString(string compStr){
        return Compound.GetCompoundFromString(compStr);
    }
}
