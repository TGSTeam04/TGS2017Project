using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private Image m_Background;
    [SerializeField]
    private Image m_Text;
    [SerializeField]
    private Camera m_Camera;

    IEnumerator Start()
    {
        m_Background.color = Color.clear;
        m_Text.color = Color.clear;
        m_Camera.depth = -2;

        yield return new WaitForSeconds(GameManager.Instance.m_LoadingAnimationTime);

        for (float t = 0; t < 3.0f; t += Time.deltaTime)
        {
            m_Background.color = Color.Lerp(Color.clear, Color.black, t);
            yield return null;
        }

        m_Camera.gameObject.SetActive(true);
        m_Camera.depth = 2;
        GameManager.Instance.m_GameStarter.RemoveScene("GameUI");
        EnemyManager.Instance.gameObject.SetActive(false);


        for (float t = 0; t < 1.0f; t += Time.deltaTime)
        {
            // m_Text.color = Color.Lerp(Color.clear, Color.white, t);
            m_Background.color = Color.Lerp(Color.black, Color.clear, t);
            yield return null;
        }

        yield return new WaitForSeconds(3);
        for (float t = 0; t < 3.0f; t += Time.deltaTime)
        {
            m_Text.color = Color.Lerp(Color.clear, Color.white, t);
            yield return null;
        }
        m_Text.color = Color.white;

        yield return new WaitForSeconds(1);
        GameManager.Instance.m_GameStarter.ChangeScenes(7);
    }
}
