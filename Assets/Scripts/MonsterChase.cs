using JetBrains.Annotations;
using UnityEngine;

public class MonsterChase : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float obstacleDetectionDistance = 2f;
    [SerializeField] private float avoidanceAngle = 45f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Player Look Settings")]
    [SerializeField] private float lookAtDistance = 5f;
    [SerializeField] private float playerRotationSpeed = 5f;

    [Header("Death Settings")]
    [SerializeField] private float killDistance = 2f;
    [SerializeField] private float lookBeforeKillDuration = 2f;

    [Header("References")]
    [SerializeField] private Transform player;

    private Quaternion originalPlayerRotation;
    private bool playerWasLooking = false;
    public bool PlayerDead = false;

    private float lookTimer = 0f;
    private bool isLookingAtMonster = false;
    private CharacterController characterController;
    private Rigidbody rb;
    private Vector3 moveDirection;

    private void Start()
    {
        // Try to get movement component
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.freezeRotation = true; // Prevent physics rotation
            }
        }

        // If player isn't assigned, try to find it by tag
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Player not found! Make sure the player has the 'Player' tag.");
            }
        }

        if (player != null)
        {
            originalPlayerRotation = player.rotation;
        }
    }

    private void Update()
    {
        if (player == null || PlayerDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Make player look at monster when close
        if (distanceToPlayer <= lookAtDistance)
        {
            MakePlayerLookAtMonster();
            playerWasLooking = true;
            isLookingAtMonster = true;

            // Increment look timer
            lookTimer += Time.deltaTime;

            // Check if player has been looking long enough and is within kill distance
            if (distanceToPlayer <= killDistance && lookTimer >= lookBeforeKillDuration)
            {
                KillPlayer();
            }
        }
        else
        {
            if (playerWasLooking)
            {
                playerWasLooking = false;
            }
            isLookingAtMonster = false;
            lookTimer = 0f; // Reset timer if monster moves away
        }

        // Check if player is within chase range and outside stop distance
        if (distanceToPlayer <= chaseRange && distanceToPlayer > stopDistance)
        {
            ChasePlayer();
        }
        else if (distanceToPlayer <= stopDistance)
        {
            // Stop moving but keep looking at player
            moveDirection = Vector3.zero;
            LookAtPlayer();
        }
        else
        {
            // Stop moving if out of range
            moveDirection = Vector3.zero;
        }

        // Apply movement
        MoveMonster();
    }

    private void ChasePlayer()
    {
        // Calculate direction to player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Check for obstacles
        Vector3 avoidanceDirection = GetAvoidanceDirection(directionToPlayer);

        // Set move direction
        moveDirection = avoidanceDirection;

        // Rotate towards movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetAvoidanceDirection(Vector3 desiredDirection)
    {
        // Check if path is clear
        if (!Physics.Raycast(transform.position, desiredDirection, obstacleDetectionDistance, obstacleLayer))
        {
            return desiredDirection;
        }

        // Try left and right avoidance
        Vector3 leftDir = Quaternion.Euler(0, -avoidanceAngle, 0) * desiredDirection;
        Vector3 rightDir = Quaternion.Euler(0, avoidanceAngle, 0) * desiredDirection;

        bool leftClear = !Physics.Raycast(transform.position, leftDir, obstacleDetectionDistance, obstacleLayer);
        bool rightClear = !Physics.Raycast(transform.position, rightDir, obstacleDetectionDistance, obstacleLayer);

        if (leftClear && rightClear)
        {
            // Choose the direction closer to the player
            float leftDot = Vector3.Dot(leftDir, desiredDirection);
            float rightDot = Vector3.Dot(rightDir, desiredDirection);
            return leftDot > rightDot ? leftDir : rightDir;
        }
        else if (leftClear)
        {
            return leftDir;
        }
        else if (rightClear)
        {
            return rightDir;
        }

        // If both blocked, try sharper angles
        leftDir = Quaternion.Euler(0, -90, 0) * desiredDirection;
        rightDir = Quaternion.Euler(0, 90, 0) * desiredDirection;

        leftClear = !Physics.Raycast(transform.position, leftDir, obstacleDetectionDistance, obstacleLayer);
        rightClear = !Physics.Raycast(transform.position, rightDir, obstacleDetectionDistance, obstacleLayer);

        if (leftClear) return leftDir;
        if (rightClear) return rightDir;

        // If all else fails, return desired direction
        return desiredDirection;
    }

    private void MoveMonster()
    {
        if (moveDirection == Vector3.zero) return;

        Vector3 movement = moveDirection * chaseSpeed * Time.deltaTime;

        if (characterController != null)
        {
            // Add gravity if using CharacterController
            movement.y = -9.81f * Time.deltaTime;
            characterController.Move(movement);
        }
        else if (rb != null)
        {
            // Use Rigidbody velocity
            rb.MovePosition(rb.position + movement);
        }
        else
        {
            // Direct transform movement (simplest, no physics)
            transform.position += movement;
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void MakePlayerLookAtMonster()
    {
        // Calculate direction from player to monster
        Vector3 direction = (transform.position - player.position).normalized;

        // Create target rotation (looking at monster, keeping Y axis locked)
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Smoothly rotate player towards monster
        player.rotation = Quaternion.Slerp(player.rotation, targetRotation, playerRotationSpeed * Time.deltaTime);
    }

    private void KillPlayer()
    {
        PlayerDead = true;
        Debug.Log("Player has been killed by the monster!");

        // Stop the monster
        moveDirection = Vector3.zero;

        NoteSystem noteSystem = FindObjectOfType<NoteSystem>();
        if (noteSystem != null)
        {
            noteSystem.Death();
        }
    }

    // Optional: Visualize ranges and raycasts in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookAtDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, killDistance);

        // Draw obstacle detection rays
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, directionToPlayer * obstacleDetectionDistance);

            Vector3 leftDir = Quaternion.Euler(0, -avoidanceAngle, 0) * directionToPlayer;
            Vector3 rightDir = Quaternion.Euler(0, avoidanceAngle, 0) * directionToPlayer;

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, leftDir * obstacleDetectionDistance);
            Gizmos.DrawRay(transform.position, rightDir * obstacleDetectionDistance);
        }
    }
}