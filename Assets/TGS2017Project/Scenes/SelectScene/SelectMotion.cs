using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMotion : MonoBehaviour {

    public Vector3 m_Position;
    public Quaternion m_Rotation;

    Image m_Image;
    public Sprite m_Sprite;

    public bool m_Diffusion = true;
    public bool m_IsStop = false;
    public bool m_IsRed = false;

    Animator m_Anim;

	// Use this for initialization
	void Start () {
        m_Image = this.gameObject.GetComponentInChildren<Image>();
        m_Anim = GetComponentInChildren<Animator>();
        if (Random.Range(1, 2) == 1)
        {
            m_Anim.speed = Random.Range(0.5f, 1.0f);
        }
        else if (Random.Range(1, 2) == 2)
        {
            m_Anim.speed = Random.Range(-0.5f, -1.0f);
        }
        NextPosition();
	}
	
	// Update is called once per frame
	void Update () {
        print(m_Image.sprite.ToString());
        if (m_IsRed)
        {
            m_Image.color = Color.red;
        }
        if (m_Diffusion)
        {
            if (Vector3.Distance(transform.localPosition, m_Position) > 0.1f || Mathf.Abs(Quaternion.Angle(transform.localRotation, m_Rotation)) > 0.1f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, m_Position, 3f * Time.deltaTime);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, m_Rotation, 2f * Time.deltaTime);
            }
            else
            {
                transform.localPosition = m_Position;
                transform.localRotation = m_Rotation;
                enabled = false;
            }
        }

        m_Anim.SetBool("IsStop", m_IsStop);
	}

    public void NextPosition()
    {
        if (m_Diffusion)
        {
            m_Position = RandomPosition();
            m_Rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
    public void StarPosition()
    {
        m_Position = new Vector3(Random.Range(-210.0f, 210.0f), Random.Range(650.0f, 700.0f));
        m_Rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-3600.0f, 3600.0f)));
    }
    public Vector3 RandomPosition()
    {
        Vector3 position = Vector3.zero;
        do
        {
            position = new Vector3(Random.Range(-450.0f, 450.0f), Random.Range(-250.0f, 250.0f));
        } while ((position.x > -300 && position.x < 300) && (position.y > -400 && position.y < 150));
        return position;
    }
    public void ChangeColor(Color color)
    {
        m_Image.color = color;
    }
    public void NormalColor()
    {
        m_Image.color = Color.white;
    }
    public void ChangeSprite()
    {
        m_Image.sprite = m_Sprite;
    }
}
