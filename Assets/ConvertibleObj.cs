using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertibleObj : MonoBehaviour
{
    public int scale = 1;
    public int cost;
    // Start is called before the first frame update
    void Start()
    {
        cost = 2 * scale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
