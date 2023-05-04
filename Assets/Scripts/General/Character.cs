using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    public VoidEventSO newGameEvent;
    [Header("基本属性")] 
    public float maxHealth;
    public float currHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header(("受伤无敌"))] 
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;
    private void NewGame()
    {
        currHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void Start()
    {
        currHealth = maxHealth;
    }
    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }

        if (currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            currHealth = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();
        }
        
    }

    public void TakeDamage(Attack attacker)
    {
        if (invulnerable) return;
        if (currHealth - attacker.damage >= 0)
        {
            currHealth -= attacker.damage;
            TriggerInvulnerabe();
            // 触发伤害
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currHealth = 0;
            // 触发死亡
            OnDie?.Invoke();
        }
        OnHealthChange?.Invoke(this);
    }

    private void TriggerInvulnerabe()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}
