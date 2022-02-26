using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FoxScr : MonoBehaviour
{
    GameObject player;
    NavMeshAgent nav;

    Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
        nav.updateRotation = false;
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, destination) < 1f)
        {
            destination = transform.position + new Vector3(Random.value * 20f - 10f, 0, Random.value * 20f - 10f);
            nav.SetDestination(destination);
        }
        Quaternion rot = transform.rotation;
        transform.LookAt(transform.position - (destination - transform.position));
        transform.rotation = Quaternion.Slerp(rot, transform.rotation, 0.05f);
    }
}
