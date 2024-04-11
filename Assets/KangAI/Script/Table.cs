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
    TableArrow agentArrow; // agent가 기댈 수 있는 방향
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

            Debug.Log("들어옴");
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
            Debug.Log("나옴");
        }

        if (enemyCheck && curArrow != TableArrow.none && collision.gameObject.tag.Equals("Agent"))
        {
            enemyCheck = false;
        }
    }

    // 위치 계산
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

            // 애니메이션 대신 일단 스프라이트 변경
            tableSprite.sprite = Sprites[(int)curArrow];
            //playerCheck = false; //이건 테스트 반드시 끝나면 활성화

            // 테이블 크기 변경

        }
    }

    // 각도 계산
    private float VectorValue(Vector3 value)
    {
        float angle;
        angle = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg; // 범위가 180 ~ -180

        return angle;
    }

    // 방향 설정
    private int AngleCalculate(float angleValue)
    {
        int Index = -1;
        // 1사분면, 왼 윗 대각까진 우선순위
        if (angleValue <= 135f && angleValue > 45f)
        {
            Index = 0; // up
            distance[Index] = transform.position + new Vector3(0, 1, 0);
        }
        // 2사분면, 오른 윗 대각까진 우선순위
        else if (angleValue <= 45f && angleValue > -45f)
        {
            Index = 3; // right
            distance[Index] = transform.position + new Vector3(0.5f, 0, 0);
        }
        // 3사분면, 오른 아랫대각까진 우선
        else if (angleValue <= -45f && angleValue > -135f)
        {
            Index = 1; // down 
            distance[Index] = transform.position + new Vector3(0, -1, 0);
        }
        // 4사분면, 왼쪽 아랫대각까진 우선
        else if (angleValue <= -135f || angleValue > 135f)
        {
            Index = 2; // left
            distance[Index] = transform.position + new Vector3(-0.5f, 0, 0);
        }

        return Index;
    }

    // 현재 테이블 방향 표시 용도
    private void ActiveLine(bool activeTrue)
    {
        foreach (GameObject g in lineObj)
            g.SetActive(false);

        if(activeTrue) lineObj[lineIndex].SetActive(true);
    }

    // AI 테이블 기대기 여부 계산
    private void LeanAgent(Vector3 vec, GameObject agentObj)
    {
        moveVec = GameManager.Instance.player.transform.localPosition;

        int playerLine = -1;
        int agentLine = -1;

        float playerVec = VectorValue(moveVec - transform.position);
        float agentVec = VectorValue(vec - transform.position);

        playerLine = AngleCalculate(playerVec);
        agentLine = AngleCalculate(agentVec);

        Debug.Log("계산 체크");
        Debug.Log("playerLine : " + playerLine);
        Debug.Log("agentLine : " + agentLine);

        Debug.Log("agentArrow : " + agentArrow);
        Debug.Log("curagentArrow : " + (TableArrow)agentLine);

        if (playerLine == agentLine || agentArrow != (TableArrow)agentLine) return;
        Debug.Log("문제 체크");

        //agentObj.GetComponent<Agent>().Lean(agentArrow); // 잠시 주석

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

        // 애니메이션 대신 일단 스프라이트 변경
        //tableSprite.sprite = Sprites[(int)curArrow];


        // 추후 콜라이더도 변경
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
