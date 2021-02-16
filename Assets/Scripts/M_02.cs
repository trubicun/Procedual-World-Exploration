using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_02 : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f, maxAirAcceleration = 10f;
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;
    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;

    [SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f;

    Vector3 velocity, desiredVelocity;
    bool desiredJump, onGround;
    int jumpPhase;
    float minGroundDotProduct;
    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        OnValidate();
    }

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            maxSpeed *= 2;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            maxSpeed /= 2;
        }

        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        desiredJump |= Input.GetButtonDown("Jump");

        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        Vector2 camera = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z);
        desiredVelocity = new Vector3(camera.x, 0f, camera.y) * maxSpeed;
        desiredVelocity *= playerInput.y;

        Vector3 horizontal = Camera.main.transform.right * playerInput.x;
        horizontal.y = 0f;
        desiredVelocity += horizontal * maxSpeed;
        //desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
    }

    private void FixedUpdate()
    {
        UpdateState();
        float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        velocity += desiredVelocity * Time.deltaTime;

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        rigidbody.velocity = velocity;

        onGround = false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void UpdateState()
    {
        velocity = rigidbody.velocity;
        if (onGround)
        {
            jumpPhase = 0;
        }
    }

    void EvaluateCollision(Collision collision)
    {
        for(int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= minGroundDotProduct;
        }
    }

    void Jump()
    {
        if (onGround || jumpPhase < maxAirJumps)
        {
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            if (velocity.y > 0f) Mathf.Max(jumpSpeed -= velocity.y, 0);
            velocity.y += jumpSpeed;
        }
    }
}
