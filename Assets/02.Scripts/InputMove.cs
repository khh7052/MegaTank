using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMove : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float zoomSpeed = 2f;
    public float minZoom = 20f;
    public float maxZoom = 60f;
    private Vector3 originalPos;
    private float originalZoom;
    private float originalDistance;
    private float originalHeight;

    public float limitRange = 200;
    void Awake()
    {
        originalPos = transform.position;
        originalZoom = Camera.main.fieldOfView;
        originalDistance = FollowCam.Instance.distance;
        originalHeight = FollowCam.Instance.height;
        GameManager.Instance.OnRest.AddListener(StartMove);
        GameManager.Instance.OnEnding.AddListener(Init);
        GameManager.Instance.OnOpening.AddListener(Init);
    }

    void LateUpdate()
    {
        if (GameManager.Instance.State != GameState.REST && GameManager.Instance.State != GameState.SETTING) return;

        Move();
        Zoom();
    }

    void StartMove()
    {
        FollowCam.Instance.directMove = true;
        FollowCam.Instance.distance = 10;
        FollowCam.Instance.height = 20;
    }

    void Init()
    {
        transform.position = originalPos;
        Camera.main.fieldOfView = originalZoom;
        FollowCam.Instance.directMove = false;
        FollowCam.Instance.distance = originalDistance;
        FollowCam.Instance.height = originalHeight;
    }

    void Move()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        Vector3 dir = new Vector3(h, 0, v).normalized;

        transform.Translate(moveSpeed * Time.deltaTime * dir);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -limitRange, limitRange);
        pos.z = Mathf.Clamp(pos.z, -limitRange, limitRange);

        transform.position = pos;
    }

    void Zoom()
    {
        float wheel = -Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + wheel, minZoom, maxZoom);
    }
}
