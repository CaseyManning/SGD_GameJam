using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenController : MonoBehaviour
{
    GameObject player;
    NavMeshAgent nav;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
        GetComponent<Animator>().SetBool("Walk", true);
    }

    // Update is called once per frame
    void Update()
    {
        nav.enabled = true;
        nav.SetDestination(player.transform.position);
        Quaternion rot = transform.rotation;
        transform.LookAt(player.transform.position);
        transform.rotation = Quaternion.Slerp(rot, transform.rotation, 0.2f);
    }
}
