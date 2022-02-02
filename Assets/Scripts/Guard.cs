using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    GuardManager guardManager;
    AudioSource audioSrc;
    NavMeshAgent agent;
    Animator anim;

    List<Rigidbody> limbs = new List<Rigidbody>();
    List<Collider> cols = new List<Collider>();
    Collider myCol;

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip dieSound;

    [Header("Stats")]
    public float speed = 5f;
    public float slowSpeed = 3f;
    public float angularSpeed = 360f;
    public float damage = 20f;
    public float attackDelay = 0.5f;
    public float attackRange = 1f;

    [Header("Spotting")]
    public float spotInterval = 0.5f;
    public float spotRadius = 5f;
    public LayerMask playerMask;
    public LayerMask spottingMask;
    float lastTimeSpotted;

    bool attacking;
    bool isChasing;
    public bool isAlive = true;
    Transform target;

    List<Transform> waypoints = new List<Transform>();
    int waypointIndex;

    public float health = 50f;
    
    // Start is called before the first frame update
    void Start()
    {
        //Get components      
        audioSrc = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        target = FindObjectOfType<Player>().transform;
        guardManager = FindObjectOfType<GuardManager>();

        //Get limb rigidbodies
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (!limbs.Contains(rb)) limbs.Add(rb);

            rb.isKinematic = true;
        }

        //Get limb colliders
        myCol = GetComponent<Collider>();
        foreach(Collider col in GetComponentsInChildren<Collider>())
        {
            if (col != myCol)
            {
                if (!cols.Contains(col)) cols.Add(col);

                col.isTrigger = true;
            } 
        }

        //Setup stats for agent
        agent.speed = speed;
        agent.angularSpeed = angularSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            //Update animations
            anim.SetBool("Moving", IsMoving());

            //Try to spot
            if (Time.time >= lastTimeSpotted + spotInterval)
            {
                lastTimeSpotted = Time.time;

                //Check if player is near
                Vector3 checkPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                if (Physics.CheckSphere(checkPos, spotRadius, playerMask))
                {
                    if (CanSeePlayer())
                    {
                        isChasing = true;
                        SetTargetDestination(target.position);
                    }
                }
                else
                {
                    isChasing = false;
                }                    
            }

            //Get next waypoint if not chasing player
            if (!isChasing)
            {
                if (agent.remainingDistance < 0.25f)
                {
                    //Check if path is complete and start it again
                    if (waypointIndex == waypoints.Count - 1) waypointIndex = 0;
                    else waypointIndex++;

                    Debug.Log("Waypoint: " + waypoints[waypointIndex]);

                    SetTargetDestination(waypoints[waypointIndex].position);
                }
            }
            else //Try to attack
            {
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance <= attackRange)
                {
                    Attack();
                }
            }
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    bool IsMoving()
    {
        if (agent.velocity.magnitude > 0.2f)
            return true;

        return false;
    }

    public bool IsChasing()
    {
        return isChasing;
    }

    public bool CanSeePlayer()
    {
        Vector3 checkPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        Vector3 targetPos = new Vector3(target.position.x, target.position.y + 1f, target.position.z);
        Vector3 direction = (targetPos - checkPos).normalized;

        RaycastHit hit;
        if (Physics.Raycast(checkPos, direction, out hit, Mathf.Infinity, spottingMask, QueryTriggerInteraction.Ignore))
        {
            Player player = hit.collider.GetComponent<Player>();

            if (player != null) return true;
            else return false;
        }

        return false;
    }

    public void SetupPath(Transform path)
    {
        foreach(Transform waypoint in path)
        {
            waypoints.Add(waypoint);
        }

        SetTargetDestination(waypoints[0].position);
    }

    public void SetTargetDestination(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
    }

    public void Attack()
    {
        if (!attacking)
            StartCoroutine(StartAttacking());
    }

    IEnumerator StartAttacking()
    {
        //Play animation and stop guard
        anim.SetTrigger("Attack");
        agent.speed = 0;
        attacking = true;

        yield return new WaitForSeconds(attackDelay / 2);

        //Damage player

        yield return new WaitForSeconds(attackDelay / 2);

        //Continue movement
        agent.speed = speed;
        attacking = false;
    }

    public void PlayHurtAudio(bool die)
    {
        if (die)
        {
            audioSrc.PlayOneShot(dieSound);
        }
        else
        {
            audioSrc.PlayOneShot(hurtSound);
        }
    }

    public void Die()
    {
        isAlive = false;
        isChasing = false;
        anim.enabled = false;
        agent.enabled = false;
        myCol.enabled = false;

        EnableRagdoll();
    }

    public void Slowdown(float duration)
    {
        StartCoroutine(ResetWalk(duration));
    }

    IEnumerator ResetWalk(float delay)
    {
        agent.speed = slowSpeed;
        anim.SetBool("Slowed", true);

        yield return new WaitForSeconds(delay);

        agent.speed = speed;
        anim.SetBool("Slowed", false);
    }

    void EnableRagdoll()
    {
        //Make rigidbodies react to physics
        foreach(Rigidbody rb in limbs)
        {
            rb.isKinematic = false;
        }

        //Make colliders collide
        foreach(Collider col in cols)
        {
            col.isTrigger = false;
        }
    }
}
