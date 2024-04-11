using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private TestPlayer player; // 추후 플레이어 로직 설정
    private Vector3 targetPos, refVel = Vector3.zero;
    private Camera cam;

    [SerializeField]
    private float zOffset; // 카메라 기본 고정값
    private Vector3 mousePoint;
    [SerializeField]
    private float camDist = 2.0f;
    [SerializeField]
    private float smoothTime = 0.2f;



    private static CameraController instance;
    public static CameraController Instance => instance;



    private void Awake()
    {
        Init();
        cam = GetComponent<Camera>();

    }

    void Start()
    {
        targetPos = player.transform.position;
        zOffset = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        mousePoint = CheckMousePointer();
        targetPos = UpdateTargetPos();
        UpdateCamPos();
    }

    private void Init()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    private Vector3 CheckMousePointer()
    {
        // 마우스 포인트 찾기
        Vector2 refVec = cam.ScreenToViewportPoint(Input.mousePosition) * 2;
        refVec *= 2;
        refVec -= Vector2.one;
        float max = 0.9f;

        // 최대 벡터 길이 1로 설정
        if (Mathf.Abs(refVec.x) > max || Mathf.Abs(refVec.y) > max)
            refVec = refVec.normalized;

        return refVec;
    }

    private Vector3 UpdateTargetPos()
    {
        Vector3 mousePos = mousePoint * camDist;
        Vector3 refVec = player.transform.position + mousePos;
        refVec.z = zOffset;
        return refVec;
    }

    private void UpdateCamPos()
    {
        Vector3 tempPos;
        tempPos = Vector3.SmoothDamp(transform.position, targetPos,
                                    ref refVel, smoothTime);
        transform.position = tempPos;
    }

}
