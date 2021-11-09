using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    public static PlayerMovement instance;

    private Rigidbody rb;

    public float defaultSpeed = 5f;
    public float speed = 5f;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHight = 1.5f;

    public float time;
    public float maxTime;
    public bool pressedJump;

    [SerializeField] private float dashForce = 50f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float timeBetweenDashes = 1f;
    private bool canDash = true;

    Vector3 velocity;
    public bool isGrounded;


    //for crouching
    float originalHeight;
    public float reducedHeight;


    //health stuff
    public int playerHealth;
    public TextMeshProUGUI healthText;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalHeight = controller.height;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x * speed + transform.forward * z * speed + transform.up * velocity.y;

        controller.Move(move * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime); 
        //Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHight * -2f * gravity);
            pressedJump = true;
        }

        if (pressedJump == false && isGrounded == false)
        {
            time += Time.deltaTime;
            if (time <= maxTime)
            {
                if (Input.GetButtonDown("Jump") && pressedJump == false)
                {
                    velocity.y = Mathf.Sqrt(jumpHight * -2f * gravity);
                    pressedJump = true;
                    time = 0;
                }
                else if (time >= maxTime)
                {
                    pressedJump = true;
                    time = 0;
                }
            }
        }

        healthText.text = "Health: " + playerHealth;

        //crouching
        if (Input.GetKeyDown(KeyCode.LeftControl)){
            crouch();
        } else if (Input.GetKeyUp(KeyCode.LeftControl)){
            GetUp();
        }

        //sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift)){
            speed = 5;
        } else if (Input.GetKeyUp(KeyCode.LeftShift)){
            speed = 3;
        }
    }

    void crouch(){
        controller.height = reducedHeight;
    } 

    void GetUp(){
        controller.height = originalHeight;
    }
}
