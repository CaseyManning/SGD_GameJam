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

        //transform.position = Vector3.Lerp(
        //       transform.position,
        //       player.transform.position + offset,
        //       0.1f);

        transform.position = player.transform.position + offset;

        if (Input.GetMouseButton(0))
        {
            //transform.Rotate(new Vector3(0, -Input.GetAxis("Mouse X") * 3f, 0));
            transform.RotateAround(player.transform.position, new Vector3(0, 1, 0), Input.GetAxis("Mouse X") * 6f);
            offset = transform.position - player.transform.position;

            Vector3 vec = transform.forward;
            vec = new Vector3(vec.x, 0, vec.z);
            vec.Normalize();

            player.GetComponent<PlayerController>().rightVec = vec;

            Vector3 rvec = transform.right;
            rvec = new Vector3(rvec.x, 0, rvec.z);
            rvec.Normalize();

            player.GetComponent<PlayerController>().forwardVec = rvec;
        }
    }
}
