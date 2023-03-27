using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;
    protected PhysicsCheck physicsCheck;
    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    public float currSpeed;
    public Vector3 faceDir;
    public float hurtForce;

    [Header("计时器")] public float waitTime;
    public float waitTimeCounter;
    public bool wait;

    public Transform attacker;

    [Header("受伤状态")] public bool isHurt;
    public bool isDead;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        
        currSpeed = normalSpeed;
        waitTimeCounter = waitTime;
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);
        if ((physicsCheck.touchLeftWall && faceDir.x <= 0) || (physicsCheck.touchRightWall&& faceDir.x >= 0))
        {
            wait = true;
            anim.SetBool("walk", false);
        }
        TimeCounter();
    }

    private void FixedUpdate()
    {
        if (!isHurt)
        {
            Move();
        }
    }

    public virtual void Move()
    {
        rb.velocity = new Vector2(faceDir.x * currSpeed * Time.deltaTime, rb.velocity.y);
    }

    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, 1, 1);
            }
        }
    }

    public void OnTakeDamage(Transform attackerTrans)
    {
        attacker = attackerTrans;
        // 转身
        if (attacker.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (attacker.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        
        //受伤被击退
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0);
        //调用协程
        StartCoroutine(OnHurt(dir));
    }

    // 协程
    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir*hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }

    public void OnDie()
    {
        this.gameObject.layer = 2;
        anim.SetBool("dead", true);
        isDead = true;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
}
