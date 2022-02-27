using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject rock;
    int nRocks = 30;

    public GameObject flower;
    int nFlowers = 50;

    public GameObject tree;
    int nTrees = 20;

    float extent = 20;

    Vector3 randomPos()
    {
        float x = -1;
        float z = -1;
        float y = -1;
        while (y == -1)
        {
            x = Random.value * extent * 2 - extent;
            z = Random.value * extent * 2 - extent;

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(new Vector3(x, 30, z), new Vector3(0, -1, 0), out hit, Mathf.Infinity))
            {
                y = hit.point.y;
            }
        }

        return new Vector3(x, y, z);
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
