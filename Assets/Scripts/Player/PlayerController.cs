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
    public Character character;

    [Header("基本参数")]
    public float speed;
    public float jumpForce;
    public float wallJumpForce;
    private Vector2 originalOffset;
    private Vector2 originalSize;
    public float slideDistance;
    public float slideSpeed;
    public int slidePowerCost;

    [Header("状态")]
    public bool isHurt;
    public float hurtForce;
    public bool isDead;
    public bool isAttack;
    public bool isCrouch;
    public bool wallJump;
    public bool isSlide;

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
        character = GetComponent<Character>();
        
        originalOffset = coll.offset;
        originalSize = coll.size;

        // 注册Jump函数。当输入系统监测到Jump行为时，调用Jump函数
        inputControl.GamePlay.Jump.started += Jump;
        
        // 注册PlayerAttack
        inputControl.GamePlay.Attack.started += PlayerAttack;
        
        // 注册PlayerSlide
        inputControl.GamePlay.Slide.started += Slide;
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
        if(!isCrouch && !wallJump)
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
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            GetComponent<AudioDefination>()?.PlayAudioClip();
            // 打断slide协程
            isSlide = false;
            StopAllCoroutines();
        }
        else if (physicsCheck.onWall && !wallJump)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2.5f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
            
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        if (!physicsCheck.isGround) return;
        
        playerAnimation.PlayAttack();
        isAttack = true;
    }
    
    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;
            var targetPos = new Vector3(transform.position.x + transform.localScale.x * slideDistance,
                transform.position.y);
            // 更改player的层数，以实现slide过程中无伤的效果
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            StartCoroutine(TriggerSlide(targetPos));
            character.OnSlide(slidePowerCost);
        }
    }

    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            // 暂停一帧
            yield return null;
            if (!physicsCheck.isGround)
                break;
                // 暂停协程
                // yield break;
                

            //滑铲过程中撞墙
            if (physicsCheck.touchLeftWall && transform.localScale.x < 0f ||
                physicsCheck.touchRightWall && transform.localScale.x > 0f)
            {
                isSlide = false;
                // 暂停循环
                break;
            }

            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed,
                transform.position.y));
        } while (MathF.Abs(target.x - transform.position.x) > 0.2f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
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
        if (physicsCheck.onWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

        if (wallJump && rb.velocity.y < 0)
            wallJump = false;
        
        if (isDead || isSlide)
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");
    }
}
