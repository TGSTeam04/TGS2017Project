using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private bool m_IsCanDestroy;
    [HideInInspector]
    public int m_UseStageLevel;
    public void Start()
    {
        print(name + " is " + LayerMask.LayerToName(gameObject.layer) + " layer");
    }

    private void Update()
    {
    }

    public bool CheckNecessity()
    {
        LayerMask mask = LayerMask.GetMask(new string[] { "Wall" });
        if (Physics.Raycast(transform.position, transform.forward, Mathf.Infinity, mask)
        && Physics.Raycast(transform.position, -transform.forward, Mathf.Infinity, mask))
        {
            m_IsCanDestroy = true;
            GetComponent<MeshRenderer>().material.color = Color.red;
            return false;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 1000, Color.red, Mathf.Infinity);
            Debug.DrawRay(transform.position, -transform.forward * 1000, Color.red, Mathf.Infinity);
            return true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {        
        if (m_IsCanDestroy && collision.transform.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
