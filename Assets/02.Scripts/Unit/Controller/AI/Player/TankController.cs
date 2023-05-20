using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class TankController : Controller
{
    #region ��� ����

    public Tank myTank;

    // �յ� �̵�
    private float moveVertical;

    // �¿� �̵�
    private float rotHorizon;

    // �ͷ� ȸ��
    private float rotTurret;

    // ���� ȸ��
    private float wheel;


    private Image aimImage;
    private RectTransform aimRectTransform;
    private Vector3 zoomInAimScale = new(0.5f, 0.5f, 0.5f);
    private Vector3 zoomOutAimScale = new(1f, 1f, 1f);
    public float zoomSpeed = 0.2f;
    public float zoomInCamScale = 40;
    public float zoomOutCamScale = 60;
    public float aimMoveSpeed = 800f;
    public float aimZoomMoveSpeed = 100f;
    public bool zoomOn = false;
    Vector3 aimMoveVelocity;
    float aimMoveVelocityF;

    #endregion


    void Update()
    {
        if (GameManager.Instance.State != GameState.STARTBATTLE) return; // ���� �ƴϸ� �ȿ�����
        if (EventSystem.current.IsPointerOverGameObject()) return; // UI ���� ���콺 ���� �� �ߴ�
        TankMoveInput();
        TurretMoveInput();
        MoveVolumeUpdate();
        FireInput();
    }


    private void LateUpdate()
    {
        if (GameManager.Instance.State != GameState.STARTBATTLE) return;
        AimUpdate();
        // AimImageUpdate();
    }

    public override void AwakeInit()
    {
        aimImage = UIManager.Instance.aimImage;
        aimRectTransform = aimImage.rectTransform;
        GameManager.Instance.OnStateChange.AddListener(() => VolumeReset());
        myTank.OnAttack.AddListener(AimImageUpdate);
        base.AwakeInit();
    }

    void VolumeReset()
    {
        moveVertical = 0;
        rotHorizon = 0;
        MoveVolumeUpdate();
    }

    void AimImageUpdate()
    {
        UIFade.Instance.FadeUI(aimImage, 0, 1, myTank.attackRate);
        // aimBackImage.fillAmount = myTank.FireTime;
    }

    void FireInput()
    {
        if (Input.GetMouseButton(0))
        {
            myTank.Attack(null);
        }
    }

    void AimUpdate()
    {
        // ��ž�Ա� ����
        if (Physics.Raycast(myTank.spPoint.position, myTank.spPoint.forward, out RaycastHit hit, 1000))
        {
            Vector3 targetPos = Camera.main.WorldToScreenPoint(hit.point);

            // ������ ������ ȭ�� Ȯ��
            if (hit.transform.CompareTag("Unit"))
            {
                Unit unit = hit.transform.GetComponent<Unit>();

                if (unit.team == owner.team) AimZoomOut(targetPos);
                else AimZoomIn(targetPos);
            }
            else
            {
                AimZoomOut(targetPos);
            }
        }
    }

    void AimZoomIn(Vector3 targetPos)
    {
        zoomOn = true;
        Vector3 startPos = aimRectTransform.position;
        aimRectTransform.position = Vector3.MoveTowards(startPos, targetPos, Time.deltaTime * aimZoomMoveSpeed);
        aimRectTransform.localScale = Vector3.SmoothDamp(aimRectTransform.localScale, zoomInAimScale, ref aimMoveVelocity, zoomSpeed);
        Camera.main.fieldOfView = Mathf.SmoothDamp(Camera.main.fieldOfView, zoomInCamScale, ref aimMoveVelocityF, zoomSpeed);
    }

    void AimZoomOut(Vector3 targetPos)
    {
        zoomOn = false;
        Vector3 startPos = aimRectTransform.position;
        aimRectTransform.position = Vector3.MoveTowards(startPos, targetPos, Time.deltaTime * aimMoveSpeed);
        aimRectTransform.localScale = Vector3.SmoothDamp(aimRectTransform.localScale, zoomOutAimScale, ref aimMoveVelocity, zoomSpeed);
        Camera.main.fieldOfView = Mathf.SmoothDamp(Camera.main.fieldOfView, zoomOutCamScale, ref aimMoveVelocityF, zoomSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(myTank.spPoint.position, myTank.turret.transform.forward * 500);
    }


    // �̵� ���� ������Ʈ
    void MoveVolumeUpdate()
    {
        float volume = Mathf.Max(Mathf.Abs(moveVertical), Mathf.Abs(rotHorizon));
        myTank.VolumeUpdate(volume);
    }

    void TankMoveInput()
    {
        // ���� ����
        moveVertical = Input.GetAxis("Vertical");
        rotHorizon = Input.GetAxis("Horizontal");
        myTank.TankMove(moveVertical, rotHorizon);
    }

    void TurretMoveInput()
    {
        // �ͷ� �¿� �̵�
        if (zoomOn)
        {
            rotTurret = Input.GetAxis("Mouse X") * UIManager.Instance.mouseZoomInSensitivityX;
            wheel = Input.GetAxis("Mouse ScrollWheel") * UIManager.Instance.mouseZoomInSensitivityY;
        }
        else
        {
            rotTurret = Input.GetAxis("Mouse X") * UIManager.Instance.mouseZoomOutSensitivityX;
            wheel = Input.GetAxis("Mouse ScrollWheel") * UIManager.Instance.mouseZoomOutSensitivityY;
        }
        
        myTank.TurretMove(rotTurret, wheel);
    }

}
