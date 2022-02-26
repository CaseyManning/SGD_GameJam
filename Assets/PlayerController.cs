using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float accel;
    public float maxSpeed = 10f;

    Rigidbody rb;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * maxSpeed;

        Vector3 refVel = Vector3.zero;
        float smoothVal = .05f;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, move, ref refVel, smoothVal);
        if (move.magnitude > 0.1)
        {
            anim.SetBool("Walk", true);
            Quaternion rot = transform.rotation;
            transform.LookAt(transform.position + move);
            transform.rotation = Quaternion.Slerp(rot, transform.rotation, 0.2f);
        } else
        {
            anim.SetBool("Walk", false);
        }

        if(Input.GetKey(KeyCode.Space))
        {
            print("converting");
            anim.SetBool("Convert", true);
        }
    }
}
