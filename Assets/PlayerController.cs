using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Vector3 jump;
    public float accel;
    public float maxSpeed = 2f;
    public float jumpSpeed = 40f;
    private float moveY;

    public Vector3 forwardVec;
    public Vector3 rightVec;

    public GameObject fadeOut;

    Rigidbody rb;
    Animator anim;

    GameObject boombox;

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

    public bool beenAttacked = false;

    public bool inTutorial = false;

    public float chickenPower = 0;
    float maxChickenPower = 100;

    bool convertingAll = false;

    // Start is called before the first frame update
    void Start() {
        boombox = GameObject.FindGameObjectWithTag("Boombox");
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
            if(inTutorial)
            {
                SceneManager.LoadScene("tutorial");
            }
            SceneManager.LoadScene("Death");
        }

        if (!inTutorial)
        {
            chickenPower = 0;
            foreach (GameObject ch in GameObject.FindGameObjectsWithTag("Chicken"))
            {
                chickenPower += (int)(ch.transform.localScale.x * 20);
            }
            GameObject.FindGameObjectWithTag("ChickenBar").GetComponent<RectTransform>().localScale = new Vector2((float)chickenPower / maxChickenPower, 1);
        

            if(chickenPower >= maxChickenPower)
            {
                foreach (GameObject ground in GameObject.FindGameObjectsWithTag("Ground"))
                {
                    ground.GetComponent<MeshRenderer>().materials = new Material[] { highlightmat, highlightmat };
                }

                if (Input.GetKeyDown(KeyCode.E) && isGrounded)  // convert
                {
                    foreach (GameObject helper in GameObject.FindGameObjectsWithTag("Chicken"))
                    {
                        helper.GetComponent<ChickenController>().doConvert(transform.position);
                    }
                    convertingAll = true;
                    GameObject.FindGameObjectWithTag("Boombox").GetComponent<AudioController>().PlayOneShot(1);
                    beenAttacked = false;
                    StartCoroutine(ConvertToChicken(GameObject.FindGameObjectWithTag("Ground")));
                }

            } else
            {
                foreach (GameObject ground in GameObject.FindGameObjectsWithTag("Ground"))
                {
                    ground.GetComponent<MeshRenderer>().materials = new Material[] { basemat, basemat };
                }
            }
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
            boombox.GetComponent<AudioController>().PlayOneShot(2);
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
                if (Vector3.Distance(c.transform.position, g.transform.position) < 2*convertRange * g.GetComponent<ConvertibleObj>().range)
                {
                    converters.Add(c);
                    conversionPower += 1;
                }
            }
            if(Vector3.Distance(transform.position, g.transform.position) < convertRange* g.GetComponent<ConvertibleObj>().range && conversionPower >= g.GetComponent<ConvertibleObj>().cost)
            {
                g.GetComponent<MeshRenderer>().material = highlightmat;

                if (Input.GetKeyDown(KeyCode.E) && isGrounded)  // convert
                {
                    foreach(GameObject helper in converters)
                    {
                        helper.GetComponent<ChickenController>().doConvert(g.transform.position);
                    }
                    GameObject.FindGameObjectWithTag("Boombox").GetComponent<AudioController>().PlayOneShot(1);
                    beenAttacked = false;
                    StartCoroutine(ConvertToChicken(g));
                }

            } else
            {
                g.GetComponent<MeshRenderer>().material = basemat;
            }
            
        }
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Fox"))
        {
            int conversionPower = 2;
            List<GameObject> converters = new List<GameObject>();
            foreach (GameObject c in GameObject.FindGameObjectsWithTag("Chicken"))
            {
                if (Vector3.Distance(c.transform.position, g.transform.position) < 2 * convertRange * g.GetComponent<ConvertibleObj>().range)
                {
                    converters.Add(c);
                    conversionPower += 1;
                }
            }
            if (Vector3.Distance(transform.position, g.transform.position) < convertRange * g.GetComponent<ConvertibleObj>().range && conversionPower >= g.GetComponent<ConvertibleObj>().cost)
            {
                SkinnedMeshRenderer rend = g.GetComponentInChildren<SkinnedMeshRenderer>();
                rend.materials = new Material[] { highlightmat, highlightmat, highlightmat };

                if (Input.GetKeyDown(KeyCode.E) && isGrounded)  // convert
                {
                    foreach (GameObject helper in converters)
                    {
                        helper.GetComponent<ChickenController>().doConvert(g.transform.position);
                    }
                    GameObject.FindGameObjectWithTag("Boombox").GetComponent<AudioController>().PlayOneShot(1);
                    StartCoroutine(ConvertToChicken(g));
                }

            }
            else
            {
                SkinnedMeshRenderer rend = g.GetComponentInChildren<SkinnedMeshRenderer>();
                rend.materials = new Material[] {basemat, basemat, basemat};
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

        if(convertingAll)
        {
            foreach(GameObject conv in GameObject.FindGameObjectsWithTag("Convertible"))
            {
                GameObject chicken = Instantiate(chickenPrefab);
                chicken.transform.position = conv.transform.position + new Vector3(0, 1, 0);
                chicken.transform.localScale /= 2;
                chicken.transform.localScale *= conv.GetComponent<ConvertibleObj>().scale;
                chicken.GetComponent<NavMeshAgent>().Warp(conv.transform.position);

                GameObject featherEffect = Instantiate(feathers);
                featherEffect.transform.position = conv.transform.position;

                Destroy(conv);
            }
            foreach (GameObject conv in GameObject.FindGameObjectsWithTag("Ground"))
            {
                GameObject chicken = Instantiate(chickenPrefab);
                chicken.transform.position = conv.transform.position + new Vector3(0, 1, 0);
                chicken.transform.localScale /= 2;
                chicken.transform.localScale *= 10;
                chicken.GetComponent<NavMeshAgent>().Warp(conv.transform.position);

                GameObject featherEffect = Instantiate(feathers);
                featherEffect.transform.position = conv.transform.position;

                Destroy(conv);
            }
            transform.localScale *= 30;
            StartCoroutine(zoomOut());
            yield break;
        }

        if (g != null && !beenAttacked)
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

    IEnumerator zoomOut()
    {
        Destroy(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>());
        for (int i = 0; i < 150; i++)
        {
            yield return new WaitForSeconds(0.02f);
            GameObject.FindGameObjectWithTag("MainCamera").transform.position += -GameObject.FindGameObjectWithTag("MainCamera").transform.forward * 0.2f;
        }
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            fadeOut.GetComponent<RawImage>().color = new Color(0.0625f, 0.0625f, 0.0625f, ((float)i)/100f);
        }
        fadeOut.GetComponent<RawImage>().color = new Color(0.0625f, 0.0625f, 0.0625f, 1);
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene("Victory");
    }
}