using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// fsm 패턴 찾아서 적용시켜보기

public class Monster : MonoBehaviour
{
    Rigidbody2D rigid;
    Rigidbody2D player;

    // status, 추후에 private로 변경
    public float speed = 2.0f;
    public int damage = 1;
    public bool isAttack;
    public float attackDistance = 0.0f;
    public float attackDelay = 1.0f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Chase();
    }

    private void Init()
    {
        player = GameManager.Instance.player.GetComponent<Rigidbody2D>();
    }

    private void Chase()
    {
        if (isAttack) return;

        if (Vector2.Distance(player.position, rigid.position) <= attackDistance)
        {
            StartCoroutine(Attack());
            return;
        }
        Vector2 direction = player.position - rigid.position;
        Vector2 nextVec = direction.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    private IEnumerator Attack()
    {
        isAttack = true;
        Debug.Log("공격");

        yield return new WaitForSeconds(attackDelay);
        isAttack = false;
    }

}
