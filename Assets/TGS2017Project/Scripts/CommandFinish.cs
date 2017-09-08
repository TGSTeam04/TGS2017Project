using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommandFinish : MonoBehaviour {

    private float m_EndCount = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Back") && Input.GetButton("Home"))
        {
            Application.Quit();
        }

        if (!Input.anyKey)
        {
            m_EndCount += Time.deltaTime;
            if (m_EndCount > 120) Application.Quit();
        }
        else
        {
            m_EndCount = 0;
        }
    }
}
