using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMath : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject fractPredab;
    public GameObject generalMathPrefab;
    public GameObject content;
    public void UpdateDisplay(Element givenElement, double givenPurity, Compound pureCompound, Compound impureCompound){

        foreach (Transform child in transform){
            Destroy(child.gameObject);
        }
        ConvertExpressionToDisplayLine($" Given that {Math.Round(givenPurity*100,UIManager.SigFigs)}% of the sample is {givenElement.symbol}.");

        //1 means the compounds do not share any common elements
        //2 means the compounds do share common elements but the element we are given is not one of the shared elements
        //3 means the compounds do share common elements and the element we have the purity of is one of those shared elements
        int situationNumber = 1;
        if (impureCompound != null){
            HashSet<Element> impureElements = impureCompound.GetElements();
            foreach (Element e in pureCompound.GetElements()){
                if (impureElements.Contains(e)){
                    situationNumber = 2;
                    if (e.Equals(givenElement)){
                        situationNumber = 3;
                    }
                }
            }
        }


        if (situationNumber == 1){
            Debug.Log("sit 1");
            if (impureCompound == null){
                ConvertExpressionToDisplayLine($" And assuming the Impurity does not contain any elements in the pure compound.");
            }
            ArrayList percent = new(); 
            foreach(Element element in pureCompound.GetElements()){
                if (!element.Equals(givenElement)){
                    ConvertExpressionToDisplayLine($" Find %{element.symbol}.");

                    double result = givenPurity*(1/givenElement.atomicWeight)*pureCompound.MolRatio(element,givenElement)*element.atomicWeight*100;
                    percent.Add(result);
                    result = Math.Round(result, UIManager.SigFigs);
                    string expression = $"{givenPurity*100}g{givenElement.symbol}//100gSample|x|1mol{givenElement.symbol}//{givenElement.atomicWeight}g{givenElement.symbol}|x|{pureCompound.GetElementMoles()[element]}mol{element.symbol}//{pureCompound.GetElementMoles()[givenElement]}mol{givenElement.symbol}|x|{element.atomicWeight}g{element.symbol}//1mol{element.symbol}|x|100|=|{result}%";
                    ConvertExpressionToDisplayLine(expression);
                }

            }
            ConvertExpressionToDisplayLine($" Find %{pureCompound.properName}.");
            double result2 = givenPurity*(1.0/givenElement.atomicWeight)*(1.0/pureCompound.GetElementMoles()[givenElement])*(pureCompound.GetMolarMass())*100;
            result2 = Math.Round(result2, UIManager.SigFigs);
            string expression2 = $"{givenPurity*100}g{givenElement.symbol}//100gSample|x|1mol{givenElement.symbol}//{givenElement.atomicWeight}g{givenElement.symbol}|x|1mol{pureCompound.properName}//{pureCompound.GetElementMoles()[givenElement]}mol{givenElement.symbol}|x|{pureCompound.GetMolarMass()}g{pureCompound.properName}//1mol{pureCompound.properName}|x|100|=|{result2}%";
            ConvertExpressionToDisplayLine(expression2);
            ConvertExpressionToDisplayLine($" Or,");
            double result3 = givenPurity*100; //this will be the same as result2
            string expression3 = $" {givenPurity*100}%";
            foreach(double num in percent){
                result3 += num;
                expression3+= " + " + num + "%";
            }
           
            expression3 += $"|=|{Math.Round(result3,UIManager.SigFigs)}%";
            ConvertExpressionToDisplayLine(expression3);
            double pureCompoundPurity = result3;
            if (impureCompound == null){
                ConvertExpressionToDisplayLine($" Find %Impurity.");
                ConvertExpressionToDisplayLine($" 100% - {pureCompoundPurity}%|=|{Math.Round(100-pureCompoundPurity,UIManager.SigFigs)}%");

            }else{
                ConvertExpressionToDisplayLine($" Find %{impureCompound.properName}.");
                ConvertExpressionToDisplayLine($" 100% - {pureCompoundPurity}%|=|{Math.Round(100-pureCompoundPurity,UIManager.SigFigs)}%");
                //calulate other elements
                foreach(Element element in impureCompound.GetElements()){
                    ConvertExpressionToDisplayLine($" Find %{element.symbol}.");
                    double result = (100-pureCompoundPurity)*(1.0/impureCompound.GetMolarMass())*(impureCompound.GetElementMoles()[element])*element.atomicWeight;
                    result = Math.Round(result,UIManager.SigFigs);
                    string expression = $"{100-pureCompoundPurity}g{impureCompound.properName}//100gSample|x|1mol{impureCompound.properName}//{impureCompound.GetMolarMass()}g{impureCompound.properName}|x|{impureCompound.GetElementMoles()[element]}mol{element.symbol}//1mol{impureCompound.properName}|x|{element.atomicWeight}g{element.symbol}//1mol{element.symbol}|x|100|=|{result}%";
                    ConvertExpressionToDisplayLine(expression);
                }

            }
        }else{
            //situation 2 and 3
            ConvertExpressionToDisplayLine($" Make purity equation. Let y rep purity. Let x rep moles of impurity added.");
            string pureCompElementMassSum = "";
            Dictionary<Element, int> pureMoleLookup = pureCompound.GetElementMoles();
            foreach (Element e in pureCompound.GetElements()){
                pureCompElementMassSum += pureMoleLookup[e] + "*" + e.atomicWeight + " + ";
            }
            pureCompElementMassSum = pureCompElementMassSum[..^3];
            string impureCompElementMassSum = "";
            Dictionary<Element, int> impureMoleLookup = impureCompound.GetElementMoles();
            foreach (Element e in impureCompound.GetElements()){
                impureCompElementMassSum += impureMoleLookup[e] + "*" + e.atomicWeight + " + ";
            }
            impureCompElementMassSum = impureCompElementMassSum[..^3];
            
            double x = 0;
            if(situationNumber == 2){
                Debug.Log("sit 2");
                ConvertExpressionToDisplayLine($" y|=|g{givenElement.symbol}//g{pureCompound.properName} + g{impureCompound.properName}");
                ConvertExpressionToDisplayLine($" y|=|{pureCompound.GetElementMoles()[givenElement]}*{givenElement.atomicWeight}//({pureCompElementMassSum}) + ({impureCompElementMassSum})x");
                ConvertExpressionToDisplayLine($" Sub in {Math.Round(givenPurity*100,UIManager.SigFigs)}% for y and solve for x.");
                ConvertExpressionToDisplayLine($" {givenPurity}|=|{pureCompound.GetElementMoles()[givenElement]}*{givenElement.atomicWeight}//({pureCompElementMassSum}) + ({impureCompElementMassSum})x");
                x = (givenElement.atomicWeight*pureCompound.GetElementMoles()[givenElement]-givenPurity*pureCompound.GetMolarMass())/(impureCompound.GetMolarMass()*givenPurity);
                ConvertExpressionToDisplayLine($" x = {Math.Round(x,UIManager.SigFigs)} moles of {impureCompound.properName} for every 1 mole of {pureCompound.properName}. The ratio between these is constant for a given purity.");
            }else if (situationNumber == 3){
                Debug.Log("sit 3");
                ConvertExpressionToDisplayLine($" y|=|g{givenElement.symbol}//g{pureCompound.properName} + g{impureCompound.properName}");
                ConvertExpressionToDisplayLine($" y|=|{pureCompound.GetElementMoles()[givenElement]}*{givenElement.atomicWeight} + {impureCompound.GetElementMoles()[givenElement]}*{givenElement.atomicWeight}x//({pureCompElementMassSum}) + ({impureCompElementMassSum})x");
                ConvertExpressionToDisplayLine($" Sub in {Math.Round(givenPurity*100,UIManager.SigFigs)}% for y and solve for x.");
                ConvertExpressionToDisplayLine($" {givenPurity}|=|{pureCompound.GetElementMoles()[givenElement]}*{givenElement.atomicWeight} + {impureCompound.GetElementMoles()[givenElement]}*{givenElement.atomicWeight}x//({pureCompElementMassSum}) + ({impureCompElementMassSum})x");
                x = (givenElement.atomicWeight*pureCompound.GetElementMoles()[givenElement]-givenPurity*pureCompound.GetMolarMass())/(impureCompound.GetMolarMass()*givenPurity-givenElement.atomicWeight*impureCompound.GetElementMoles()[givenElement]);
                ConvertExpressionToDisplayLine($" x = {Math.Round(x,UIManager.SigFigs)} moles of {impureCompound.properName} for every 1 mole of {pureCompound.properName}. The ratio between these is constant for a given purity.");
                
            }
            ConvertExpressionToDisplayLine($" Calculate the \"hypothetical\" total mass of the sample using the moles calculated above.");
            double totalMass = pureCompound.GetMolarMass() + impureCompound.GetMolarMass()*x;
            
            ConvertExpressionToDisplayLine($" {pureCompound.GetMolarMass()}g{pureCompound.properName}//1mol{pureCompound.properName}|x|1mol{pureCompound.properName} in Sample|+|{impureCompound.GetMolarMass()}g{impureCompound.properName}//1mol{impureCompound.properName}|x|{x}mols{impureCompound.properName} in Sample|=|{Math.Round(totalMass,UIManager.SigFigs)}g in Sample");
            

            HashSet<Element> pureElements = pureCompound.GetElements();
            HashSet<Element> impureElements = impureCompound.GetElements();
            HashSet<Element> allElements = new();
            allElements.UnionWith(pureElements);
            allElements.UnionWith(impureElements);
            foreach(Element e in allElements){
                if (!e.Equals(givenElement)){
                    if (pureElements.Contains(e) && impureElements.Contains(e)){
                        ConvertExpressionToDisplayLine($" Find %{e.symbol}.");
                        double result = e.atomicWeight*pureCompound.GetElementMoles()[e]*(1/totalMass) + e.atomicWeight*impureCompound.GetElementMoles()[e]*(x/totalMass);
                        result = Math.Round(result*100,UIManager.SigFigs);
                        string expression = $"{e.atomicWeight}g{e.symbol}//1mol{e.symbol}|x|{pureCompound.GetElementMoles()[e]}mol{e.symbol}//1mol{pureCompound.properName}|x|1mol{pureCompound.properName}//Sample|x|Sample//{totalMass}|x|100|+|{e.atomicWeight}g{e.symbol}//1mol{e.symbol}|x|{impureCompound.GetElementMoles()[e]}mol{e.symbol}//1mol{impureCompound.properName}|x|{x}mol{impureCompound.properName}//Sample|x|Sample//{totalMass}|x|100|=|{result}%";
                        ConvertExpressionToDisplayLine(expression);
                    }else if(pureElements.Contains(e)){
                        ConvertExpressionToDisplayLine($" Find %{e.symbol}.");
                        double result = e.atomicWeight*pureCompound.GetElementMoles()[e]*(1/totalMass);
                        result = Math.Round(result*100,UIManager.SigFigs);
                        string expression = $"{e.atomicWeight}g{e.symbol}//1mol{e.symbol}|x|{pureCompound.GetElementMoles()[e]}mol{e.symbol}//1mol{pureCompound.properName}|x|1mol{pureCompound.properName}//Sample|x|Sample//{totalMass}|x|100|=|{result}%";
                        ConvertExpressionToDisplayLine(expression);
                    }else if (impureElements.Contains(e)){
                        ConvertExpressionToDisplayLine($" Find %{e.symbol}.");
                        double result = e.atomicWeight*impureCompound.GetElementMoles()[e]*(x/totalMass);
                        result = Math.Round(result*100,UIManager.SigFigs);
                        string expression = $"{e.atomicWeight}g{e.symbol}//1mol{e.symbol}|x|{impureCompound.GetElementMoles()[e]}mol{e.symbol}//1mol{impureCompound.properName}|x|{x}mol{impureCompound.properName}//Sample|x|Sample//{totalMass}|x|100|=|{result}%";
                        ConvertExpressionToDisplayLine(expression); 

                    }
                }
            }
            ConvertExpressionToDisplayLine($" Find %{pureCompound.properName}.");
            double lastResult = pureCompound.GetMolarMass()/totalMass;
            lastResult = Math.Round(lastResult*100, UIManager.SigFigs);
            string lastExpression = $" {pureCompound.GetMolarMass()}g{pureCompound.properName}//1mol{pureCompound.properName}|x|1mol{pureCompound.properName}//Sample|x|Sample//{totalMass}gSample|x|100|=|{lastResult}%";
            ConvertExpressionToDisplayLine(lastExpression);
            ConvertExpressionToDisplayLine($" Find %{impureCompound.properName}.");
            ConvertExpressionToDisplayLine($" 100% - {lastResult}%|=|{100-lastResult}%");
        }

    














        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
 



    }
    private void ConvertExpressionToDisplayLine(string expression){
        GameObject line = Instantiate(linePrefab);
        line.transform.SetParent(transform);
        string[] parts = expression.Split("|");
        foreach(string part in parts){
            if (part.Contains("//")){
                //treat as fraction
                GameObject fract = Instantiate(fractPredab);
                string[] fraction = part.Split("//");
                fract.transform.SetParent(line.transform);
                fract.GetComponent<TextMeshProUGUI>().text =  $" <u>{fraction[0]}</u>\n {fraction[1]}";


            }else{
                GameObject general = Instantiate(generalMathPrefab);
                general.transform.SetParent(line.transform);
                general.GetComponent<TextMeshProUGUI>().text = $"{part}";
                

            }
        }



    }
}
