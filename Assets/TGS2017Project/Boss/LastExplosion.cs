using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastExplosion : MonoBehaviour {

    public List<GameObject> m_SmallExplosion;
    public GameObject m_BigExplosion;

    int m_Count;

    bool m_Dead;

	// Use this for initialization
	void Start () {
        StartCoroutine(Explosion());
	}
	
	// Update is called once per frame
	void Update () {
		if (m_Dead)
        {
            Destroy(gameObject);
        }
        //print(m_Dead);
	}
    IEnumerator Explosion()
    {
        m_Count = 0;
        while(m_Count < m_SmallExplosion.Count)
        {
            yield return new WaitForSeconds(0.5f);
            m_SmallExplosion[m_Count].SetActive(true);
            m_Count++;
        }
        Instantiate(m_BigExplosion, transform.position, transform.rotation);
        m_Dead = true;
    }
}
