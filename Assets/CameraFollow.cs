using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject player;
    Vector3 offset;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
    }


    public float followSharpness = 0.02f;

    void FixedUpdate()
    {

        float blend = 1f - Mathf.Pow(1f - followSharpness, Time.deltaTime * 30f);

        transform.position = Vector3.Lerp(
               transform.position,
               player.transform.position + offset,
               0.1f);
    }
}
