using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public enum RoomStatus // ���� ���� ����
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


    // ���� �÷��̾ �濡 ���� ����
    private void PlayerRoom(bool check)
    {
        isPlayerRoom = check;
        RoomController.Instance.ChangePlayerRoom(roomIndex); // �ʿ����� ���
    }
    
    // ���� �濡 �ִ� ���͵��� Ȱ��ȭ ��Ű�� ����, �� �� ������ �ڵ� ���������� ������ ����
    public void AgentActive(bool check)
    {
        if (check && agetns != null)
        {
            foreach (Agent a in agetns)
                a.PlayerRoom();
        }
    }
}
