using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BDecorator<T> : BNode<T> where T : BBoard
{
    public abstract bool Check();
}