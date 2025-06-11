using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PercentageDisplay : MonoBehaviour
{
    public GameObject prefab;
    private ArrayList elemDisplays = new ArrayList();
    private Dictionary<Element, Color> elemColors = new Dictionary<Element, Color>();
    private Dictionary<Compound, Color> compColors = new Dictionary<Compound, Color>();
    private GameObject[] compDisplays = new GameObject[2];

    public void UpdateDisplay(Dictionary<Element, double> elementPurity){
        if (gameObject.tag == "ElemDis"){
            foreach (GameObject display in elemDisplays){
                Destroy(display);
            }
            RectTransform container = GetComponent<RectTransform>();
            double containerWidth = container.rect.width;
            double containerHeight = container.rect.height;
            float heightBelow = 0;
            foreach (KeyValuePair<Element, double> pair in elementPurity){
                GameObject display = Instantiate(prefab, transform);
                elemDisplays.Add(display);
                display.GetComponentInChildren<TMP_Text>().text = pair.Key.symbol+": "+System.Math.Round(pair.Value*100,UIManager.SigFigs)+"%";
                RectTransform scale = display.GetComponent<RectTransform>();
                
                float targetHeight = (float)(pair.Value * containerHeight);
                
                scale.sizeDelta = new Vector2((float)containerWidth, (float)targetHeight);
                if (pair.Key.symbol == "?"){
                    display.GetComponent<Image>().color = Color.black;
                }else if (elemColors.ContainsKey(pair.Key)){
                    display.GetComponent<Image>().color = elemColors[pair.Key];
                }else{
                    Color color = new Color(Random.Range(0f, 1f),Random.Range(0f, 1f),Random.Range(0f, 1f));
                    display.GetComponent<Image>().color = color;
                    elemColors.Add(pair.Key, color);
                }
                scale.anchoredPosition = new Vector2(0, heightBelow);
              
                heightBelow += targetHeight;
            }
        }
    }
    public void UpdateDisplay(Compound pure, double purePurity, Compound impure, double impurePurity){
        if (gameObject.CompareTag("CompDis"))
        {
            RectTransform container = GetComponent<RectTransform>();
            double containerWidth = container.rect.width;
            double containerHeight = container.rect.height;
            float targetHeight = 0;
            if (impure != null){
                Destroy(compDisplays[0]);
                
                
                GameObject display = Instantiate(prefab, transform);
                compDisplays[0] = display;
                RectTransform scale = display.GetComponent<RectTransform>();
                targetHeight = (float)(impurePurity * containerHeight);
                scale.sizeDelta = new Vector2((float)containerWidth, (float)targetHeight);
                if (compColors.ContainsKey(impure)){
                    display.GetComponent<Image>().color = compColors[impure];
                }else if (impure.properName == "Impurity"){
                    display.GetComponent<Image>().color = new Color(0,0,0);
                }else{
                    Color color = new Color(Random.Range(0f, 1f),Random.Range(0f, 1f),Random.Range(0f, 1f));
                    display.GetComponent<Image>().color = color;
                    compColors.Add(impure, color);
                }
                // scale.anchoredPosition = new Vector2((float)(0.5*containerWidth), (float)(0.5*targetHeight));
                // scale.anchoredPosition = new Vector2(0,0);
                // scale.anchoredPosition = new Vector2(0, heightBelow);

                display.GetComponentInChildren<TMP_Text>().text = impure.properName+": "+System.Math.Round(impurePurity*100,UIManager.SigFigs)+"%";
            }
            if (pure != null){
                Destroy(compDisplays[1]);
                GameObject display2 = Instantiate(prefab, transform);
                compDisplays[1] = display2;
                RectTransform scale2 = display2.GetComponent<RectTransform>();
                double prevTargetHeight = targetHeight;
                targetHeight = (float)(purePurity * containerHeight);
                scale2.sizeDelta = new Vector2((float)containerWidth, (float)targetHeight);
                if (compColors.ContainsKey(pure)){
                    display2.GetComponent<Image>().color = compColors[pure];
                }else if (pure.properName == "Impurity"){
                    display2.GetComponent<Image>().color = Color.black;
                }else{
                    Color color = new Color(Random.Range(0f, 1f),Random.Range(0f, 1f),Random.Range(0f, 1f));
                    display2.GetComponent<Image>().color = color;
                    compColors.Add(pure, color);
                }            
                // scale2.anchoredPosition = new Vector2((float)(0.5*containerWidth), (float)(0.5*targetHeight + prevTargetHeight));
                // scale2.anchoredPosition = new Vector2(0,0);
                scale2.anchoredPosition = new Vector2(0, (float)prevTargetHeight);
              
                display2.GetComponentInChildren<TMP_Text>().text = pure.properName+": "+System.Math.Round(purePurity*100,UIManager.SigFigs)+"%";
            }
        }
    
    }
    public Dictionary<Compound, Color> GetCompColor(){
        return compColors;
    }

}
