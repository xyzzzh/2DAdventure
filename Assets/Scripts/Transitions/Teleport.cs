using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour, IInteractable
{
    public Vector3 positionToGO;
    
    public void TriggerAction()
    {
        Debug.Log("传送！");
        
    }
}
