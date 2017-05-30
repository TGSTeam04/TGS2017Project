using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    [SerializeField]
    test tes;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(this.SafeUpdateWhileMethodBool(tes, tes.DebugFunc, new WaitForSeconds(1.0f)));
    }
}
