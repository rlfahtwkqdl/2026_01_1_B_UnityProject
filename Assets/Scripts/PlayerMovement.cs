using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("기본 이동 설정")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 7.0f;
    public float turnSpeed = 10f;

    [Header("점프 개선 설정")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultipliper = 2.0f;

    [Header("지면 감지 설정")]
    public float coyoteTime = 0.15f;
    public float coyoteTimeCounter;
    public bool realGround = true;

    [Header("글라이더 설정")]
    public GameObject gliderObject;
    public float gliderFallSpeed = 1.0f;
    public float gliderMoveSpeed = 7.0f;
    public float gliderMaxTime = 5.0f;
    public float gliderTimeLeft;
    public bool isGliding = false;

    public Rigidbody rd;

    public bool isGrounded = true;

    public int coinCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coyoteTimeCounter = 0;

        if (gliderObject !=null)
        {
            gliderObject.SetActive(false);
        }

        gliderTimeLeft = gliderMaxTime;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateGroundedState();

        float movehorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(movehorizontal, 0, moveVertical);
        
        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.G) && !isGrounded && gliderTimeLeft > 0) 
        {
            if (!isGliding)
            {
                EnableGlider();
            }

            gliderTimeLeft -= Time.deltaTime;

            if(gliderTimeLeft <= 0)
            {
                DisableGlider();
            }
        }
        else if(isGliding)
        {
            DisableGlider();
        }

        if (isGliding)
        {
            ApplyGliderMovement(movehorizontal, moveVertical);
        }
        else
        {

            rd.linearVelocity = new Vector3(movehorizontal * moveSpeed, rd.linearVelocity.y, moveVertical * moveSpeed);

            if (rd.linearVelocity.y < 0)
            {
                rd.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rd.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            {
                rd.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultipliper - 1) * Time.deltaTime;
            }


            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rd.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                realGround = false;
                coyoteTimeCounter = 0;
            }

            if (isGrounded)
            {
                {
                    if(isGliding)
                    {
                        DisableGlider();
                    }

                    gliderTimeLeft = gliderMaxTime;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            realGround = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            realGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            realGround = true;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coinCount++;
            Destroy(other.gameObject);
        }
    }

    void UpdateGroundedState()
    {
        if (realGround)
        {
            coyoteTimeCounter -= Time.deltaTime;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void EnableGlider()
    {
        isGliding = true;

        if (gliderObject != null)
        {
            gliderObject.SetActive(true);
        }

        rd.linearVelocity = new Vector3(rd.linearVelocity.x, -gliderFallSpeed , rd.linearVelocity.z);
    }

    void DisableGlider()
    {
        isGliding = false;

        if (gliderObject != null)
        {
            gliderObject.SetActive(false);
        }

        rd.linearVelocity = new Vector3(rd.linearVelocity.x, 0, rd.linearVelocity.z);
    }

    void ApplyGliderMovement(float horizontal , float vertical)
    {
        Vector3 gliderVelecity = new Vector3(
            horizontal * gliderMoveSpeed,
            -gliderFallSpeed,
            vertical * gliderMoveSpeed
            );

        rd.linearVelocity = gliderVelecity;
    }
}
