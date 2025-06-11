using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorManager : MonoBehaviour
{
    private TMP_Text log;
    public static ErrorManager instance;
    void Awake(){
        instance = this;
        log = GetComponent<TMP_Text>();
    }
    public void Log(string str){
        log.text = str;
    }
    public void Clear(){
        log.text = "";
    }
}
