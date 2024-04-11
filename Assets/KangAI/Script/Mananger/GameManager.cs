using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    // 임시 플레이어 등록
    public TestPlayer player;

    private bool isPause = false; //일단 현재는 인게임 상태이므로 일시정지 해제
    public bool IsPause => isPause;

    [Header("플레이어")]
    private int hp;
    private int maxHp;

    private void Awake()
    {
        Init();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Pause(bool check)
    {
        isPause = check;
    }
}
