using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Searching : MonoBehaviour {

    public static bool s_Find = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            s_Find = true;
        }
    }
}
