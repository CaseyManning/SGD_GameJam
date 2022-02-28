using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum EnemyState
{
    PATROL,
    CHASE,
    ATTACK
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

    float attackRange = 1.8f;

    float visionRadius = 4f;

    float attackCooldown = -1;
    float maxRadius = 30;
    NavMeshHit navHit;

    Quaternion startRot;

    GameObject attackTarget;

    public GameObject feathers;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //nav.updateRotation = false;
        //destination = transform.position;

        state = EnemyState.PATROL;

        patrolTimer = timerMax;

    }

    private void Update()
    {

        if(state == EnemyState.PATROL)
        {
            nav.speed = 2;
            patrolTimer -= Time.deltaTime;
            //if(Vector3.Distance(transform.position, destination) < 1f || patrolTimer < 0)
            if (patrolTimer < 0)
            {
                destination = SetNewRandomDestination();
            } else 
                {
                    if (Vector3.Distance(transform.position, destination) <= 1f)
                {
                    GetComponent<Animator>().SetBool("Idle", true);
                } else {
                    GetComponent<Animator>().SetBool("Idle", false);
                }
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
            nav.speed = 3;
            if(target == null)
            {
                state = EnemyState.PATROL;
                destination = SetNewRandomDestination();
            }

            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Chicken"))
            {
                if (Vector3.Distance(g.transform.position, transform.position) < Vector3.Distance(target.transform.position, transform.position)-0.5)
                {
                    target = g;
                }
            }

            nav.SetDestination(target.transform.position);
            if(Vector3.Distance(target.transform.position, transform.position) > visionRadius * 1.5f)
            {
                state = EnemyState.PATROL;
                destination = SetNewRandomDestination();
            }

            //GameObject[] chickens = GameObject.FindGameObjectsWithTag("Chicken");
            //foreach (GameObject ch in chickens)
            //{
            //    if (Vector3.Distance(ch.transform.position, transform.position) < attackRange && attackCooldown < 0f)
            //    {
            //        attack(ch);
            //        return;
            //    }
            //}
            if (Vector3.Distance(target.transform.position, transform.position) < attackRange && attackCooldown < 0f)
            {
                attack(target);
                return;
            }
        }

       
        if(state == EnemyState.ATTACK)
        {
            if(attackTarget == null)
            {
                state = EnemyState.PATROL;
                return;
            }
            transform.LookAt(transform.position - (attackTarget.transform.position - transform.position));
            transform.rotation = startRot;
            attackCooldown -= Time.deltaTime;
            if(attackCooldown < 0)
            {
                nav.enabled = true;
                destination = SetNewRandomDestination();
                GameObject f = Instantiate(feathers);
                f.transform.position = attackTarget.transform.position;
                if (attackTarget.CompareTag("Player"))
                {
                    GameObject[] others = GameObject.FindGameObjectsWithTag("Chicken");
                    if(others.Length == 0)
                    {
                        Destroy(attackTarget.GetComponentInChildren<SkinnedMeshRenderer>());
                        StartCoroutine(endgame());
                    } else
                    {
                        player.GetComponent<PlayerController>().beenAttacked = true;
                        player.transform.position = others[0].transform.position;
                        Destroy(others[0]);
                    }
                }
                else
                {
                    Destroy(attackTarget);
                }
                state = EnemyState.PATROL;

                GameObject.FindGameObjectWithTag("Boombox").GetComponent<AudioController>().PlayOneShot(0);
            }
        }
       
    }

    IEnumerator endgame()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Death");
    }

    void attack(GameObject chicken)
    {
        startRot = transform.rotation;
        state = EnemyState.ATTACK;
        nav.speed = 0;

        GetComponent<Animator>().SetTrigger("Attack");
        attackCooldown = .5f;
        attackTarget = chicken;
    }

    Vector3 SetNewRandomDestination() {
        //float randRadius = Random.Range(20, 30);
        int noFreeze = 0;
        // re-sample dest points until one is decently far away
        while ((Vector3.Distance(transform.position, destination) < 2.5f) && (noFreeze < 100)) {
            Vector3 randDir = Random.insideUnitSphere * maxRadius;
            randDir += transform.position;
            NavMesh.SamplePosition(randDir, out navHit, maxRadius, -1);
            noFreeze++;
        }
        nav.SetDestination(navHit.position);
        return navHit.position;
    }
}
