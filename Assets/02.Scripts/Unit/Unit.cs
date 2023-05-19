using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public enum Team
{
    PLAYER,
    ENEMY
}

public enum UnitType
{
    UNIT,
    BUILDING
}

public enum ConditionType
{
    NONE, // 조건없음
    LIMIT, // 자신의 개수 제한
    NEED, // 필요한 유닛
}

[System.Serializable]
public class Condition
{
    public ConditionType type;
    public int count = 1;
    public Unit needUnit;

    public bool ConditionCheck(Unit myUnit)
    {
        switch (type)
        {
            case ConditionType.NONE:
                return true;
            case ConditionType.LIMIT:
                if (PoolManager.Instance.poolList.ContainsKey(myUnit.gameObject) == false) return true; // 만든적 없으면 true
                return PoolManager.Instance.poolList[myUnit.gameObject].Count < count; // 만든 개수가 제한보다 적으면 true
            case ConditionType.NEED:
                foreach (GameObject g in PoolManager.Instance.poolList[needUnit.gameObject])
                {
                    if (g.activeInHierarchy) return true; // 현재 필요한 유닛이 존재하면 true
                }

                return false;
        }

        return false;
    }
}

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : InitSystem
{
    protected NavMeshAgent agent;
    public UnitType unitType;
    public UnityEvent OnDeath = new();
    public UnityEvent OnMove = new();
    public UnityEvent OnAttack = new();
    public UnityEvent<int, Unit> OnDamage = new();
    public Condition makeCondition;
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
        agent = GetComponent<NavMeshAgent>();
    }

    public override void EnableInit()
    {
        currentHP = maxHP;

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

    public virtual void Move(Vector3 pos)
    {
        agent.SetDestination(pos);
        OnMove.Invoke();
    }

    public void Attack(Unit target, Transform tr = null)
    {
        if (!AttackOn) return;
        OnAttack.Invoke();
        nextAttackTime = Time.time + attackRate;
    }


    public virtual void Death()
    {
        if(team == Team.PLAYER)
        {
            if(unitType == UnitType.UNIT)
            {
                GameManager.Instance.playerUnitDeathCount++;
                if (this == GameManager.Instance.playerUnit || this == GameManager.Instance.baseUnit) GameManager.Instance.State = GameState.ENDING; // 플레이어가 죽으면 엔딩
            }
            else if (unitType == UnitType.BUILDING) GameManager.Instance.playerBuildingDeathCount++;
        }
        if (team == Team.ENEMY)
        {
            GameManager.Instance.saveMoney += deathMoney;
            GameManager.Instance.enemyUnitDeathCount++;
            GameManager.Instance.EnemyUnitRemainCount--;
            UIManager.Instance.EnemyCountUpdate();
        }

        OnDeath.Invoke();
        if(destroyImpact) PoolManager.Instance.Pop(destroyImpact, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }

}
