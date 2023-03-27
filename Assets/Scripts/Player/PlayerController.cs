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
    public PlayerAnimation playerAnimation;

    [Header("基本参数")]
    public float speed;
    public float jumpForce;
    private Vector2 originalOffset;
    private Vector2 originalSize;

    [Header("状态")]
    public bool isHurt;
    public float hurtForce;
    public bool isDead;
    public bool isAttack;
    public bool isCrouch;

    [Header("物理材质")] public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;
    
    //Awake()->OnEnable()->Start()
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        inputControl = new PlayerInputControl();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        
        originalOffset = coll.offset;
        originalSize = coll.size;

        // 注册Jump函数。当输入系统监测到Jump行为时，调用Jump函数
        inputControl.GamePlay.Jump.started += Jump;
        
        // 注册PlayerAttack
        inputControl.GamePlay.Attack.started += PlayerAttack;
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

    // Update is called once per frame
    void Update()
    {
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
        CheckState();
    }

    public void FixedUpdate()
    {
        if(!isHurt && !isAttack) Move();
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

    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        if (!physicsCheck.isGround) return;
        
        playerAnimation.PlayAttack();
        isAttack = true;
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

    public void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
    }
}
