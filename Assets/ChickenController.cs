using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenController : MonoBehaviour
{
    GameObject player;
    NavMeshAgent nav;
    float convertingTimer = 0;
    bool converting = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
        GetComponent<Animator>().SetBool("Walk", true);
        gameObject.tag = "Chicken";

        //GameObject.FindGameObjectWithTag("Boombox").GetComponent<AudioController>().PlayOneShot(1);
        GameObject.FindGameObjectWithTag("Boombox").GetComponent<AudioController>().PlayOneShot(1);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(converting)
        {
            convertingTimer -= Time.deltaTime;
            if(convertingTimer < 0)
            {
                converting = false;
                GetComponent<Animator>().SetBool("Convert", false);
                nav.enabled = true;
            }
        } else
        {
            nav.SetDestination(player.transform.position);
            Quaternion rot = transform.rotation;
            transform.LookAt(player.transform.position);
            transform.rotation = Quaternion.Slerp(rot, transform.rotation, 0.2f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fox") {
            Destroy(gameObject);
        }
    }

    public void doConvert(Vector3 pos)
    {
        transform.LookAt(pos);
        converting = true;
        convertingTimer = 1f;
        nav.enabled = false;
        GetComponent<Animator>().SetBool("Convert", true);

    }
}
