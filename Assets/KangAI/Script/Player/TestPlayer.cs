using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// hp같은 정보는 어차피 싱글게임이라 게임매니저에서 관리해도 좋을듯

public class TestPlayer : MonoBehaviour
{
    Rigidbody2D rigid;

    float speed = 3.0f;
    Vector2 inputVec;
    public Vector2 InputVec => inputVec;

    [SerializeField]
    private int hp = 10;
    public int Hp => hp;

    public bool isDead;

    // 무기 구현이후 attackDelay알맞게 수정
    public float attackDelay = 1.0f;
    public float curAttackDelay;



    // InputKey
    public bool isAttack;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        InputKey();
    }

    private void FixedUpdate()
    {
        Move();
        Attack();
    }
    private void InputKey()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
        isAttack = Input.GetButton("Fire1");
    }

    private void Move()
    {
        Vector2 nextVect = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVect);
    }

    public void Damage(int power)
    {
        if (!isDead) return;

        hp -= power;
        Debug.Log("피격당함");
        Debug.Log("현재 체력 : " + hp);
    }

    // 무기 기능 구현하여 추가하기
    private void Attack()
    {
        curAttackDelay += Time.deltaTime;

        if (isAttack && curAttackDelay >= attackDelay)
        {
            curAttackDelay = 0;
            StartCoroutine(IAttack());
        }
    }

    public IEnumerator IAttack()
    {
        
        Debug.Log("공격");
        yield return new WaitForSeconds(1.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("플레이어에 들어온 트리거");
    }
}
