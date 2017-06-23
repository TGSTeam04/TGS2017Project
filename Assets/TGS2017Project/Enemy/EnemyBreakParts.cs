using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyBreakParts : MonoBehaviour
{    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            StartCoroutine(this.Delay(new WaitForSeconds(1.0f), ()
                => Destroy(gameObject)
            ));
        }            
    }       
}
