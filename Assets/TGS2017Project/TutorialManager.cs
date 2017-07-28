using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour
{
	[SerializeField]
	private PlayerController m_PlayerController;
	[SerializeField]
	private Transform m_RobotL;
	[SerializeField]
	private Transform m_RobotR;
	[SerializeField]
	private Transform m_TutorialMessage;
	[SerializeField]
	private Transform m_ComleteMessage;
	[SerializeField]
	private Transform m_TargetL;
	[SerializeField]
	private Transform m_TargetR;
	[SerializeField]
	private float m_Distance;
	[SerializeField]
	private int m_TutorialCount;
	[SerializeField]
	private int m_TutorialNumber;

	[SerializeField]
	private Image m_FadeImage;
	[SerializeField]
	private Image m_CompleteImage;
	[SerializeField]
	private Text m_TutorialText;

	[SerializeField]
	private Transform m_Enemy1;
	[SerializeField]
	private Transform m_Enemy2;
	[SerializeField]
	private Transform m_Enemy3;
	[SerializeField]
	private Transform m_Enemy4;
	[SerializeField]
	private Transform m_Enemy5;

	public Sprite m_ControllerTop;
	public Sprite m_ControllerFront;
	public Sprite m_ControllerActiveMove;
	public Sprite m_ControllerActiveBoost;
	public Sprite m_ControllerActiveJump;
	public Sprite m_ControllerActiveCombine;
	public Sprite m_ControllerActiveRocket;
	public Sprite m_ControllerActiveL;
	public Sprite m_ControllerActiveR;

	public Image m_Controller;
	public Image m_ControllerActive;

	[SerializeField]
	private Transform m_TargetBoost;
	private bool m_IsBoosted;

	public GameObject m_UIEnergy;
	public GameObject m_UIShield;

	public RectTransform m_Panel;

	private string[][] m_Text = {
		new string[] { "訓練所へようこそ" , "あなた達には\n最終試験に向けての" , "操作確認を\n行ってもらいます" , "ナーサティア、ダスラを\n操作して目的地に\n向かってください。"},
		new string[] { ""},
		new string[] { "敵を挟んで合体し\nアシュヴィンになってください。"},
		new string[] { "アシュヴィンでいられる時間は\n挟んだ敵の数で決まります","エネルギー残量は\nこちらのゲージで確認できます。" },
		new string[] { "ブースト移動を利用して\n目的地に向かってください。"},
		new string[] { "ブースト移動を使用することで\n速く移動できますが\nエネルギーを消費します。"},
		new string[] { "ジャンプをしてください。"},
		new string[] { "ジャンプをすることで\n避けられる攻撃があります。"},
		new string[] { "ロケットを発射してください。" },
		new string[] { "ロケットを発射すると\n一定量のエネルギーを消費します。" },
		new string[] { "ロケットを敵に当ててください\n右スティックで視点を操作できます。" },
		new string[] { "ロケットを当てた敵が\n壁か他の敵に当たると撃破出来ます。" },
		new string[] { "緊急回避をしてください。" },
		new string[] { "攻撃などを回避して\n反撃できるかもしれません。"},
		new string[] { "こちらの敵を二体同時に挟んで\nウプヴァスフォームに\nなってください。"},
		new string[] { "ウプヴァスフォームは\n移動速度が速く\n速い敵から逃げるのに最適です。"},
		new string[] { "蜘蛛型の敵だけを挟んで\nアヴァーフォーム\nになってください。"},
		new string[] { "アヴァーフォームは移動速度が\n遅い代わりに高い攻撃力を\n持っています"},
		new string[] { "こちらに表示されているUIは\nバリアの残量を表しています","ゲージがない状態で攻撃を\n受けてしまうと","機体が破壊され任務失敗となります\n注意してください。"},
		new string[] { "以上で訓練終了です\nお疲れさまでした。"}};
	// Use this for initialization
	IEnumerator Start()
	{
		StartCoroutine(ControllerActive());
		m_PlayerController = GameManager.Instance.m_PlayerController;
		m_RobotL = m_PlayerController.m_LRobot.transform;
		m_RobotR = m_PlayerController.m_RRobot.transform;
		m_TutorialNumber = 0;
		while (m_TutorialNumber < m_TutorialCount)
		{
			TutorialStart();
			if (m_TutorialNumber > 0)
			{
				yield return new WaitForSeconds(0.5f);
				// フェード
				for (float t = 0; t < 0.5f; t += Time.deltaTime)
				{
					m_FadeImage.color = Color.Lerp(Color.black, Color.clear, t / 0.5f);
					yield return null;
				}
				m_FadeImage.color = Color.clear;
			}
			else
			{
				yield return new WaitForSeconds(2);
			}

			//テキスト表示
			for (int i = 0; i < m_Text[m_TutorialNumber*2].Length; i++)
			{
				yield return StartCoroutine(textset(m_TutorialNumber*2,i));
			}


			if(m_TutorialNumber != 9)
			{
				m_Controller.gameObject.SetActive(true);

			}

			yield return new WaitUntil(() => Complete());
			if (m_TutorialNumber != 9)
			{
				m_Controller.gameObject.SetActive(false);

			}


			// 演出
			for (float t = 0; t < 0.5f; t += Time.deltaTime)
			{
				m_CompleteImage.fillAmount = t / 0.5f;
				yield return null;
			}
			m_CompleteImage.fillAmount = 1;

			//テキスト表示
			for (int i = 0; i < m_Text[m_TutorialNumber*2+1].Length; i++)
			{
				yield return StartCoroutine(textset(m_TutorialNumber*2+1,i));
			}

			//yield return new WaitForSeconds(1);

			if (m_TutorialNumber+1 < m_TutorialCount)
			{
				//フェード
				for (float t = 0; t < 0.5f; t += Time.deltaTime)
				{
					m_FadeImage.color = Color.Lerp(Color.clear, Color.black, t / 0.5f);
					yield return null;
				}
				m_FadeImage.color = Color.black;
			}
			m_TutorialNumber++;
		}

		GameManager.Instance.m_GameStarter.ChangeScenes(7);
	}

	private void TutorialStart()
	{
		m_Controller.gameObject.SetActive(false);
		m_CompleteImage.fillAmount = 0;
		m_FadeImage.color = Color.black;
		m_TutorialText.text = "";
		switch (m_TutorialNumber)
		{
			case 0:
				GameManager.Instance.m_PlayMode = PlayMode.TwinRobot;
				m_PlayerController.m_CanRelease = false;
				m_FadeImage.color = Color.clear;
				m_Controller.sprite = m_ControllerTop;
				m_ControllerActive.sprite = m_ControllerActiveMove;
				m_TargetBoost.gameObject.SetActive(false);
				return;
			case 1:
				m_TargetL.gameObject.SetActive(false);
				m_TargetR.gameObject.SetActive(false);
				m_Enemy1.gameObject.SetActive(true);
				m_Controller.sprite = m_ControllerFront;
				m_ControllerActive.sprite = m_ControllerActiveCombine;
				return;
			case 2:
				m_Controller.sprite = m_ControllerTop;
				m_ControllerActive.sprite = m_ControllerActiveBoost;
				m_IsBoosted = false;
				m_TargetBoost.gameObject.SetActive(true);
				m_UIEnergy.SetActive(false);
				return;
			case 3:
				m_Controller.sprite = m_ControllerTop;
				m_ControllerActive.sprite = m_ControllerActiveJump;
				m_TargetBoost.gameObject.SetActive(false);
				return;
			case 4:
				m_Controller.sprite = m_ControllerActiveL;
				m_ControllerActive.sprite = m_ControllerActiveR;
				return;
			case 5:
				m_Enemy2.gameObject.SetActive(true);
				m_Controller.sprite = m_ControllerTop;
				m_ControllerActive.sprite = m_ControllerActiveRocket;
				return;
			case 6:
				m_PlayerController.m_CanRelease = true;
				m_Controller.sprite = m_ControllerFront;
				m_ControllerActive.sprite = m_ControllerActiveCombine;
				return;
			case 7:
				m_PlayerController.m_CanRelease = false;
				m_Enemy3.gameObject.SetActive(true);
				m_Enemy4.gameObject.SetActive(true);
				m_Controller.sprite = m_ControllerFront;
				m_ControllerActive.sprite = m_ControllerActiveCombine;
				return;
			case 8:
				m_Enemy3.gameObject.SetActive(false);
				m_Enemy4.gameObject.SetActive(false);
				m_Enemy5.gameObject.SetActive(true);
				StartCoroutine(m_PlayerController.Release(false));
				m_Controller.sprite = m_ControllerFront;
				m_ControllerActive.sprite = m_ControllerActiveCombine;
				return;
			case 9:
				StartCoroutine(m_PlayerController.Release(false));
				m_UIShield.SetActive(true);
				m_Controller.enabled = false;
				m_ControllerActive.enabled = false;
				m_Panel.localPosition = new Vector3(0, -420, 0);
				return;
			default:
				return;
		}
	}

	private bool Complete()
	{
		switch (m_TutorialNumber)
		{
			case 0:
				return
					m_Distance >= Vector3.Distance(m_RobotL.position, m_TargetL.position) &&
					m_Distance >= Vector3.Distance(m_RobotR.position, m_TargetR.position);
			case 1:
				return
					m_PlayerController.m_HRobot.activeSelf;
			case 2:
				return
					m_Distance >= Vector3.Distance(m_PlayerController.m_HRobot.transform.position, m_TargetBoost.position)&&
					m_IsBoosted;
			case 3:
				return
					m_PlayerController.m_HRobot.transform.position.y>1;
			case 4:
				return
					!m_PlayerController.m_Battery.LIsCanFire ||
					!m_PlayerController.m_Battery.RIsCanFire;
			case 5:
				return
					!m_Enemy2.gameObject.activeSelf;
			case 6:
				return
					!m_PlayerController.m_HRobot.activeSelf;
			case 7:
				return
					m_PlayerController.m_HumanoidRobot.m_Config == m_PlayerController.m_HumanoidT;
			case 8:
				return
					m_PlayerController.m_HRobot.activeSelf;
			case 9:
				return true;
			default:
				return false;
		}
	}

	IEnumerator textset(int i,int j)
	{
		var textSending = m_TutorialText.GetComponent<TextSending>();
		m_TutorialText.text = m_Text[i][j];
		textSending.Initialize();
		textSending.SetRate(10);

		if (i == 3 && j == 1)
		{
			m_UIEnergy.SetActive(true);
		}
		while (!textSending.IsEnd())
		{
			textSending.SetSkip(Input.GetKeyDown(KeyCode.Space));
			m_TutorialText.SetAllDirty();
			yield return null;
		}

		float endTime = Time.time + 2;

		while (true)
		{
			float diff = endTime - Time.time;
			if (diff <= 0 || Input.GetKeyDown(KeyCode.Space)) { break; }
			yield return null;
		}
		yield return null;
	}

	private void Update()
	{
		if(m_PlayerController.m_HumanoidRobot != null)
		{
			m_PlayerController.m_HumanoidRobot.m_Energy = 100;
		}
		if(m_TutorialNumber == 2 && m_PlayerController.m_HumanoidRobot.m_IsBoost)
		{
			m_IsBoosted = true;
		}
		if (m_TutorialNumber == 7)
		{
			if(m_PlayerController.m_HRobot.activeSelf && m_PlayerController.m_HumanoidRobot.m_Config != m_PlayerController.m_HumanoidT)
			{
				StartCoroutine(m_PlayerController.Release(false));
			}
			if (!m_Enemy3.gameObject.activeSelf)
			{
				m_Enemy3.GetComponent<EnemyBase>().m_IsDead = false;
				m_Enemy3.gameObject.SetActive(true);
			}
			if (!m_Enemy4.gameObject.activeSelf)
			{
				m_Enemy4.GetComponent<EnemyBase>().m_IsDead = false;
				m_Enemy4.gameObject.SetActive(true);
			}
		}
	}

	IEnumerator ControllerActive()
	{
		while (true)
		{
			m_ControllerActive.enabled = !m_ControllerActive.enabled;
			yield return new WaitForSeconds(0.3f);
		}
	}
}
