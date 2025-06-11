using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompoundAdderKeyDet : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject buttonAdder;
    void Start()
    {
        gameObject.GetComponent<TMPro.TMP_InputField>().onEndEdit.AddListener(HandleEndEdit);
    }

    public void HandleEndEdit(string text){
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
                buttonAdder.GetComponent<SelectionManager>().addButton();
            }
    }
}
