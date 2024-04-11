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
    // ���� gameManager�� player�� �޾ƿð�
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

    Vector3 moveVec; // lean���¿��� ������ ����
    Vector3 tableVec; // table ��ġ

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

    // ���� ����
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
                Debug.Log("�̱��� ���");
                break;
        }
    }

    private void ChangeSprite()
    {
        spritesRenderer.sprite = basicSprites[agentAngleIndex];
    }

    // �÷��̾��� ���� ���
    private void AgentAngle()
    {
        agentAngleValue = AgentVector();
        agentAngleIndex = AngleCalculate(agentAngleValue); // up(�ĸ�), down(����), left(����), right(������)
    }

    private float AgentVector()
    {
        Vector3 value = GameManager.Instance.player.transform.position - transform.position;
        // ���� �ڱ� �ڽ��� �������� �÷��̾��� ��ġ�� ����Ͽ� ��� ������ �ٶ�����ϴ��� ������
        float angle;
        angle = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg; // ������ 180 ~ -180

        return angle;
    }

    private int AngleCalculate(float angleValue)
    {
        // �Ƹ� �ش� �̹����� �־������ �˵�

        int Index = -1;
        // 1��и�, �� �� �밢���� �켱����
        if (angleValue <= 135f && angleValue > 45f) Index = 0; // up
        // 2��и�, ���� �� �밢���� �켱����
        else if (angleValue <= 45f && angleValue > -45f) Index = 3; // right
        // 3��и�, ���� �Ʒ��밢���� �켱
        else if (angleValue <= -45f && angleValue > -135f) Index = 1; // down 
        // 4��и�, ���� �Ʒ��밢���� �켱
        else if (angleValue <= -135f || angleValue > 135f) Index = 2; // left

        return Index;
    }

    private void Idle()
    {
       /* if (!activeRoom)
        {
            isDetect = false;
        }*/ //�� �����̰� �������� �ʿ� ������?

        if (isDetect)
        {
            curStatus = EnemyStatus.Chase;
            return;
        }


        // ���ڸ� �ִϸ��̼� ����
        
    }

    // �÷��̾ ������ Ž��
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

        Debug.Log("����ũ");
        StartCoroutine(IAttack());
    }

    private IEnumerator IAttack()
    {
        isDetect = false;
        agent.isStopped = true;
        yield return new WaitForSeconds(1.0f);

        // ���ݷ��� �ۼ�

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
    // ������ �ٽ� �غ���
    
    public void UpLean() // ���̺� �̵� �� ���ݱ���
    {
        // ����
        if (curTableArrow != TableArrow.none && isLean)
            transform.position = Vector3.MoveTowards(transform.position, tableVec, 1.5f * Time.deltaTime);
        // ����
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
        Debug.Log("����.");

        Vector3 playerVec = GameManager.Instance.player.transform.position - transform.position;

        int index = -1; // �ִϸ��̼� ����
        moveVec = Vector3.zero; // ������ ����

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

        yield return new WaitForSeconds(0.5f); // ���ر��� �ɾ�� �ð�
        agent.isStopped = false;
        curStatus = EnemyStatus.Chase;
    }
    private void LeanAiming()
    {
        transform.localPosition = Vector3.MoveTowards(transform.position, moveVec, 3.0f * Time.deltaTime);
    }

    /*private void ChangeLean(int index, Vector3 vec)
    {
        Debug.Log("���� �ε���(��,�Ʒ�,��,�� ���� ���� 2����) : " + index);
        //transform.position += vec; // �ϴ� �����̵�, �ڷ�ƾ�ȿ� �ڷ�ƾ ����ġ��������� �۾��ϸ� ������?
    }*/

    private void UpdateLean()
    {
        if (!isMoveLean) return;
        transform.localPosition = Vector3.MoveTowards(transform.position, moveVec, 1.0f);
    }


    // ���߿� �׾����� ��� ����
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
