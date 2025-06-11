using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ParticleManager : MonoBehaviour
{
    public TMP_Text TextArea;
    Rigidbody2D rb;
   
    
    public void Construct(Vector2 vel, string text, Color color){
        TextArea.text = text;
        TextArea.color = color;
        rb.velocity = vel;
        
    }
    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate(){
        
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.name == "Left" || other.name == "Right"){
            rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
        } else if (other.name == "Top" || other.name == "Bottom"){
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y* -1 );
        }
        
        
    }
    public string GetText(){
        return TextArea.text;
    }
}
