﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void ReSpawnEnemy(GameObject m_Enemy)
	{
		StartCoroutine(ReSpawn(m_Enemy));
	}
	IEnumerator ReSpawn(GameObject m_Enemy)
	{
		yield return new WaitForSeconds(1f);
		m_Enemy.transform.position = new Vector3(Random.Range(-15f, 15f), 0.5f, Random.Range(-15f, 15f));
		m_Enemy.GetComponent<EnemyBase>().NextTarget();
		m_Enemy.SetActive(true);
		m_Enemy.GetComponent<EnemyBase>().m_IsDead = false;
		m_Enemy.transform.localScale = Vector3.one * 
			(GameManager.Instance.m_StageManger.m_StageLevel==0?1:
			GameManager.Instance.m_StageManger.m_StageLevel==1?4:15);
	}
}
