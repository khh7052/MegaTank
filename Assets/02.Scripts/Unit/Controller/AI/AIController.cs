using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    None = -1,
    IDLE, // ���
    MOVE, // �̵� (target�� �þ߹����ȿ� ���� ��)
    CHASE, // ���� (�ֺ��� ���� ����, target�� �����ٰ� ������� ��)
    ATTACK, // ���� (target�� ���ݹ����ȿ� ���� ��)
    RETREAT, // ���� (target�� ���� ���� �ȿ� ���� ��)
}

public class AIController : Controller
{
    public Unit mainTarget;
    public Unit target;
    public NavMeshAgent agent;

    protected readonly WaitForSeconds wait = new(0.1f);
    public AIState state = AIState.None;

    protected bool isTargetVisible = false;
    Coroutine chaseCoroutine;

    public Unit Target
    {
        get { return target; }
        set
        {
            if(target) target.OnDeath.RemoveListener(ResetTarget); // ���� Ÿ���� ResetTarget�� ����

            target = value;

            if (target == null)
            {
                state = AIState.IDLE;
            }
            else
            {
                target.OnDeath.AddListener(ResetTarget);
                state = AIState.MOVE;
            }
        }
    }

    public bool TargetInLookDistance
    {
        get { return Vector3.Distance(transform.position, target.transform.position) <= owner.lookDistance; }
    }

    public bool TargetInAttackDistance
    {
        get { return Vector3.Distance(transform.position, target.transform.position) <= owner.attackDistance; }
    }

    public bool TargetInRetreatDistance
    {
        get { return Vector3.Distance(transform.position, target.transform.position) <= owner.retreatDistance; }
    }

    public Vector3 TargetDir
    {
        get { return (target.transform.position - transform.position).normalized; }
    }

    private void OnDisable()
    {
        StopCoroutine(StateUpdate());
    }

    public override void EnableInit()
    {
        Init();
        base.EnableInit();
    }

    public void Init()
    {
        agent.speed = owner.moveSpeed;
        agent.angularSpeed = owner.rotateSpeed;
        agent.stoppingDistance = owner.attackDistance;
        owner.OnDamage.AddListener(DamageTarget);
        if(owner.team == Team.ENEMY) mainTarget = GameManager.Instance.baseUnit;
        ResetTarget();
        StartCoroutine(StateUpdate());
    }

    public virtual void Attack()
    {
        if (target == null) return;
        agent.stoppingDistance = owner.attackDistance;
        owner.Attack(target);
    }

    public virtual void Move()
    {
        if (target == null && mainTarget)
        {
            agent.SetDestination(mainTarget.transform.position);
        }
        else
        {
            agent.SetDestination(target.transform.position);
        }
    }

    public virtual void Chase()
    {
        if (target == null) return;
        agent.SetDestination(target.transform.position);

        ChaseStart();
    }

    void ChaseStart()
    {
        if (chaseCoroutine != null) return;
        chaseCoroutine = StartCoroutine(ChaseCoroutine(target));
    }

    void ChaseEnd()
    {
        if(chaseCoroutine == null) return;
        StopCoroutine(chaseCoroutine);
        chaseCoroutine = null;
    }

    // �þ� ������ ������� �� �����ð����� �߰� ����
    // �����ð� ������ Ÿ�� �ʱ�ȭ
    // ���� Ÿ���� �׾����� �̹� �ʱ�ȭ�����״� �н�
    // Ÿ�� OR �ٸ� ������ �þ߿� ������ �߰� �ڷ�ƾ ���
    public IEnumerator ChaseCoroutine(Unit target)
    {
        yield return new WaitForSeconds(owner.chaseTime);
        if (this.target == target) ResetTarget(); // �����ϰ� ���� Ÿ�� �߰����̸� �ʱ�ȭ
    }

    public virtual void Idle()
    {
        agent.SetDestination(transform.position);
    }

    public virtual void Retreat()
    {
        agent.stoppingDistance = 0;
        owner.Attack(target);
        agent.SetDestination(transform.position - (TargetDir * owner.moveSpeed));
    }

    public bool IsVisible(Vector3 start, Vector3 end)
    {
        return !Physics.Linecast(start, end, GameManager.Instance.obstacleLayerMask);
    }

    public void DamageTarget(int damage, Unit attacker)
    {
        if (state == AIState.ATTACK || state == AIState.RETREAT || state == AIState.MOVE) return;
        if (owner.team == attacker.team) return;
        Target = attacker;
        ChaseEnd();
        ChaseStart();
    }

    public void ResetTarget()
    {
        Target = mainTarget;
    }
    

    public virtual void TargetUpdate()
    {
        // 1. �þ߿� ���� �Ⱥ��̸� ��� ���̽��� Ÿ������ ��
        // 2. �þ߿� ���� ���̸� ���̴� ���� Ÿ��������

        // Ÿ���� ���ݹ������� ���̸� Ÿ�� �״��
        if (target)
        {
            if (TargetInAttackDistance && IsVisible(transform.position, target.transform.position)) return;
        }

        // �þ߹��� ���� ���ֵ��� �ҷ���
        Collider[] chaseColliders = Physics.OverlapSphere(transform.position, owner.lookDistance, GameManager.Instance.unitLayerMask);

        if (chaseColliders.Length > 0)
        {
            foreach (var coll in chaseColliders)
            {
                Unit unit = coll.GetComponent<Unit>();
                if (unit == null) continue;
                if (unit.team == owner.team) continue; // �������̸� �ѱ�

                isTargetVisible = IsVisible(transform.position, coll.transform.position);

                // �þ߿� �Ȱ����� Ÿ������ �����ϰ� ����
                if (isTargetVisible)
                {
                    Target = unit;
                    return;
                }
            }
        }

        if (target) isTargetVisible = IsVisible(transform.position, target.transform.position);
    }

    private IEnumerator StateUpdate()
    {
        while (true)
        {
            while (GameManager.Instance.State != GameState.STARTBATTLE) yield return wait;

            TargetUpdate();

            if(target == null)
            {
                if (mainTarget) state = AIState.MOVE;
                else state = AIState.IDLE;
            }
            else
            {
                if (isTargetVisible)
                {
                    // Ÿ���� ����������̸�
                    if (TargetInRetreatDistance) 
                        state = AIState.RETREAT; // �þ߿� ���̸� ����
                    // Ÿ���� ���ݹ������̸�
                    else if (TargetInAttackDistance) 
                        state = AIState.ATTACK; // �þ߿� ���̸� ����
                    // Ÿ���� �þ߹������̸�
                    else if (TargetInLookDistance)
                        state = AIState.MOVE; // �þ߿� ���̸� �̵�
                    // Ÿ���� �þ߹������̸�
                    else state = AIState.CHASE; // �Ⱥ��̸� �߰�
                }
                else
                {
                    state = AIState.CHASE; // �Ⱥ��̸� �߰�
                }
            }

            switch (state)
            {
                case AIState.IDLE:
                    Idle();
                    break;
                case AIState.MOVE:
                    Move();
                    break;
                case AIState.CHASE:
                    Chase();
                    break;
                case AIState.ATTACK:
                    Attack();
                    break;
                case AIState.RETREAT:
                    Retreat();
                    break;
                default:
                    Idle();
                    break;
            }

            if (state != AIState.CHASE) ChaseEnd();

            yield return wait;
        }
    }

    private void OnDrawGizmos()
    {
        if (owner == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, owner.attackDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, owner.lookDistance);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, owner.retreatDistance);
    }

}
