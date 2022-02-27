using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FoxScr : MonoBehaviour
{
    GameObject player;
    NavMeshAgent nav;
    Rigidbody rb;
    Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
        nav.updateRotation = false;
        destination = transform.position;
        gameObject.tag = "Fox";
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, destination) < 1f)
        {
            // commented out code is for truly random movmement
            //destination = transform.position + new Vector3(Random.value * 20f - 10f, 0, Random.value * 20f - 10f);
            
            // chooses a random location close to the player
            destination = player.transform.position + new Vector3(Random.value * 28f - 14f, 0, Random.value * 28f - 14f);
           nav.SetDestination(destination);
        }
        Quaternion rot = transform.rotation;
        transform.LookAt(transform.position - (destination - transform.position));
        transform.rotation = Quaternion.Slerp(rot, transform.rotation, 0.05f);
    }
}
