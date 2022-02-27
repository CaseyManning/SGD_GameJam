using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    PATROL,
    CHASE
}
public class FoxScr : MonoBehaviour
{
    GameObject player;
    NavMeshAgent nav;
    Rigidbody rb;
    Vector3 destination;
    private EnemyState enemyState;

    public float patrolRadiusMin = 20f, patrolRadiusMax = 60f;
    public float walkSpeed = 0.5f, runSpeed = 4f;
    public float chaseDistance = 7f, currChaseDistance;
    public float patrolItertime = 15f;
    private float patrolTimer;

    private Transform target;


    void Awake() {
        //enemy_Anim = GetComponent<EnemyAnimator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();
      //  enemy_Audio = GetComponentInChildren<EnemyAudio>();

    }

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        //nav = GetComponent<NavMeshAgent>();
        nav.updateRotation = false;
        destination = transform.position;
        gameObject.tag = "Fox";

        enemyState = EnemyState.PATROL;

        patrolTimer = patrolItertime;

        // memorize the value of chase distance so that we can put it back
        currChaseDistance = chaseDistance;
    }

    // Update is called once per frame
    void Update() {
        if (enemyState == EnemyState.PATROL) {
            Patrol();
        }
        if (enemyState == EnemyState.CHASE) {
            Chase();
        }
    }

    void Patrol() {
        // tell nav agent that he can move
        nav.isStopped = false;
        nav.speed = walkSpeed;
        // add to the patrol timer
        patrolTimer += Time.deltaTime;
        if (patrolTimer > patrolItertime) {
            SetNewRandomDestination();
            patrolTimer = 0f;
        }
        if (nav.velocity.sqrMagnitude > 0) {
            //enemy_Anim.Walk(true);
            ;
        }
        else {
            //enemy_Anim.Walk(false);
            ;
        }
        // test the distance between the player and the enemy
        if (Vector3.Distance(transform.position, target.position) <= chaseDistance) {
            //enemy_Anim.Walk(false);
            enemyState = EnemyState.CHASE;
        }
    } // patrol

    void Chase() {
        // enable the agent to move again
        nav.isStopped = false;
        nav.speed = runSpeed;
        // set the player's position as the destination
        // because we are chasing(running towards) the player
        nav.SetDestination(target.position);
        if (nav.velocity.sqrMagnitude > 0) {
            //enemy_Anim.Run(true);
            ;
        }
        else {
            //enemy_Anim.Run(false);
            ;
        }
        if (Vector3.Distance(transform.position, target.position) > chaseDistance) {
            // player run away from enemy
            // stop running
            //enemy_Anim.Run(false);
            enemyState = EnemyState.PATROL;
            // reset the patrol timer so that the function
            // can calculate the new patrol destination right away
            patrolTimer = patrolItertime;
            // reset the chase distance to previous
            if (chaseDistance != currChaseDistance) {
                chaseDistance = currChaseDistance;
            }
        } // else
    } // chase

    void SetNewRandomDestination() {
        float randRadius = Random.Range(patrolRadiusMin, patrolRadiusMax);
        Vector3 randDir = Random.insideUnitSphere * randRadius;
        randDir += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, randRadius, -1);
        nav.SetDestination(navHit.position);
    }



    /*
    void Update()
    {
        float rand_Radius = Random.Range(patrol_Radius_Min, patrol_Radius_Max);

        Vector3 randDir = Random.insideUnitSphere * rand_Radius;
        randDir += transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, rand_Radius, -1);
        destination = navHit.position;

        nav.SetDestination(destination);
        //StayOnGround();
            // commented out code is for truly random movmement
            //destination = transform.position + new Vector3(Random.value * 20f - 10f, 0, Random.value * 20f - 10f);
            
            // chooses a random location close to the player
            //destination = player.transform.position + new Vector3(Random.value * 28f - 14f, 0, Random.value * 28f - 14f);
           //nav.SetDestination(destination);
        if(Vector3.Distance(transform.position, destination) < 1f) {
            rand_Radius = Random.Range(patrol_Radius_Min, patrol_Radius_Max);

            randDir = Random.insideUnitSphere * rand_Radius;
            randDir += transform.position;

            NavMesh.SamplePosition(randDir, out navHit, rand_Radius, -1);
            destination = navHit.position;

            nav.SetDestination(destination);


            Quaternion rot = transform.rotation;
            transform.LookAt(transform.position - (destination - transform.position));
            transform.rotation = Quaternion.Slerp(rot, transform.rotation, 0.05f);
        }
    }

    void SetNewRandomDestination()
    {
        float rand_Radius = Random.Range(patrol_Radius_Min, patrol_Radius_Max);

        Vector3 randDir = Random.insideUnitSphere * rand_Radius;
        randDir += transform.position;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDir, out navHit, rand_Radius, -1);

        nav.SetDestination(navHit.position);
    }



    void StayOnGround()
    {
    Ray downwardsRay = new Ray(transform.position, Vector3.down);
    RaycastHit hit;
    if(Physics.Raycast(downwardsRay, out hit, 1.0f))
        {
            //You could check hit.collider.transform/tag to make sure
            //you're only trying to stand on the terrain
            print(hit.point);
            transform.position = hit.point;
        }
    }
    */
}
