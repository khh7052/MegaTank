using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : Singleton<FollowCam>
{
    public Transform target;
    public Transform openingTr;
    public Transform battleTr;
    public Transform restTr;
    private Transform camTr;

    // 카메라의 위치 범위 설정
    [Range(2.0f, 20.0f)] public float distance = 10.0f;
    [Range(0.0f, 20.0f)] public float height = 2.0f;

    public float damping = 10.0f;
    public float targetOffset = 2.0f;
    private Vector3 velocity = Vector3.zero;

    public bool directMove;

    void Start()
    {
        camTr = GetComponent<Transform>();
    }

    public void CamTargetUpdate()
    {
        switch (GameManager.Instance.State)
        {
            case GameState.LOBBY:
                target = openingTr;
                break;
            case GameState.OPENING:
                target = openingTr;
                break;
            case GameState.STARTBATTLE:
                target = battleTr;
                break;
            case GameState.ENDBATTLE:
                target = battleTr;
                break;
            case GameState.REST:
                target = restTr;
                break;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (GameManager.Instance.State == GameState.ENDING || GameManager.Instance.State == GameState.LOBBY) return;

        Vector3 pos = target.position + (-target.forward * distance) + (Vector3.up * height);
        // camTr.position = targetTr.position + (-targetTr.forward * distance) + (Vector3.up * height); 방법 1 (position)
        // camTr.position = Vector3.Slerp(camTr.position, pos, Time.deltaTime * damping); // 방법 2 (Slerp/Lerp)
        // 방법 3 (SmoothDamp)

        if (directMove) camTr.position = pos;
        else camTr.position = Vector3.SmoothDamp(camTr.position, pos, ref velocity, damping);
        camTr.LookAt(target.position + (target.up * targetOffset));
    }
}
