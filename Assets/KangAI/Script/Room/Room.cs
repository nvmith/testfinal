using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public enum RoomStatus // 현재 방의 상태
{
    
}

public class Room : MonoBehaviour
{
    [SerializeField]
    List<Agent> agetns;
    bool isPlayerRoom = false;

    [SerializeField]
    private int roomIndex;
    public int RoomIndex => roomIndex;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerRoom(true);
            AgentActive(isPlayerRoom);
        }
    }


    // 현재 플레이어가 방에 들어온 상태
    private void PlayerRoom(bool check)
    {
        isPlayerRoom = check;
        RoomController.Instance.ChangePlayerRoom(roomIndex); // 필요할지 고민
    }
    
    // 현재 방에 있는 몬스터들을 활성화 시키는 로직, 추 후 몬스터의 자동 움직임으로 구현을 변경
    public void AgentActive(bool check)
    {
        if (check && agetns != null)
        {
            foreach (Agent a in agetns)
                a.PlayerRoom();
        }
    }
}
