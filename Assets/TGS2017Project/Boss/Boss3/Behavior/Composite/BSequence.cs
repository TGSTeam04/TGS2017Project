﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSequence : BComposite
{
    public BSequence()
    {
        NodeName = "sequence";
    }
    public override void ChildSuccess()
    {
        m_NowIndex++;
        if (m_NowIndex >= m_Child.Count)
        {            
            Succes();
        }
    }
    public override void ChildFailure()
    {
        Failure();
    }
}