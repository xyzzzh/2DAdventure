using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    public PhysicsCheck physicsCheck;
    public Vector2 inputDirection;
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;

    [Header("Basic Params")]
    public float speed;
    public float jumpForce;
    public bool isCrouch;
    private Vector2 originalOffset;
    private Vector2 originalSize;

    public bool isHurt;
    public float hurtForce;
    public bool isDead;


    //Awake()->OnEnable()->Start()
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        inputControl = new PlayerInputControl();
        coll = GetComponent<CapsuleCollider2D>();
        originalOffset = coll.offset;
        originalSize = coll.size;

        // add jump function 
        inputControl.GamePlay.Jump.started += jump;
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    // 测试
    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     Debug.Log(other.name);
    // }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
    }

    public void FixedUpdate()
    {
        if(!isHurt) Move();
    }

    public void Move()
    {
        if(!isCrouch)
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        int faceDir = (int)transform.localScale.x;

        if (inputDirection.x > 0) faceDir = 1;
        if (inputDirection.x < 0) faceDir = -1;

        transform.localScale = new Vector3(faceDir, 1, 1);

        isCrouch = (inputDirection.y < -0.5f) && physicsCheck.isGround;
        if (isCrouch)
        {
            coll.offset = new Vector2(-0.05f, 0.85f);
            coll.size = new Vector2(0.7f, 1.7f);
        }
        else
        {
            coll.offset = originalOffset;
            coll.size = originalSize;
        }
    }

    private void jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 Dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
        
        rb.AddForce(Dir*hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.GamePlay.Disable();
    }
}
