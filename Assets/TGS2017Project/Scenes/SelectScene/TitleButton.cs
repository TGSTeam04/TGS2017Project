using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleButton : MonoBehaviour {

    Button m_Button;
    public Button m_PrevButton;
    Button m_Select;

	// Use this for initialization
	void Start () {
        m_Select = GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
        if (EventSystem.current.currentSelectedGameObject != this.gameObject)
        {
            m_PrevButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            m_Select.navigation.selectOnLeft.enabled = true;
            m_Select.navigation.selectOnRight.enabled = true;
        }
        else
        {
            m_Select.navigation = m_PrevButton.navigation;
            m_Select.navigation.selectOnLeft.enabled = false;
            m_Select.navigation.selectOnRight.enabled = false;
        }
    }
}
