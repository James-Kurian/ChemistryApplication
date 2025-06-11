using System;
using System.Collections.Generic;

[System.Serializable]
public class Element : Compound{
    public static Element[] elements;
    public static Dictionary<string, int> polyatomicElements = new Dictionary<string, int> {
        {"I", 2},
        {"Br", 2},
        {"Cl", 2},
        {"F", 2},
        {"O", 2},
        {"N", 2},
        {"H", 2},
        {"P", 4},
        {"S", 8}
        //I2 Br2 Cl2 F2 O2 N2 H2 P4 S8
    };
    public string name;
    public bool isPolyatomic;
    public string properSymbol;
    public string symbol; 
    public int atomicNumber;
    public float atomicWeight;
    public float density;
    public string phase;
    public float electroNegativity;
    public float electronAffinity;
    public string block;
    public int group;
    public int period;
    public string electronConfiguration;
    public string color;
    public float atomicRadius;
    public Element(){
        isElement = true; 
    }

    public override bool Equals(object obj)
    {
        
        if (obj == null || GetType() != obj.GetType() || ((Element)obj).name != name) {
            return false;
        }
        return true;
        
    
    }
    public override int GetHashCode()
    {
        return name.GetHashCode();
    }


    public static Element GetElementBySymbol(string symbol){
        foreach (var elem in elements){
            if (Utilities.Simplify(elem.symbol)==Utilities.Simplify(symbol)){
                return elem;
            }
        }
        
        return null;
    }
}

