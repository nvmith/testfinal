using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum TableArrow
{
    up, down, left, right, none
}

public class Table : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;
    public Sprite[] Sprites => sprites;

    [SerializeField]
    private GameObject[] lineObj;

    private SpriteRenderer tableSprite;

    float curDelay = 0;
    float delay = 0.5f;
    bool playerCheck = false;
    bool isMove = false;
    Vector3 moveVec;
    Rigidbody2D rigid;
    bool enemyCheck = false;
    
    
    TableArrow curArrow;
    TableArrow agentArrow; // agent�� ��� �� �ִ� ����
    int lineIndex = -1;

    private Vector3[] distance = new Vector3[4];

    // Start is called before the first frame update
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        tableSprite = GetComponent<SpriteRenderer>();

        curArrow = TableArrow.none;

    }

    // Update is called once per frame
    void Update()
    {
        //MoveTable();
        CurPos();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (curArrow == TableArrow.none && collision.gameObject.tag.Equals("Player"))
        {
            playerCheck = true;

            Debug.Log("����");
        }

        if(!enemyCheck && curArrow != TableArrow.none && collision.gameObject.tag.Equals("Agent"))
        {
            enemyCheck = true;
            GameObject agentObj = collision.gameObject;
            LeanAgent(agentObj.transform.position, agentObj);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (curArrow == TableArrow.none && collision.gameObject.tag.Equals("Player"))
        {
            playerCheck = false;
            ActiveLine(false);
            Debug.Log("����");
        }

        if (enemyCheck && curArrow != TableArrow.none && collision.gameObject.tag.Equals("Agent"))
        {
            enemyCheck = false;
        }
    }

    // ��ġ ���
    private void CurPos()
    {
        if (!playerCheck) return; 
        
        moveVec = GameManager.Instance.player.transform.localPosition;
        moveVec -= transform.position;

        float tableAngle = VectorValue(moveVec);

        lineIndex = AngleCalculate(tableAngle);
        if(lineIndex != -1) ActiveLine(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            switch((TableArrow)lineIndex)
            {
                case TableArrow.up:
                    agentArrow = TableArrow.down;
                    break;

                case TableArrow.down:
                    agentArrow = TableArrow.up;
                    break;

                case TableArrow.left:
                    agentArrow = TableArrow.right;
                    break;

                case TableArrow.right:
                    agentArrow = TableArrow.left;
                    break;
            }
            
            
            curArrow = (TableArrow)lineIndex;

            // �ִϸ��̼� ��� �ϴ� ��������Ʈ ����
            tableSprite.sprite = Sprites[(int)curArrow];
            //playerCheck = false; //�̰� �׽�Ʈ �ݵ�� ������ Ȱ��ȭ

            // ���̺� ũ�� ����

        }
    }

    // ���� ���
    private float VectorValue(Vector3 value)
    {
        float angle;
        angle = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg; // ������ 180 ~ -180

        return angle;
    }

    // ���� ����
    private int AngleCalculate(float angleValue)
    {
        int Index = -1;
        // 1��и�, �� �� �밢���� �켱����
        if (angleValue <= 135f && angleValue > 45f)
        {
            Index = 0; // up
            distance[Index] = transform.position + new Vector3(0, 1, 0);
        }
        // 2��и�, ���� �� �밢���� �켱����
        else if (angleValue <= 45f && angleValue > -45f)
        {
            Index = 3; // right
            distance[Index] = transform.position + new Vector3(0.5f, 0, 0);
        }
        // 3��и�, ���� �Ʒ��밢���� �켱
        else if (angleValue <= -45f && angleValue > -135f)
        {
            Index = 1; // down 
            distance[Index] = transform.position + new Vector3(0, -1, 0);
        }
        // 4��и�, ���� �Ʒ��밢���� �켱
        else if (angleValue <= -135f || angleValue > 135f)
        {
            Index = 2; // left
            distance[Index] = transform.position + new Vector3(-0.5f, 0, 0);
        }

        return Index;
    }

    // ���� ���̺� ���� ǥ�� �뵵
    private void ActiveLine(bool activeTrue)
    {
        foreach (GameObject g in lineObj)
            g.SetActive(false);

        if(activeTrue) lineObj[lineIndex].SetActive(true);
    }

    // AI ���̺� ���� ���� ���
    private void LeanAgent(Vector3 vec, GameObject agentObj)
    {
        moveVec = GameManager.Instance.player.transform.localPosition;

        int playerLine = -1;
        int agentLine = -1;

        float playerVec = VectorValue(moveVec - transform.position);
        float agentVec = VectorValue(vec - transform.position);

        playerLine = AngleCalculate(playerVec);
        agentLine = AngleCalculate(agentVec);

        Debug.Log("��� üũ");
        Debug.Log("playerLine : " + playerLine);
        Debug.Log("agentLine : " + agentLine);

        Debug.Log("agentArrow : " + agentArrow);
        Debug.Log("curagentArrow : " + (TableArrow)agentLine);

        if (playerLine == agentLine || agentArrow != (TableArrow)agentLine) return;
        Debug.Log("���� üũ");

        //agentObj.GetComponent<Agent>().Lean(agentArrow); // ��� �ּ�

        agentObj.GetComponent<Agent>().TableValue(distance[(int)agentArrow] ,agentArrow);
    }



    /*
    private IEnumerator CheckKey()
    {
        while(!playerCheck)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.E))
            {
                playerCheck = true;
                ChangeTable();
            }
        }
    }

    
    private void ChangeTable()
    {
       


        
        float minDis = 999.0f;
        float curDis = 0f;

        for(int i=0; i < tablePos.Length; i++)
        {
            curDis = Vector3.Distance(tablePos[i], moveVec);
            if (curDis < minDis)
            {
                curArrow = (TableArrow)i;
                minDis = curDis;
            }
        }

        // �ִϸ��̼� ��� �ϴ� ��������Ʈ ����
        //tableSprite.sprite = Sprites[(int)curArrow];


        // ���� �ݶ��̴��� ����
    }*/

    /*
    private void MoveTable()
    {
        if(isMove)
        {
            curDelay += Time.deltaTime;
        }

        if (delay <= curDelay) VectorMove(GameManager.Instance.player.InputVec);
    }

    private void VectorMove(Vector2 vec)
    {
        // right
        if(vec.x == 1 && GameManager.Instance.player.gameObject.transform.position.x < transform.position.x)
        {
            moveVec = new Vector2(1, 0);
            rigid.MovePosition(rigid.position + moveVec);
        }
        // left
        else if (vec.x == -1 && GameManager.Instance.player.gameObject.transform.position.x > transform.position.x)
        {
            moveVec = new Vector2(-1, 0);
            rigid.MovePosition(rigid.position + moveVec);
        }
        // up
        else if (vec.y == 1 && GameManager.Instance.player.gameObject.transform.position.y < transform.position.y)
        {
            moveVec = new Vector2(0, 1);
            rigid.MovePosition(rigid.position + moveVec);
        }
        // down
        else if (vec.y == -1 && GameManager.Instance.player.gameObject.transform.position.y > transform.position.x)
        {
            moveVec = new Vector2(0, -1);
            rigid.MovePosition(rigid.position + moveVec);
        }

        curDelay = 0f;
        isMove = false;

    }*/
}
