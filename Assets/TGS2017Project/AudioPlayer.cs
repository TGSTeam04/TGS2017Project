using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

    AudioSource m_Audio;

    bool m_IsPlay = false;

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
    public void PlaySE2()
    {
        if (m_IsPlay != true)
        {
            m_Audio.Play();
        }
        m_IsPlay = true;
    }
}
