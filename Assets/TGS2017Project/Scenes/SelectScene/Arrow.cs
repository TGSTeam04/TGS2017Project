using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour {

    [SerializeField]
    private bool m_IsLeft;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (m_IsLeft)
        {
            if (Input.GetAxis("Horizontal") < -0.1f)
            {
                transform.localScale = new Vector3(2, 2, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            if (Input.GetAxis("Horizontal") > 0.1f)
            {
                transform.localScale = new Vector3(2, 2, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
	}
}
