using UnityEngine;
using System.Collections;

public class Emotions : MonoBehaviour{

    Vector2 direction;
    Collider2D current;
    bool escape;
    static GameObject menu;
    int counter;
    int counter1 = 5;
    

    public GUIpopUP pop;

 
    void OnMouseDown()
    {
        if (counter == counter1)
        {
            pop.enabled = false;
            Debug.Log("counter: " + counter);
            Debug.Log("counter1:  " + counter1);
            counter = 1;
            counter1 = 5;
        }
        else if (counter != counter1)
        {
            pop.enabled = true;
            Debug.Log("Counter:  " + counter);
            counter = 5;
            counter1 = 5;     
        }
        
    }

   



    void activate()
    {
        //Cast a ray in the direction specified in the inspector.
        RaycastHit2D hit = Physics2D.Raycast(this.gameObject.transform.position, direction);
        
        
            //Display the point in world space where the ray hit the collider's surface.
            pop.enabled = true;
          
   
        current = hit.collider;
     }
}