using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Vector3 jump;
    public float accel;
    public float maxSpeed = 2f;
    public float jumpSpeed = 40f;
    public float gravity = -9.81f;
    private float moveY;

    public Vector3 forwardVec;
    public Vector3 rightVec;

    Rigidbody rb;
    Animator anim;

    bool jumping = false;

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

    int maxJumps = 5;
    int nJumps = 5;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        rb.angularVelocity = Vector3.zero;
        if (!converting)
        {
            jumpPlayer();
            movePlayer();
        } else
        {
            rb.velocity = Vector3.zero;
        }

        convertAction();

        if(transform.position.y < -5)
        {
            print("dying");
            SceneManager.LoadScene("Death");
        }
    }

    void jumpPlayer() {
        if(jumping)
        {
            rb.velocity += Time.deltaTime * Vector3.up * jumpSpeed;
            //rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.Space) && nJumps > 0)  // jump
        {
            isGrounded = false;
            nJumps--;
            StartCoroutine(Jump());
        }
    }

    void movePlayer() {
        //Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * maxSpeed;
        Vector3 move = (forwardVec * Input.GetAxis("Horizontal") + rightVec * Input.GetAxis("Vertical")).normalized * maxSpeed;

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
                slopeSpeed=Vector3.ProjectOnPlane(move, rH.normal);
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
            int conversionPower = 2;
            List<GameObject> converters = new List<GameObject>();
            foreach (GameObject c in GameObject.FindGameObjectsWithTag("Chicken"))
            {
                if (Vector3.Distance(c.transform.position, g.transform.position) < 2*convertRange)
                {
                    converters.Add(c);
                    conversionPower += 1;
                }
            }
            if(Vector3.Distance(transform.position, g.transform.position) < convertRange && conversionPower >= g.GetComponent<ConvertibleObj>().cost)
            {
                g.GetComponent<MeshRenderer>().material = highlightmat;

                if (Input.GetKey(KeyCode.E) && isGrounded)  // convert
                {
                    StartCoroutine(ConvertToChicken(g));
                }

            } else
            {
                g.GetComponent<MeshRenderer>().material = basemat;
            }
            
        }
    }

    void OnCollisionEnter() {
        isGrounded = true;
        nJumps = maxJumps;
    }

    IEnumerator ConvertToChicken(GameObject g) {
        anim.SetBool("Convert", true);
        converting = true;
        transform.LookAt(g.transform.position);

        yield return new WaitForSeconds(1);

        if (g != null)
        {
            GameObject chicken = Instantiate(chickenPrefab);
            chicken.transform.position = g.transform.position + new Vector3(0, 1, 0);
            chicken.transform.localScale /= 2;
            chicken.transform.localScale *= g.GetComponent<ConvertibleObj>().scale;
            chicken.GetComponent<NavMeshAgent>().Warp(g.transform.position);

            GameObject featherEffect = Instantiate(feathers);
            featherEffect.transform.position = g.transform.position;

            Destroy(g);
        }
        anim.SetBool("Convert", false);
        converting = false;
    }

    IEnumerator Jump() {
        jumping = true;
        yield return new WaitForSeconds(.2f);
        jumping = false;
        anim.SetBool("Jump", true);
        while(!isGrounded)
        {
            yield return new WaitForSeconds(.1f);
        }
        anim.SetBool("Jump", false);
    }
}