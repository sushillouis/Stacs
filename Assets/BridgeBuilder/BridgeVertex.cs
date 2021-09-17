using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeVertex : MonoBehaviour
{
    public int id;
    public List<BridgeEdge> edges;

    private void Awake()
    {
        edges = new List<BridgeEdge>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
