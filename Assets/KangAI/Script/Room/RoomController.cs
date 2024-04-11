using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField]
    private List<Room> rooms;
    public List<Room> Rooms => rooms;


    private static RoomController instance;
    public static RoomController Instance => instance;

    private int curIndex = 0;
    public int CurIndex => curIndex;


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

    void Init()
    {
        instance = this;
    }

    // ¹Ì´Ï¸Ê ¼öÁ¤
    public void ChangePlayerRoom(int num)
    {
        curIndex = num;
        
        //CheckEnemy(curIndex);
    }
 
    /*
    private void CheckEnemy(int num)
    {
        rooms[num]?.AgentActive(false);

     }
    */


}
