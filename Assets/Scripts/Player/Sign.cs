using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    private Animator anim;
    private PlayerInputControl playerInput;
    public GameObject signSprite;
    public Transform playerTrans;
    private bool canPress;

    private void Awake()
    {
        anim = signSprite.GetComponent<Animator>();
        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += actionChange;
    }

    private void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
        if(canPress)    anim.Play("Keyboard");
    }

    private void actionChange(object obj, InputActionChange actionChange)
    {
        // var d = ((InputAction)obj).activeControl.device;
        // switch (d.device)
        // {
        //     case Keyboard:
        //         anim.Play("Keyboard");
        //         break;
        // }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            canPress = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
    }
}
