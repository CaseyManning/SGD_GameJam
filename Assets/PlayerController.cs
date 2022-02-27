using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 jump;
    public float accel;
    public float maxSpeed = 2f;
    public float jumpSpeed = 40f;
    public float gravity = -9.81f;
    private float moveY;

    Rigidbody rb;
    Animator anim;

    GameObject canConvert = null;

    public GameObject feathers;

    public float convertRange = 1.5f;

    public GameObject chickenPrefab;
    bool converting = false;
    bool isGrounded = true;

    public Material basemat;
    public Material highlightmat;
    // for walking up slopes
    private RaycastHit rH;
    [SerializeField]
    private float angSpd=5;
    [SerializeField]
    private Vector3 slopeSpeed;


    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        if (!converting)
        {
            jumpPlayer();
            rotacionarCamera();
            movePlayer();
        } else
        {
            rb.velocity = Vector3.zero;
        }

        convertAction();
    }

    void jumpPlayer() {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)  // jump
        {
            isGrounded = false;
            StartCoroutine(Jump());
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
    }

    void movePlayer() {
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
            if (isGrounded) {
                // code for walking up slopes
                // source: https://forum.unity.com/threads/character-movement-and-slopes.290381/
                Physics.Raycast(transform.position,-transform.up,out rH,Mathf.Infinity);
                slopeSpeed=Vector3.ProjectOnPlane(transform.forward, rH.normal);
                slopeSpeed=Vector3.Normalize(slopeSpeed);
                rb.velocity = slopeSpeed*maxSpeed;
            }
        }
        else
        {
            anim.SetBool("Walk", false);
        }
    }

    void convertAction() {
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

        if (Input.GetKey(KeyCode.E) && canConvert != null && !converting && isGrounded)  // convert
        {
            StartCoroutine(ConvertToChicken(canConvert));
        }
    }

    void OnCollisionEnter() {
        isGrounded = true;
    }
    private void rotacionarCamera() {
        rb.angularVelocity=new Vector3(0,Input.GetAxis("Mouse X")*angSpd,0);
    }

    IEnumerator ConvertToChicken(GameObject g) {
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

    IEnumerator Jump() {
        //anim.SetBool("Jump", true);
        yield return new WaitForSeconds(.5f);
        //anim.SetBool("Jump", false);
    }
}