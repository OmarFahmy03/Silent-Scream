using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Car : MonoBehaviour
{
    private const string H = "Horizontal";
    private const string V = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentBrakeForce;
    private float currentSteerAngle;
    private bool isBraking;

    [Header("Car Settings")]
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] public float maxSpeed = 200f; // km/h
    [SerializeField] private float brakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 30f;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider FLwheelCol;
    [SerializeField] private WheelCollider FRwheelCol;
    [SerializeField] private WheelCollider BLwheelCol;
    [SerializeField] private WheelCollider BRwheelCol;

    [Header("Dashboard")]
    [SerializeField] private Transform speedNeedle;
    [SerializeField] private Transform rpmNeedle;
    [SerializeField] private float maxNeedleRotation = -150f;

    [Header("Visual Steering")]
    [SerializeField] private Transform steeringWheel;
    private float currentVisualSteerAngle = 0f;

    [Header("Crash Detection")]
    [SerializeField] private float raycastHeight = 1.5f; // start height above car
    [SerializeField] private float rayLength = 2f;       // distance to check upward
    [SerializeField] private LayerMask crashLayer;       // layer to detect crash with

    private Rigidbody rb;

    // For RPM simulation
    private float currentRPM = 1000f;
    private float targetRPM = 1000f;
    private const float minRPM = 800f;
    private const float maxRPM = 7000f;

    public CanvasGroup crashCanvasGroup; // Assign in inspector

    private bool hasCrashed = false;

    private IEnumerator AnimateCrashAndLoadScene()
    {
        if (crashCanvasGroup != null)
        {
            float duration = 1.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                crashCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            crashCanvasGroup.alpha = 1f;
        }
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Hospital"); // Change "CrashScene" to your actual scene name
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Start with a small initial speed to avoid stalling
        float startSpeedKmh = 10f; // adjust this to your desired starting speed
        Vector3 startVelocity = transform.forward * (startSpeedKmh / 3.6f); // convert to m/s
        rb.linearVelocity = startVelocity;
    }

    void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        LimitSpeed();
        UpdateDashboard();
        UpdateSteeringWheel();
        DetectCrashAbove();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(H);
        verticalInput = Input.GetAxis(V);
        isBraking = Input.GetButton("Fire1");
    }

    private void HandleMotor()
    {
        float torque = verticalInput * motorForce;

        // Prevent applying more torque if we're already at max speed
        float speed = rb.linearVelocity.magnitude * 3.6f;
        if (speed >= maxSpeed && verticalInput > 0f)
            torque = 0f;

        FLwheelCol.motorTorque = torque;
        FRwheelCol.motorTorque = torque;

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        FLwheelCol.brakeTorque = currentBrakeForce;
        FRwheelCol.brakeTorque = currentBrakeForce;
        BLwheelCol.brakeTorque = currentBrakeForce;
        BRwheelCol.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        FLwheelCol.steerAngle = currentSteerAngle;
        FRwheelCol.steerAngle = currentSteerAngle;
    }

    private void LimitSpeed()
    {
        float maxSpeedMS = maxSpeed / 3.6f;
        if (rb.linearVelocity.magnitude > maxSpeedMS)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeedMS;
    }

    private void UpdateDashboard()
    {
        if (rb == null) return;

        float speed = rb.linearVelocity.magnitude * 3.6f;
        float speedT = Mathf.Clamp01(speed / 150);

        if (speedNeedle != null)
        {
            float speedAngle = Mathf.Lerp(0f, maxNeedleRotation, speedT);
            speedNeedle.localRotation = Quaternion.Euler(0f, 0f, speedAngle);
        }

        targetRPM = Mathf.Lerp(minRPM, maxRPM, Mathf.Abs(verticalInput) * 0.7f + speedT * 0.3f);
        currentRPM = Mathf.Lerp(currentRPM, targetRPM, Time.deltaTime * 5f);

        float rpmNormalized = Mathf.InverseLerp(minRPM, maxRPM, currentRPM);
        if (rpmNeedle != null)
        {
            float rpmAngle = Mathf.Lerp(0f, maxNeedleRotation, rpmNormalized);
            rpmNeedle.localRotation = Quaternion.Euler(0f, 0f, rpmAngle);
        }
    }

    private void UpdateSteeringWheel()
    {
        if (steeringWheel == null) return;

        currentVisualSteerAngle = Mathf.Lerp(currentVisualSteerAngle, currentSteerAngle, Time.deltaTime * 10f);
        steeringWheel.localRotation = Quaternion.Euler(-82.75f, 0f, 0f);
        steeringWheel.Rotate(Vector3.up * currentVisualSteerAngle, Space.Self);
    }

    // 🔹 Raycast Detection for Crashes Above the Car
    private void DetectCrashAbove()
    {
        // The ray now uses the car’s local up direction (transform.up)
        Vector3 rayOrigin = transform.position + transform.up * raycastHeight;
        Ray ray = new Ray(rayOrigin, transform.up);

        if (Physics.Raycast(ray, out RaycastHit hit, rayLength, crashLayer))
        {
            Debug.DrawRay(rayOrigin, transform.up * rayLength, Color.red);
            Debug.Log("🚗 Crash detected above with: " + hit.collider.name);
            if (!hasCrashed)
            {
                hasCrashed = true;
                StartCoroutine(AnimateCrashAndLoadScene());
            }
        }
        else
        {
            Debug.DrawRay(rayOrigin, transform.up * rayLength, Color.green);
        }
    }

}
