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
    GameObject target;
    NavMeshAgent nav;
    Rigidbody rb;
    Vector3 destination;
    private EnemyState state;

    private float patrolTimer;
    float timerMax = 5;


    float visionRadius = 5f;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nav.updateRotation = true;
        destination = transform.position;

        state = EnemyState.PATROL;

        patrolTimer = timerMax;

    }

    private void Update()
    {
        if(state == EnemyState.PATROL)
        {
            patrolTimer -= Time.deltaTime;
            if(Vector3.Distance(transform.position, destination) < 1f || patrolTimer < 0)
            {
                SetNewRandomDestination();
            }

            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Chicken"))
            {
                if (Vector3.Distance(g.transform.position, transform.position) < visionRadius)
                {
                    state = EnemyState.CHASE;
                    target = g;
                }
            }
            if (Vector3.Distance(player.transform.position, transform.position) < visionRadius)
            {
                state = EnemyState.CHASE;
                target = player;
            }
        }
        if(state == EnemyState.CHASE)
        {
            if(target == null)
            {
                state = EnemyState.PATROL;
            }
            nav.SetDestination(target.transform.position);
            if(Vector3.Distance(target.transform.position, transform.position) > visionRadius * 1.5f)
            {
                state = EnemyState.PATROL;
            }
        }
        
    }

    void SetNewRandomDestination() {
        float randRadius = Random.Range(5, 10);
        Vector3 randDir = Random.insideUnitSphere * randRadius;
        randDir += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, randRadius, -1);
        nav.SetDestination(navHit.position);
    }
}
