using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

    AudioSource m_Audio;

	// Use this for initialization
	void Start () {
        m_Audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void PlaySE()
    {
        m_Audio.Play();
    }
}
