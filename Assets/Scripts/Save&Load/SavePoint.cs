using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    [Header("事件广播")] public VoidEventSO saveGameEvent;
    [Header("参数")]
    public SpriteRenderer spriteRenderer;

    public GameObject lightObj;
    public Sprite darkSprite;
    public Sprite lightSprite;

    public bool isDone;

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? lightSprite : darkSprite;
        lightObj.SetActive(isDone);
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            isDone = true;
            spriteRenderer.sprite = lightSprite;
            lightObj.SetActive(isDone);
            // TODO:保存数据
            saveGameEvent.RaisedEvent();
            
            this.gameObject.tag = "Untagged";
        }
    }
}
