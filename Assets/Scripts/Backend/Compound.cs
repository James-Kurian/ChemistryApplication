using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Compound
{
    [SerializeField]
    public Dictionary<Compound, int> compound = new Dictionary<Compound, int>();
    public string properName;
    private HashSet<Element> allElements;
    private Dictionary<Element, int> allElementMoles;
    public bool isElement;

    public static Compound GetCompoundFromString(string compStr){
        Compound comp = new();
        string pattern = @"[A-Z][a-z]?[0-9]*|\(.*?\)[0-9]*";
        MatchCollection matches = Regex.Matches(compStr, pattern);
        List<string> parts = new();
        foreach (Match match in matches.Cast<Match>())
        {
            string part = match.Value;
            // Check if the element or group ends with a number, if not, append "1"
            if (!char.IsDigit(part[part.Length - 1]))
            {
                part += "1";
            }
            parts.Add(part);
        }
        foreach (var part in parts){
            if (IsStillCompound(part)){
                Compound compPart = GetCompoundFromString(part[1..^1]);
                List<string> compAndNum = SplitBracketGroupFromIntegers(part);
                if (comp.compound.ContainsKey(compPart)){
                    comp.compound[compPart] += int.Parse(compAndNum[1]);
                }else{
                    comp.AddToCompound(compPart, int.Parse(compAndNum[1]));
                }
            }else{
                List<string> elemAndNum = SplitIntegersFromLetters(part);
                Element currElem = Element.GetElementBySymbol(elemAndNum[0]);
                if (currElem == null){
                    return null;
                }
                if (comp.compound.ContainsKey(currElem)){
                    comp.compound[currElem] += int.Parse(elemAndNum[1]);
                }else{
                    comp.AddToCompound(currElem, int.Parse(elemAndNum[1]));
                }
            }
        }
        comp.UpdateProperName();
        if(comp.GetElements().Count==0){
            return null;
        }
        
        return comp;
    }

    private static bool IsStillCompound(string part)
    {
        return part[0] == '(';
    }
    private static List<string> SplitIntegersFromLetters(string str){
        string pattern = @"[a-zA-Z]+|\d+|.";

        List<string> parts = new List<string>();

        foreach (Match match in Regex.Matches(str, pattern))
        {
            parts.Add(match.Value);
        }
        return parts;
    }
    private static List<string> SplitBracketGroupFromIntegers(string str){
        string pattern = @"\([^\)]*\)|\d+";  

        List<string> parts = new List<string>();

    
        foreach (Match match in Regex.Matches(str, pattern))
        {
            parts.Add(match.Value);
        }
        return parts;
    }

    public void AddToCompound(Compound comp, int count){
        compound.Add(comp, count);
    }
    public void UpdateProperName(){

        properName = UpdateProperNameRecursive(this);
    }
    private string UpdateProperNameRecursive(Compound comp){
        string finalName = "";
        foreach (var species in comp.compound.Keys){
            if (species.isElement){
                finalName += ((Element)species).symbol;
                int count = comp.compound[species];
                if (count > 1){
                    finalName += "<sub>" + count + "</sub>";
                }
            } else{
                int count = comp.compound[species];
                if (count != 1){
                    finalName += "(" + species.properName + ")" + "<sub>" + count + "</sub>";
                }else{
                    finalName += "(" + species.properName + ")";
                }
            }
        }
        
        return finalName;
    }

    public HashSet<Element> GetElements()
    {
        if (allElements == null){
            allElements = GetElements(this);
        }

        return allElements;
    }
    private HashSet<Element> GetElements(Compound c) {
        HashSet<Element> elements = new HashSet<Element>();
        foreach (Compound species in c.compound.Keys){
            if (species.isElement){
                elements.Add((Element)species);
            }else{
                elements.UnionWith(GetElements(species));
            }
        }
        return elements;
    }

    public Dictionary<Element, int> GetElementMoles(){
        if (allElementMoles == null){
            allElementMoles = GetElementMoles(this, 1);
        }
        return allElementMoles;
    }
    private Dictionary<Element, int> GetElementMoles(Compound c, int multiplicationFactor){
        Dictionary<Element, int> elementMoles = new Dictionary<Element, int>();

        foreach (Compound species in c.compound.Keys){
            if (species.isElement){
                if (elementMoles.ContainsKey((Element)species)){
                    elementMoles[(Element)species] += c.compound[species] * multiplicationFactor; 
                }else{
                    elementMoles[(Element)species] = c.compound[species] * multiplicationFactor;
                }
                
            }else{
                Dictionary<Element, int> childElementMoles = GetElementMoles(species,c.compound[species]*multiplicationFactor);
                foreach (var pair in childElementMoles){
                    if (elementMoles.ContainsKey(pair.Key)){
                        elementMoles[pair.Key] += pair.Value;
                    }else{
                        elementMoles[pair.Key] = pair.Value;
                    }
                }
            }
        }
        return elementMoles;
    }
    public double MolRatio(Element e1, Element e2){
        Dictionary<Element, int> moles = GetElementMoles();
        if (!moles.ContainsKey(e1) || !moles.ContainsKey(e2)){
            return Double.NaN;
        }
        return ((double)moles[e1])/moles[e2];
    }
    public double GetPurityOf(Element elem)
    {
        Dictionary<Element, int> elemMoles = GetElementMoles();
        return (elemMoles[elem] * elem.atomicWeight)/GetMolarMass();
    }
    public static double CalculateMolarMass(Compound comp){
        Dictionary<Element, int> elemMoles = comp.GetElementMoles();
        double molarMass = 0;
        foreach (KeyValuePair<Element, int> pair in elemMoles){
            molarMass += pair.Key.atomicWeight * pair.Value;
        }
        return molarMass;
    }
    public double GetMolarMass(){
        return CalculateMolarMass(this);
    }

    //.Equal and .HashCode method implemetned by Chat-GPT. This might lead to infinite recursion if there is a nested compound but I do not recall that as a possibility in the code.
    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Compound other = (Compound)obj;

        // Quick check for identity or equality of reference
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        // Check if the dictionaries have the same count
        if (compound.Count != other.compound.Count)
        {
            return false;
        }

        // Check each entry in the dictionary for equality
        foreach (var entry in compound)
        {
            Compound key = entry.Key;
            int value = entry.Value;

            if (!other.compound.TryGetValue(key, out int otherValue) || otherValue != value)
            {
                // Either the key doesn't exist in the other dictionary, or the counts don't match
                return false;
            }
        }

        return true;
    }
    public override int GetHashCode() {
        int hash = 17; // Start with a prime number
        // Compute hash code considering the hash codes of entries in the compound dictionary
        foreach (var entry in compound)
        {
            hash = hash * 31 + entry.Key.GetHashCode(); // Multiply by a prime number and add the hash code of the key
            hash = hash * 31 + entry.Value.GetHashCode(); // Again, for the value
        }
        return hash;
    }
    public static string FoilCompound(string comp){
        int outer = -1;
        if(!int.TryParse(comp[^1].ToString(), out outer)){
            //If the parsing did not work then the end is a closed bracket
            return comp[1..^1];
        }
        string pattern = @"[A-Z][a-z]?[0-9]*|\(.*?\)[0-9]*";
        
        MatchCollection matches = Regex.Matches(comp[1..^1], pattern);
        List<string> parts = new();
        foreach (Match match in matches.Cast<Match>())
        {
            string part = match.Value;
            // Check if the element or group ends with a number, if not, append "1"
            if (!char.IsDigit(part[part.Length - 1]))
            {
                part += outer.ToString();
            }else{
                List<string> splitPart = SplitIntegersFromLetters(part);
                splitPart[1] = (int.Parse(splitPart[1])*outer).ToString();
                part = string.Join("", splitPart);
            }

            parts.Add(part);
        }
        return string.Join("", parts);
    }


}
