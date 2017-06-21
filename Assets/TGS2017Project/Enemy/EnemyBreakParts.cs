using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyBreakParts : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
            GameObject.Destroy(this.gameObject, 1);
    }
}
