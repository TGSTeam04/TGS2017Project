using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class testinput : MonoBehaviour {

	public Image m_image;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		m_image.fillAmount = (Input.GetAxis("RotateL")+1)/2.0f;
	}
}
