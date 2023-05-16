using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InitSystem : MonoBehaviour
{
    public UnityEvent OnAwakeInit;
    public UnityEvent OnEnableInit;
    public UnityEvent OnDisableInit;
    
    private void Awake()
    {
        ComponentInit();
        AwakeInit();
    }

    private void OnEnable()
    {
        EnableInit();
    }

    private void OnDisable()
    {
        DisableInit();
    }

    public virtual void ComponentInit() { }

    public virtual void AwakeInit()
    {
        OnAwakeInit.Invoke();
    }
    public virtual void EnableInit() {
        OnEnableInit.Invoke();
    }
    public virtual void DisableInit()
    {
        OnDisableInit.Invoke();
    }
}
