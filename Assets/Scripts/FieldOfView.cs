using System;
using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0, 360)]
    public float angle;
    private GameManager gameManager;
    public GameObject gameOverScreen;
    public GameObject playerRef;
    public GameObject patrolPath;
    public float followSpeed;
    public float patrolSpeed;
    public float rotationSpeed;
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    private void Start()
    {
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        if (playerRef == null)
        {
            playerRef = GameObject.FindGameObjectWithTag("Player");
        }
        PopulateWaypoints();
        StartCoroutine(FOVRoutine());
    }

    private void PopulateWaypoints()
    {
        waypoints = new Transform[patrolPath.transform.childCount];
        for (int i = 0; i < patrolPath.transform.childCount; i++)
        {
            waypoints[i] = patrolPath.transform.GetChild(i);
        }
    }

    private void Update()
    {
        if (canSeePlayer)
        {
            FollowPlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;  // Return if no waypoints are set

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 movement = (targetWaypoint.position - transform.position).normalized;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetWaypoint.position, patrolSpeed * Time.deltaTime);
        transform.position = newPosition;

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Switch to the next waypoint if the current waypoint is reached
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void FollowPlayer()
    {
        Vector3 movement = (playerRef.transform.position - transform.position).normalized;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, playerRef.transform.position, followSpeed * Time.deltaTime);
        transform.position = newPosition;

        // Make the enemy face the player
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerRef)
        {
            gameManager.GameOver();
        }
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }
    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}