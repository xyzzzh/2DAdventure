using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;
    private Rigidbody2D rb;

    [Header("Basic Params")]
    public float speed;
    public float jumpForce;
    

    //启动顺序：Awake()->OnEnable()->Start()
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputControl = new PlayerInputControl();

        // 注册函数：+=
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
        Move();
    }

    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        // 翻转人物
        int faceDir = (int)transform.localScale.x;

        if (inputDirection.x > 0) faceDir = 1;
        if (inputDirection.x < 0) faceDir = -1;

        transform.localScale = new Vector3(faceDir, 1, 1);
    }

    private void jump(InputAction.CallbackContext obj)
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }
}
