using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    None = -1,
    IDLE, // 대기
    MOVE, // 이동 (target이 시야범위안에 보일 때)
    CHASE, // 추적 (주변에 적이 없고, target이 보였다가 사라졌을 때)
    ATTACK, // 공격 (target이 공격범위안에 보일 때)
    RETREAT, // 후퇴 (target이 후퇴 범위 안에 있을 때)
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
            if(target) target.OnDeath.RemoveListener(ResetTarget); // 이전 타겟의 ResetTarget을 제거

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

    // 시야 밖으로 사라졌을 때 일정시간동안 추격 시작
    // 일정시간 지나면 타겟 초기화
    // 만약 타겟이 죽었으면 이미 초기화됬을테니 패스
    // 타겟 OR 다른 유닛이 시야에 들어오면 추격 코루틴 취소
    public IEnumerator ChaseCoroutine(Unit target)
    {
        yield return new WaitForSeconds(owner.chaseTime);
        if (this.target == target) ResetTarget(); // 이전하고 같은 타겟 추격중이면 초기화
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
        // 1. 시야에 적이 안보이면 상대 베이스를 타겟으로 함
        // 2. 시야에 적이 보이면 보이는 적을 타겟으로함

        // 타겟이 공격범위내에 보이면 타겟 그대로
        if (target)
        {
            if (TargetInAttackDistance && IsVisible(transform.position, target.transform.position)) return;
        }

        // 시야범위 내의 유닛들을 불러옴
        Collider[] chaseColliders = Physics.OverlapSphere(transform.position, owner.lookDistance, GameManager.Instance.unitLayerMask);

        if (chaseColliders.Length > 0)
        {
            foreach (var coll in chaseColliders)
            {
                Unit unit = coll.GetComponent<Unit>();
                if (unit == null) continue;
                if (unit.team == owner.team) continue; // 같은팀이면 넘김

                isTargetVisible = IsVisible(transform.position, coll.transform.position);

                // 시야에 안가리면 타겟으로 설정하고 종료
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
                    // 타겟이 후퇴범위안이면
                    if (TargetInRetreatDistance) 
                        state = AIState.RETREAT; // 시야에 보이면 후퇴
                    // 타겟이 공격범위안이면
                    else if (TargetInAttackDistance) 
                        state = AIState.ATTACK; // 시야에 보이면 공격
                    // 타겟이 시야범위안이면
                    else if (TargetInLookDistance)
                        state = AIState.MOVE; // 시야에 보이면 이동
                    // 타겟이 시야범위밖이면
                    else state = AIState.CHASE; // 안보이면 추격
                }
                else
                {
                    state = AIState.CHASE; // 안보이면 추격
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
