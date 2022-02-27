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
        destination = transform.position;

        state = EnemyState.PATROL;

        patrolTimer = timerMax;

    }

    private void Update()
    {

        if(state == EnemyState.PATROL)
        {
            nav.speed = 2;
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
            nav.speed = 3;
            if(target == null)
            {
                state = EnemyState.PATROL;
                SetNewRandomDestination();
            }
            nav.SetDestination(target.transform.position);
            if(Vector3.Distance(target.transform.position, transform.position) > visionRadius * 1.5f)
            {
                state = EnemyState.PATROL;
                SetNewRandomDestination();
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
            transform.LookAt(transform.position - (attackTarget.transform.position - transform.position));
            transform.rotation = startRot;
            attackCooldown -= Time.deltaTime;
            if(attackCooldown < 0)
            {
                nav.enabled = true;
                SetNewRandomDestination();
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
        yield return 2;
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

    void SetNewRandomDestination() {
        float randRadius = Random.Range(5, 10);
        Vector3 randDir = Random.insideUnitSphere * randRadius;
        randDir += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, randRadius, -1);
        nav.SetDestination(navHit.position);
    }
}
