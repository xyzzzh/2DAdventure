using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance; // 单例模式
    private List<ISaveable> saveableList = new List<ISaveable>();

    [Header("监听")] public VoidEventSO saveDataEvent;

    private Data saveData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        saveData = new Data();
    }

    private void OnEnable()
    {
        saveDataEvent.OnEventRaised += Save;
    }
    
    private void OnDisable()
    {
        saveDataEvent.OnEventRaised -= Save;
    }

    // 观察者模式/广播模式 
    // 通过DataManager控制所有在列表中注册的内容，进行统一通知，或者把他们剔除
    public void RegisterSaveData(ISaveable saveable)
    {
        // 如果列表中不包含当前saveable，则添加
        if (!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }
    
    public void UnregisterSaveData(ISaveable saveable)
    {
        saveableList.Remove(saveable);
    }
    
    public void Save()
    {
        foreach (var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }
    }

    public void Load()
    {
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }

}
