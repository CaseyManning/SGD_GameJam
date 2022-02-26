using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject rock;

    public GameObject flower;
    int nFlowers = 200;

    float extent = 20;

    Vector3 randomPos()
    {
        float x = Random.value * extent * 2 - extent;
        float z = Random.value * extent * 2 - extent;
        return new Vector3(x, 0, z);
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < nFlowers; i++)
        {
            Vector3 pos = randomPos();
            GameObject nFlower = Instantiate(flower);
            nFlower.transform.position = pos;
            nFlower.transform.Rotate(new Vector3(0, Random.value * 360, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
