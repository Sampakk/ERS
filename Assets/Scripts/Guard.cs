using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    AudioSource audioSrc;
    NavMeshAgent agent;
    Animator anim;

    List<Rigidbody> limbs = new List<Rigidbody>();
    List<Collider> cols = new List<Collider>();
    Collider myCol;

    [Header("Audio")]
    public AudioClip bonkSound;
    public AudioClip hurtSound;
    public AudioClip dieSound;

    [Header ("Chasing")]
    public AudioClip[] chaseSounds;

    [Header("Footsteps")]
    public AudioClip[] footsteps;
    public float footstepDelay = 0.4f;
    float timeToNextFootstep;

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float slowedSpeed = 2f;
    public float angularSpeed = 360f;

    [Header("Attacking")]
    public float damage = 20f;
    public float attackDelay = 0.5f;
    public float attackRange = 1f;

    [Header("Spotting")]
    public float spotInterval = 0.5f;
    public float spotRadius = 5f;
    public float chasingDelay = 2f;
    public LayerMask playerMask;
    public LayerMask spottingMask;
    float lastTimeSpotted;
    float lastTimeChased;

    [Header("Vitals")]
    public bool isAlive = true;

    Transform target;
    bool attacking;
    bool isChasing;
    bool isSlowed;

    List<Transform> waypoints = new List<Transform>();
    int waypointIndex;

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        audioSrc = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        target = FindObjectOfType<Player>().transform;

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
        agent.speed = walkSpeed;
        agent.angularSpeed = angularSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        AI();

        Footsteps();
    }

    void AI()
    {
        if (isAlive)
        {
            //Try to spot, update animations & speed
            if (Time.time >= lastTimeSpotted + spotInterval)
            {
                lastTimeSpotted = Time.time;

                //Check if player is near
                Vector3 localForward = transform.forward.normalized;
                Vector3 checkPos = new Vector3(transform.position.x + localForward.x * (spotRadius * 0.5f), transform.position.y + 1f, transform.position.z + localForward.z * (spotRadius * 0.5f));
                if (Physics.CheckSphere(checkPos, spotRadius, playerMask))
                {
                    if (CanSeePlayer())
                    {
                        //Alert gamemanager
                        GameManager game = FindObjectOfType<GameManager>();
                        if (game != null && !game.alerted) 
                            game.alerted = true;

                        //Chase audio
                        if (!isChasing)
                            PlayChaseAudio();

                        //Start chasing player, target destination is player position
                        isChasing = true;
                        agent.speed = runSpeed;
                        SetTargetDestination(target.position);

                        //Update chase timer
                        lastTimeChased = Time.time;
                    }
                    else
                    {
                        //Chase for short duration after player was lost
                        if (Time.time > lastTimeChased + chasingDelay)
                            isChasing = false;
                    }
                }
                else
                {
                    //Chase for short duration after player was lost
                    if (Time.time > lastTimeChased + chasingDelay)
                        isChasing = false;
                }

                //Update animations
                anim.SetBool("Moving", IsMoving());
                anim.SetBool("Chasing", IsChasing());

                //Update speed
                if (isSlowed)
                {
                    agent.speed = slowedSpeed;
                }
                else
                {
                    if (isChasing) agent.speed = runSpeed;
                    else agent.speed = walkSpeed;
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
    }

    void Footsteps()
    {
        if (IsMoving())
        {
            //Check speed
            float speedMult = 1f;
            if (isSlowed) speedMult *= 0.5f;
            else if (isChasing) speedMult *= 1.2f;

            //Timer
            timeToNextFootstep -= speedMult * Time.deltaTime;

            if (timeToNextFootstep <= 0)
            {
                timeToNextFootstep = footstepDelay;

                //Play audio
                int random = Random.Range(0, footsteps.Length);
                audioSrc.PlayOneShot(footsteps[random], 0.5f);
            }
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
        Vector3 direction = (targetPos - checkPos);
        float distance = direction.magnitude;
        direction = direction.normalized;

        RaycastHit hit;
        if (Physics.Raycast(checkPos, direction, out hit, Mathf.Infinity, spottingMask, QueryTriggerInteraction.Ignore))
        {
            //Try to get player component so we know did we see it
            Player player = hit.collider.GetComponent<Player>();
            if (player == null) player = hit.collider.GetComponentInParent<Player>();

            //Return true or false
            if (player != null)
            {
                if (player.IsCrouched() && distance <= spotRadius / 2 * 0.5f) return true;
                else return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public void SetupPath(Transform path)
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        foreach(Transform waypoint in path)
        {
            waypoints.Add(waypoint);
        }

        SetTargetDestination(waypoints[0].position);
    }

    public void SetTargetDestination(Vector3 targetPos)
    {
        if (agent.enabled)
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
        agent.speed = runSpeed;
        attacking = false;
    }

    public void PlayHurtAudio(bool die)
    {
        //Hurt or death
        if (die) audioSrc.PlayOneShot(dieSound);
        else audioSrc.PlayOneShot(hurtSound);

        //Bonk!
        audioSrc.PlayOneShot(bonkSound);
    }

    public void PlayChaseAudio()
    {
        int random = Random.Range(0, chaseSounds.Length);
        audioSrc.PlayOneShot(chaseSounds[random]);
    }

    public void Die()
    {
        isAlive = false;
        isChasing = false;
        anim.enabled = false;
        agent.enabled = false;
        myCol.enabled = false;

        PlayHurtAudio(true);

        EnableRagdoll(false);

        StartCoroutine(ResetAlive(5f));
    }

    public void Slowdown(float duration)
    {
        StartCoroutine(ResetWalk(duration));

        PlayHurtAudio(true);
    }

    IEnumerator ResetWalk(float delay)
    {
        isSlowed = true;
        agent.speed = slowedSpeed;
        anim.SetBool("Slowed", true);

        yield return new WaitForSeconds(delay);

        agent.speed = (isChasing) ? runSpeed : walkSpeed;

        isSlowed = false;
        anim.SetBool("Chasing", isChasing);
        anim.SetBool("Slowed", false);
    }

    IEnumerator ResetAlive(float delay)
    {
        yield return new WaitForSeconds(delay);

        isAlive = true;
        isChasing = false;
        anim.enabled = true;
        agent.enabled = true;
        myCol.enabled = true;

        EnableRagdoll(true);
    }

    void EnableRagdoll(bool state)
    {
        //Make rigidbodies react to physics
        foreach(Rigidbody rb in limbs)
        {
            rb.isKinematic = state;
        }

        //Make colliders collide
        foreach(Collider col in cols)
        {
            col.isTrigger = state;
        }
    }

    void OnDrawGizmos()
    {
        Vector3 localForward = transform.forward.normalized;
        Vector3 checkPos = new Vector3(transform.position.x + localForward.x * (spotRadius * 0.5f), transform.position.y + 1f, transform.position.z + localForward.z * (spotRadius * 0.5f));

        Gizmos.DrawWireSphere(checkPos, spotRadius);
    }
}
