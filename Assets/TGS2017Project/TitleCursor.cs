using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// スクリプト：タイトル画面のカーソル
/// 作成者：Ho Siu Ki（何兆祺）
/// </summary>

public class TitleCursor : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_Cursor;    // カーソル

    void Awake()
    {
        m_Cursor = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        if (selectedObject == null)
        {
            return;
        }
        m_Cursor.anchoredPosition = selectedObject.GetComponent<RectTransform>().anchoredPosition
            + new Vector2(-240.0f, 0.0f);
    }
}
