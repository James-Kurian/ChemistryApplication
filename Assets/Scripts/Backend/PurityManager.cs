using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PurityManager{
    private Compound pure;
    private Compound impure;
    
    private double pureCompoundPurity;
    private double impureCompoundPurity;
    private Dictionary<Element, double> elementPurity;
    private double fillerElementPurity;

    public PurityManager(){
           
    }

    //given an element and its purity this function calculated the purity of the other elements
    internal bool CalculatePurity(Compound comp, Element givenElement, double purity){
        bool calculationSuccessful = true;
        elementPurity = new Dictionary<Element, double>();
        Compound given;
        Compound limbo;
        if (comp.Equals(pure)){
            given = pure;
            limbo = impure;
        }else{
            given = impure;
            limbo = pure;
        }
        


        double elementMass = givenElement.atomicWeight;
        int molesOfElementInGiven = given.GetElementMoles()[givenElement];
        int molesOfElementInLimbo;
        double massOfGivenCompound = given.GetMolarMass();
        double massOfLimboCompound;
        HashSet<Element> elements = new HashSet<Element>(given.GetElements()); 
        bool foundMols;
        if(limbo == null){
            massOfLimboCompound = 1;
            molesOfElementInLimbo = 0;
        }else{
            HashSet<Element> moreElements = new HashSet<Element>(limbo.GetElements());
            elements.UnionWith(moreElements);
            massOfLimboCompound = limbo.GetMolarMass();
            foundMols = limbo.GetElementMoles().TryGetValue(givenElement, out molesOfElementInLimbo);
            if (!foundMols) molesOfElementInLimbo = 0;
        }
        double eliminate = 1;
        double molesOfLimboAdded = (elementMass*molesOfElementInGiven-massOfGivenCompound*purity)/(massOfLimboCompound*purity-elementMass*molesOfElementInLimbo);
        if (molesOfLimboAdded == -1){
            molesOfLimboAdded = 0;
        }else if (molesOfLimboAdded < 0){
            double purityInGiven = elementMass*molesOfElementInGiven/massOfGivenCompound;
            double purityInLimbo = elementMass*molesOfElementInLimbo/massOfLimboCompound;
            if (Math.Min(purityInLimbo,purityInGiven) > purity){
                ErrorManager.instance.Log("The purity of " + givenElement.symbol + " should be between " + Math.Round(Math.Min(purityInLimbo,purityInGiven)*100,UIManager.SigFigs) + "% - " + Math.Round(Math.Max(purityInLimbo,purityInGiven)*100,UIManager.SigFigs) + "%");
                purity = Math.Min(purityInLimbo,purityInGiven);
                molesOfLimboAdded = (elementMass*molesOfElementInGiven-massOfGivenCompound*purity)/(massOfLimboCompound*purity-elementMass*molesOfElementInLimbo);

            }else if (Math.Max(purityInLimbo,purityInGiven) < purity){
                ErrorManager.instance.Log("The purity of " + givenElement.symbol + " should be between " + Math.Round(Math.Min(purityInLimbo,purityInGiven)*100,UIManager.SigFigs) + "% - " + Math.Round(Math.Max(purityInLimbo,purityInGiven)*100,UIManager.SigFigs) + "%");
                purity = Math.Max(purityInLimbo,purityInGiven);
                molesOfLimboAdded = (elementMass*molesOfElementInGiven-massOfGivenCompound*purity)/(massOfLimboCompound*purity-elementMass*molesOfElementInLimbo);

            }else{
                ErrorManager.instance.Log("Something went wrong... This error message should never log which means James missed a bug.");
                calculationSuccessful = false;
            }
            
        }
        if (double.IsInfinity(molesOfLimboAdded)){
            molesOfLimboAdded = 1;
            eliminate = 0;
        }
        double denominator = massOfGivenCompound*eliminate + massOfLimboCompound*molesOfLimboAdded;

        double numerator;
        foreach (Element element in elements){
            elementMass = element.atomicWeight;
            foundMols = given.GetElementMoles().TryGetValue(element, out molesOfElementInGiven);
            if (!foundMols) molesOfElementInGiven = 0;            
            foundMols = limbo != null && limbo.GetElementMoles().TryGetValue(element, out molesOfElementInLimbo);
            if (!foundMols) molesOfElementInLimbo = 0;
            numerator = elementMass*molesOfElementInGiven*eliminate + elementMass*molesOfElementInLimbo*molesOfLimboAdded;
            elementPurity[element] = numerator/denominator;
        }
        double givenCompoundPurity = massOfGivenCompound*eliminate/denominator;
        double limboCompoundPurity = massOfLimboCompound*molesOfLimboAdded/denominator;
        
        if (given == pure){
            pureCompoundPurity = givenCompoundPurity;
            impureCompoundPurity = limboCompoundPurity;
        }else{
            pureCompoundPurity = limboCompoundPurity;
            impureCompoundPurity = givenCompoundPurity;
        }
        fillerElementPurity = molesOfLimboAdded/denominator;
        return calculationSuccessful;

    }



    internal Dictionary<Element, double> GetElementPurity(){
        return elementPurity;
    }
    internal double GetPureCompoundPurity(){
        return pureCompoundPurity;
    }
    internal double GetImpureCompoundPurity(){
        return impureCompoundPurity;
    }
    internal double GetFillerElementPurity(){
        return fillerElementPurity;
    }


    internal void SetPureCompound(Compound c)
    {
        pure = c;


    }

    internal void SetImpureCompound(Compound c)
    {
        impure = c;

    }
    internal Compound GetPureCompound(){
        return pure;
    }
    internal Compound GetImpureCompound(){
        return impure;
    }
    internal void Remove(bool removePure){

        if (removePure){
            pure = null;
        }else{
            impure = null;
        }
    }
}
