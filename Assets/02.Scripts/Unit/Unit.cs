using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Team
{
    PLAYER,
    ENEMY
}

public class Unit : InitSystem
{
    public UnityEvent OnDeath = new();
    public UnityEvent OnAttack = new();
    public UnityEvent<int, Unit> OnDamage = new();
    public Team team;

    public string unitName = "";
    [Multiline]
    public string unitDescription = "";

    public int spawnMoney = 100; // 생성 비용
    public int deathMoney = 50; // 사망 시 흭득하는 돈

    [HideInInspector] public AIController aiController;
    protected Rigidbody rb;

    [Header("HP")]
    public int maxHP = 100;
    public int currentHP = 100;
    public int armor = 0;

    [Header("Move")]
    public float moveSpeed = 10f; // 이동속도
    public float rotateSpeed = 10f; // 회전속도

    [Header("Attack")]
    public int attackDamage = 10;
    public float attackRate = 1f;
    public float lookDistance = 100f; // 시야 사거리
    public float attackDistance = 50f; // 공격 사거리
    public float retreatDistance = 10f; // 후퇴 사거리
    public float chaseTime = 5f; // 추격시간
    private float nextAttackTime = 0;
    public GameObject destroyImpact;

    [Header("Audio")]
    public AudioSource audioSoruce;
    public AudioClip moveSound;
    public AudioClip attackSound;
    

    // HP
    public bool IsDying
    {
        get { return currentHP <= 0; }
    }

    // Attack
    public float RestAttackTime // 남은 공격시간
    {
        get { return nextAttackTime - Time.time; }
    }

    public bool AttackOn
    {
        get { return nextAttackTime < Time.time; }
    }

    // AI
    public Unit Target
    {
        get { return aiController.target; }
    }

    public override void ComponentInit()
    {
        aiController = GetComponent<AIController>();
        rb = GetComponent<Rigidbody>();
    }

    public override void EnableInit()
    {
        currentHP = maxHP;

        if (team == Team.ENEMY) {
            SpawnManager.Instance.RemainSpawnUnit++;
        }

        base.EnableInit();
    }

    public virtual void TakeDamage(int damage, Unit attacker)
    {
        currentHP -= Mathf.Max(damage - armor, 0);
        
        if (currentHP <= 0)
        {
            Death();
        }
        else
        {
            OnDamage.Invoke(damage, attacker);
        }
    }

    public void Attack(Unit target, Transform tr = null)
    {
        if (!AttackOn) return;
        OnAttack.Invoke();
        nextAttackTime = Time.time + attackRate;
    }

    public virtual void Death()
    {
        if (team == Team.ENEMY)
        {
            GameManager.Instance.saveMoney += deathMoney;
            SpawnManager.Instance.RemainSpawnUnit--;
        }
        

        OnDeath.Invoke();
        if(destroyImpact) PoolManager.Instance.Pop(destroyImpact, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }
}
