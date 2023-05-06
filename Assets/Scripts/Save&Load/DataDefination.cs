 using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDefination : MonoBehaviour
{
    // 负责生成GUID
    public string ID;
    public PersistentType persistentType;

    private void OnValidate()
    {
        if (persistentType == PersistentType.ReadWrite)
        {
            if (ID == String.Empty)
                ID = System.Guid.NewGuid().ToString(); 
        }
        else
        {
            ID = String.Empty;
        }

    }
    
}
