using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Generator : MonoBehaviour
{
    public string rootObjectName = "Generator";
    private Transform parent;
    public GameObject rootObject;

    public virtual void Awake()
    {
        MakeRootObject();
    }

    public virtual void MakeRootObject()
    {
        rootObject = new GameObject();
        rootObject.name = rootObjectName;
    }

    public virtual void Generate()
    {
        Clear();
        // Make things
    }

    public virtual void Clear()
    {
        Destroy(rootObject);
        MakeRootObject();
    }
}
