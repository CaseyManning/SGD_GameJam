using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float accel;
    public float maxSpeed = 10f;

    Rigidbody rb;
    Animator anim;

    GameObject canConvert = null;

    public GameObject feathers;

    public float convertRange = 1.5f;

    public GameObject chickenPrefab;
    bool converting = false;

    public Material basemat;
    public Material highlightmat;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!converting)
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * maxSpeed;

            Vector3 refVel = Vector3.zero;
            float smoothVal = .05f;
            rb.velocity = Vector3.SmoothDamp(rb.velocity, move, ref refVel, smoothVal);
            if (move.magnitude > 0.1)
            {
                anim.SetBool("Walk", true);
                Quaternion rot = transform.rotation;
                transform.LookAt(transform.position + move);
                transform.rotation = Quaternion.Slerp(rot, transform.rotation, 0.2f);
            }
            else
            {
                anim.SetBool("Walk", false);
            }
        } else
        {
            rb.velocity = Vector3.zero;
        }

        

        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Convertible"))
        {
            if(Vector3.Distance(transform.position, g.transform.position) < convertRange)
            {
                canConvert = g;
                g.GetComponent<MeshRenderer>().material = highlightmat;
            } else
            {
                g.GetComponent<MeshRenderer>().material = basemat;
            }
        }

        if (Input.GetKey(KeyCode.Space) && canConvert != null && !converting)
        {
            
            StartCoroutine(ConvertToChicken(canConvert));
        }
    }

    IEnumerator ConvertToChicken(GameObject g)
    {
        anim.SetBool("Convert", true);
        converting = true;
        transform.LookAt(g.transform.position);


        yield return new WaitForSeconds(1);
        
        GameObject chicken = Instantiate(chickenPrefab);
        chicken.transform.position = g.transform.position;
        chicken.transform.localScale /= 2;
        chicken.transform.localScale *= g.GetComponent<ConvertibleObj>().scale;

        GameObject featherEffect = Instantiate(feathers);
        featherEffect.transform.position = g.transform.position;

        Destroy(g);
        anim.SetBool("Convert", false);
        converting = false;
    }
}
