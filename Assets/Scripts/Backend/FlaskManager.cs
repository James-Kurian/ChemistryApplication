using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FlaskManager : MonoBehaviour
{
    public int numParticlesAllowed = 10;
    public Dictionary<string, Color> compoundColor = new Dictionary<string, Color>();
    private ArrayList pureCompParticles = new ArrayList();
    private ArrayList impureCompParticles = new ArrayList();
    public GameObject ParticlePrefab;
    private PurityManager purityManager;
    public GameObject pureSpeciesHolder;
    public GameObject impureSpeciesHolder;
    public GameObject elemPercentageDisplay;
    public GameObject compPercentageDisplay;
    public GameObject MathDisplay;
    private Compound prevInputComp;
    private Element prevInputElem;
    private double prevInputPurity;


    private bool recivingPure;
    private bool recivingImpure;
    void Awake(){
        purityManager = new PurityManager();
    }
    public void UpdateBasedOnPrevious(){
        if (prevInputComp != null && (prevInputComp.Equals(purityManager.GetPureCompound()) || prevInputComp.Equals(purityManager.GetImpureCompound())))
            
            UpdateFlasks(prevInputComp, prevInputElem, prevInputPurity);
            
    }

    public void UpdateFlasks(Compound comp, Element elem, double purity){
        prevInputComp = comp;
        prevInputElem = elem;
        prevInputPurity = purity;

        bool calculationSuccessful = purityManager.CalculatePurity(comp, elem, purity);
  
        

        //Updates the slider positions and the numbers in the elementHolder
        Dictionary<Element, double> elementPurity = new Dictionary<Element, double>(purityManager.GetElementPurity());
        ElementPutiryManager[] scripts;
        if (purityManager.GetPureCompound() != null && purityManager.GetImpureCompound() != null){
            scripts = pureSpeciesHolder.GetComponent<PercentPanelHolderManager>().GetElementPurityManagers().Concat(impureSpeciesHolder.GetComponent<PercentPanelHolderManager>().GetElementPurityManagers()).ToArray();
            pureSpeciesHolder.GetComponent<PercentPanelHolderManager>().SetCompoundPurity(purityManager.GetPureCompoundPurity());
            impureSpeciesHolder.GetComponent<PercentPanelHolderManager>().SetCompoundPurity(purityManager.GetImpureCompoundPurity());       
        }else if (purityManager.GetPureCompound() != null){
            scripts = pureSpeciesHolder.GetComponent<PercentPanelHolderManager>().GetElementPurityManagers();
            pureSpeciesHolder.GetComponent<PercentPanelHolderManager>().SetCompoundPurity(purityManager.GetPureCompoundPurity());
        }else{
            scripts = impureSpeciesHolder.GetComponent<PercentPanelHolderManager>().GetElementPurityManagers();
            impureSpeciesHolder.GetComponent<PercentPanelHolderManager>().SetCompoundPurity(purityManager.GetImpureCompoundPurity());
        }
        foreach (ElementPutiryManager script in scripts){
            script.SetInputText(Math.Round(elementPurity[script.GetElement()]*100,UIManager.SigFigs)+"");      
        }

    
        //Updates the Flasks that display element and compound percentage
        if (calculationSuccessful){
            Compound pure = purityManager.GetPureCompound();
            Compound impure = purityManager.GetImpureCompound();

            if(pure == null || impure == null){
                Element fillerElement = new Element
                {
                    name = "fillerElement",
                    symbol = "?"
                };
                elementPurity.Add(fillerElement, purityManager.GetFillerElementPurity());
                if (pure == null){
                    pure = new Compound
                    {
                        properName = "Impurity"
                    };
                }
                else if (impure == null){
                    impure = new Compound
                    {
                        properName = "Impurity"
                    };
                }
            }else{
                int pureCount = pureCompParticles.Count;
                int impureCount = impureCompParticles.Count;
                if (pureCount > 0 && ((GameObject)pureCompParticles[0]).GetComponent<ParticleManager>().GetText() == "Impurity"){
                    for (int i = 0; i < pureCount; i++){
                        GameObject particle = (GameObject)pureCompParticles[0];
                        pureCompParticles.RemoveAt(0);
                        
                        Destroy(particle);
                    }
                }else if(impureCount > 0 && ((GameObject)impureCompParticles[0]).GetComponent<ParticleManager>().GetText() == "Impurity"){
            
                    for (int i = 0; i < impureCount; i++){
                        GameObject particle = (GameObject)impureCompParticles[0];
                        impureCompParticles.RemoveAt(0);
                        Destroy(particle);
                        
                    }
                }
                
            }
            elemPercentageDisplay.GetComponent<PercentageDisplay>().UpdateDisplay(elementPurity);
            compPercentageDisplay.GetComponent<PercentageDisplay>().UpdateDisplay(pure, purityManager.GetPureCompoundPurity(),impure, purityManager.GetImpureCompoundPurity());
        
        //Updates the number of particles in the flask
        
            Dictionary<Compound, Color> compColors = compPercentageDisplay.GetComponent<PercentageDisplay>().GetCompColor();
            Color color;
            bool gotColor;
            gotColor = compColors.TryGetValue(pure, out color);
            if (!gotColor) color = Color.black;
            pureCompParticles = UpdateParticles(pureCompParticles, purityManager.GetPureCompoundPurity(), pure.properName, color);

            gotColor = compColors.TryGetValue(impure, out color);
            if (!gotColor) color = Color.black;
            impureCompParticles = UpdateParticles(impureCompParticles, purityManager.GetImpureCompoundPurity(), impure.properName, color);
        }
        //Uppdates the unit analysis display
        Compound otherComp;
        if (comp.Equals(purityManager.GetImpureCompound())){
            otherComp = purityManager.GetPureCompound();
        }else{
            otherComp = purityManager.GetImpureCompound();
        }
        MathDisplay.GetComponent<DisplayMath>().UpdateDisplay(elem, purity, comp, otherComp);


       
    }
    public ArrayList UpdateParticles(ArrayList particles, double purity, string compoundName, Color color){
        if (particles.Count != 0 && ((GameObject)particles[0]).GetComponent<ParticleManager>().GetText() != compoundName){
            particles = new ArrayList();
        }
        int newNumParticles = (int)Math.Round(numParticlesAllowed * purity);
        int delta = newNumParticles - particles.Count;
        if (delta < 0){
            for (int i = 0; i < delta*-1; i++){
                GameObject particle = (GameObject)particles[0];
                particles.RemoveAt(0);
                Destroy(particle);
            }
            return particles;
        }else if (delta > 0){
            for (int i = 0; i < delta; i++){
                
                GameObject newParticle = Instantiate(ParticlePrefab, transform);
                RectTransform rect = newParticle.GetComponent<RectTransform>();
                newParticle.GetComponent<RectTransform>().anchoredPosition = new Vector2(-0.5f + rect.sizeDelta.x + UnityEngine.Random.Range(0f, 1f - 2*rect.sizeDelta.x), -0.5f + rect.sizeDelta.y + UnityEngine.Random.Range(0f, 1f - 2*rect.sizeDelta.y));
                newParticle.GetComponent<ParticleManager>().Construct(new Vector2( UnityEngine.Random.Range(-1f,1f) , UnityEngine.Random.Range(-1f,1f) ), compoundName, color);
                particles.Add(newParticle);
            }
            return particles;
        }
        return particles;
    }

    public void SetRecivingPure(){
        recivingPure = true;
        recivingImpure = false;
    }
    public void SetRecivingImpure(){
        recivingImpure = true;
        recivingPure = false;
    }
    public void SetPureOrImpureCompound(Compound c){
        if (recivingPure){
            purityManager.SetPureCompound(c);
            recivingPure = false;
        }else if (recivingImpure){
            purityManager.SetImpureCompound(c);
            recivingImpure = false;
        }

    }
    public void Remove(bool removePure){
        purityManager.Remove(removePure);
        
        int count = pureCompParticles.Count;
        for (int i = 0; i < count; i++){
            GameObject particle = (GameObject)pureCompParticles[0];
            pureCompParticles.RemoveAt(0);
            
            Destroy(particle);
        }
    
        count = impureCompParticles.Count;
        for (int i = 0; i < count; i++){
            GameObject particle = (GameObject)impureCompParticles[0];
            impureCompParticles.RemoveAt(0);
            Destroy(particle);
            
        }

    }


}



   
