using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageReview : MonoBehaviour {

    public List<GameObject> m_Stars;

    [SerializeField]
    private int m_Number;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (m_Number == 1)
        {
            for (int i = 0; i < ResultController.s_FirstStageLank; i++)
            {
                m_Stars[i].SetActive(true);
            }
        }
        if (m_Number == 2)
        {
            for (int i = 0; i < ResultController.s_SecondStageLank; i++)
            {
                m_Stars[i].SetActive(true);
            }
        }
        if (m_Number == 3)
        {
            for (int i = 0; i < ResultController.s_ThirdStageLank; i++)
            {
                m_Stars[i].SetActive(true);
            }
        }
    }
}
