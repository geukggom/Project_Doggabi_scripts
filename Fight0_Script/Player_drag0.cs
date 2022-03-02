using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_drag0 : MonoBehaviour
{
    public Transform player_parent;
    List<Transform> players = new List<Transform>();
    int playerCount = 0;
    float panel_speed = 5f;
    float a;

    bool moremove = false;

    private void Start()
    {
        player_parent = FindAtoZ.Find_tag(GlobalAtoZ.Tag_PlayerOBJ).transform;      //전투 씬 처음부터 플레이어가 있다고 가정함.

        for (int i = 0; i < player_parent.childCount; i++)
        {
            if (player_parent.GetChild(i).gameObject.activeSelf) 
            {
                playerCount++;     //플레이어가 활성 상태면 추가.
                players.Add(player_parent.GetChild(i));
            }
        }
    }

    /// <summary>
    /// by지선, 전투 패널 클릭 - 캐릭터들을 이동 시킬 위치 표시
    /// </summary>
    private void OnMouseDown()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) ;
        Vector3 a = new Vector3(pos.x, transform.position.y, -Camera.main.transform.position.z);
        this.a = a.x;
        transform.GetChild(0).GetComponent<Image>().enabled = true;
        transform.position = a;
        moremove = false;
    }

    /// <summary>
    /// by지선, 캐릭터들을 이동시킬 위치 조정
    /// </summary>
    private void OnMouseDrag()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 b = new Vector3(pos.x, transform.position.y, -Camera.main.transform.position.z);
        this.a = b.x;
        if (!moremove && Vector3.Distance(transform.GetChild(0).position, b) > 1f)
        {
            moremove = true;
            for (int i = 0; i < playerCount+1; i++)
            {
                if(i==0) transform.GetChild(0).GetComponent<Image>().enabled = false;
                else
                {
                    transform.GetChild(i).GetComponent<Image>().enabled = true;
                }
            }
        }
        if (moremove)
        {
            if (playerCount == 3)
            {
                transform.GetChild(2).position = Vector3.Lerp(transform.GetChild(2).position, (transform.position + b)/2, Time.deltaTime * panel_speed);
                transform.GetChild(3).position = Vector3.Lerp(transform.GetChild(3).position, b, Time.deltaTime * panel_speed);
            }
            else if(playerCount == 2)
            {
                transform.GetChild(2).position = Vector3.Lerp(transform.GetChild(2).position, b, Time.deltaTime * panel_speed);
            }
        }
    }

    /// <summary>
    /// by지선, 위치를 표시하는 막대가 사라지고 캐릭터들 이동
    /// </summary>
    private void OnMouseUp()
    {
        if (moremove) //드래그로 위치를 변경했을 경우(캐릭터 셋이 정렬)
        {
            for (int i = 0; i < playerCount; i++)
            {
                player_parent.GetChild(i).GetComponent<PlayerManager0>().playermove(transform.GetChild(i + 1).position.x);
            }
        }
        else //드래그 하지 않고 클릭만 했을 경우(캐릭터 셋이 같은 위치로 이동)
        {
            for (int i = 0; i < playerCount; i++)
            {
                player_parent.GetChild(i).GetComponent<PlayerManager0>().playermove(a);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i).position = transform.position;
            transform.GetChild(i).GetComponent<Image>().enabled = false; //막대 이미지 사라짐
        }
    }
}
