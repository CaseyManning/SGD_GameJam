using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject rock;
    int nRocks = 50;

    public GameObject flower;
    int nFlowers = 200;

    public GameObject tree;
    int nTrees = 20;

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
            nFlower.transform.Rotate(new Vector3(0, 0, Random.value * 360));
        }

        for (int i = 0; i < nRocks; i++)
        {
            Vector3 pos = randomPos();
            GameObject nRock = Instantiate(rock);
            nRock.transform.position = pos;
            nRock.transform.Rotate(new Vector3(0, 0, Random.value * 360));
        }
        for (int i = 0; i < nTrees; i++)
        {
            Vector3 pos = randomPos();
            GameObject nTree = Instantiate(tree);
            nTree.transform.position = pos;
            nTree.transform.Rotate(new Vector3(0, 0, Random.value * 360));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
