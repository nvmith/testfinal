using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStatus
{ 
    Idle,
    Chase,
    Attack,
    Stun,
    Lean,
    Die
}

public class Agent : MonoBehaviour
{
    // 추후 gameManager의 player를 받아올것
    [SerializeField]
    Transform target;

    NavMeshAgent agent;
    Rigidbody2D rigid;
    CircleCollider2D cirCollider2D;
    SpriteRenderer spritesRenderer;

    [SerializeField]
    private Sprite[] basicSprites;


    /*private bool activeRoom = false;
    public bool ActiveRoom => activeRoom;*/

    private EnemyStatus curStatus;
    public EnemyStatus CurStatus => curStatus;

    private bool isDie = false;

    // AI
    private bool isDetect = false;
    private bool isMoveLean = false;
    private bool isLean = false;

    private bool tableMove;
    private TableArrow curTableArrow;

    private float agentAngleValue;
    private int agentAngleIndex;

    Vector3 moveVec; // lean상태에서 움직일 방향
    Vector3 tableVec; // table 위치

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        rigid = GetComponent<Rigidbody2D>();
        curStatus = EnemyStatus.Idle;
        cirCollider2D = GetComponent<CircleCollider2D>();
        spritesRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDetect) UpdateState(curStatus);

        if (Input.GetKeyDown(KeyCode.Space)) Die();
    }

    // 떨림 방지
    private void FixedUpdate()
    {
        rigid.velocity = Vector2.zero;
    }

    private void UpdateState(EnemyStatus enemy)
    {
        if (isDie) return;

        AgentAngle();
        if (isDetect && enemy != EnemyStatus.Lean) ChangeSprite();

        switch (enemy)
        {
            case EnemyStatus.Idle:
                Idle();
                break;
            case EnemyStatus.Chase:
                Chase();
                break;
            case EnemyStatus.Attack:
                Attack();
                break;
            case EnemyStatus.Lean:
                UpLean();
                //UpdateLean();
                break;
            case EnemyStatus.Die:
                Die();
                break;
            default:
                Debug.Log("미구현 기능");
                break;
        }
    }

    private void ChangeSprite()
    {
        spritesRenderer.sprite = basicSprites[agentAngleIndex];
    }

    // 플레이어의 방향 계산
    private void AgentAngle()
    {
        agentAngleValue = AgentVector();
        agentAngleIndex = AngleCalculate(agentAngleValue); // up(후면), down(정면), left(왼쪽), right(오른쪽)
    }

    private float AgentVector()
    {
        Vector3 value = GameManager.Instance.player.transform.position - transform.position;
        // 현재 자기 자신을 기점으로 플레이어의 위치를 계산하여 어느 방향을 바라봐야하는지 보여줌
        float angle;
        angle = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg; // 범위가 180 ~ -180

        return angle;
    }

    private int AngleCalculate(float angleValue)
    {
        // 아마 해당 이미지를 넣어봐야지 알듯

        int Index = -1;
        // 1사분면, 왼 윗 대각까진 우선순위
        if (angleValue <= 135f && angleValue > 45f) Index = 0; // up
        // 2사분면, 오른 윗 대각까진 우선순위
        else if (angleValue <= 45f && angleValue > -45f) Index = 3; // right
        // 3사분면, 오른 아랫대각까진 우선
        else if (angleValue <= -45f && angleValue > -135f) Index = 1; // down 
        // 4사분면, 왼쪽 아랫대각까진 우선
        else if (angleValue <= -135f || angleValue > 135f) Index = 2; // left

        return Index;
    }

    private void Idle()
    {
       /* if (!activeRoom)
        {
            isDetect = false;
        }*/ //다 안죽이곤 못나가서 필요 없을듯?

        if (isDetect)
        {
            curStatus = EnemyStatus.Chase;
            return;
        }


        // 재자리 애니메이션 적용
        
    }

    // 플레이어가 들어온지 탐지
    public void PlayerRoom()
    {
        isDetect = true;
    }

    private void Chase()
    {
        //if (!isDetect) return;

        if (!isDetect)
        {
            curStatus = EnemyStatus.Idle;
            return;
        }
        
        agent.SetDestination(target.position);
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= 2.0f) curStatus = EnemyStatus.Attack;
    }

    private void Attack()
    {
        if(agent.isStopped) return;

        Debug.Log("공격크");
        StartCoroutine(IAttack());
    }

    private IEnumerator IAttack()
    {
        isDetect = false;
        agent.isStopped = true;
        yield return new WaitForSeconds(1.0f);

        // 공격로직 작성

        agent.isStopped = false;
        isDetect = true;
        curStatus = EnemyStatus.Idle;
    }

    private void Stun()
    {

    }

    private void Damage()
    {
        
    }
    // 집가서 다시 해보기
    
    public void UpLean() // 테이블 이동 및 저격까지
    {
        // 기대기
        if (curTableArrow != TableArrow.none && isLean)
            transform.position = Vector3.MoveTowards(transform.position, tableVec, 1.5f * Time.deltaTime);
        // 조준
        else
        {
            LeanAiming();
        }
        
    }

    public void TableValue(Vector3 vec, TableArrow arrow)
    {
        tableVec = vec;
        curTableArrow = arrow;
        curStatus = EnemyStatus.Lean;
        isLean = true;

        agent.isStopped = true;
        StartCoroutine(LeanCount());
    }

    private IEnumerator LeanCount()
    {
        Debug.Log("기대다.");

        Vector3 playerVec = GameManager.Instance.player.transform.position - transform.position;

        int index = -1; // 애니메이션 방향
        moveVec = Vector3.zero; // 움직일 방향

        switch (curTableArrow)
        {
            case TableArrow.up:
                if (playerVec.x <= 0)
                {
                    index = 0;
                    moveVec = new Vector3(-5, 0);
                }
                else
                {
                    index = 1;
                    moveVec = new Vector3(5, 0);
                }
                break;
            case TableArrow.down:
                if (playerVec.x <= 0)
                {
                    index = 2;
                    moveVec = new Vector3(-5, 0);
                }
                else
                {
                    index = 3;
                    moveVec = new Vector3(5, 0);
                }
                break;
            case TableArrow.left:
                if (playerVec.y <= 0)
                {
                    index = 4;
                    moveVec = new Vector3(0, -5);
                }
                else
                {
                    index = 5;
                    moveVec = new Vector3(0, 5);
                }
                break;
            case TableArrow.right:
                if (playerVec.y <= 0)
                {
                    index = 6;
                    moveVec = new Vector3(0, -5);
                }
                else
                {
                    index = 7;
                    moveVec = new Vector3(0, 5);
                }
                break;
        }

        yield return new WaitForSeconds(1.0f);

        isLean = false;

        yield return new WaitForSeconds(0.5f); // 조준까지 걸어가는 시간
        agent.isStopped = false;
        curStatus = EnemyStatus.Chase;
    }
    private void LeanAiming()
    {
        transform.localPosition = Vector3.MoveTowards(transform.position, moveVec, 3.0f * Time.deltaTime);
    }

    /*private void ChangeLean(int index, Vector3 vec)
    {
        Debug.Log("현재 인덱스(위,아래,왼,오 기준 각각 2개씩) : " + index);
        //transform.position += vec; // 일단 순간이동, 코루틴안에 코루틴 끊어치기느낌으로 작업하면 될지도?
    }*/

    private void UpdateLean()
    {
        if (!isMoveLean) return;
        transform.localPosition = Vector3.MoveTowards(transform.position, moveVec, 1.0f);
    }


    // 나중에 죽었을때 기능 구현
    private void Die()
    {
        //if (isDetect) isDetect = false;
        //agent.enabled = false;

        isDie = true;
        isDetect = false;
        cirCollider2D.enabled = false;
        agent.enabled = false;
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isDetect = true;
        }
    }*/
}
