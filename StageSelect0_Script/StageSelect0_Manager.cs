using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelect0_Manager : MonoBehaviour
{
    Transform duruContent;      //스테이지 넣을 곳
    GameObject duru_prefab;     //스테이지 프리팹
    public int stage_num;       //스테이지 총 개수
    public int stage_num_now;   //열린 스테이지 개수

    Transform duruCenter;       //아까 가운데 있던 두루마리
    int duruCenter_num;         //아까 가운데 있던 두루마리 번호
    int duruCenter_num_now;     //지금 가운데 있는 두루마리 번호

    bool is_touch = false;      //터치하면 켜지는 bool

    Sprite[] openduru_IMG = new Sprite[3];

    //public Transform[] map_list;

    void Start()
    {
        Time.timeScale = 1f;
        duruContent = FindAtoZ.Find_tag(GlobalAtoZ.Tag_DuruContent).transform;
        duru_prefab = Resources.Load<GameObject>(GlobalAtoZ.Prefab_Duru);
        stage_num = 3;
        stage_num_now = 3;
        openduru_IMG = Resources.LoadAll<Sprite>(GlobalAtoZ.Sprite_opinduru_IMG);

        for (int i = 0; i < stage_num + 4; i++)
        {
            GameObject a = Instantiate(duru_prefab);
            a.transform.SetParent(duruContent);
            a.GetComponent<RectTransform>().localScale = Vector3.one;
            if (i >= 2 && i < stage_num_now + 2) 
            { 
                duruContent.GetChild(i).GetChild(0).gameObject.SetActive(true);
                duruContent.GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = openduru_IMG[i - 2];         // 스테이지 그림 바꾸는 곳.
            }
            else if (i >= stage_num_now + 2 && i < stage_num + 2) duruContent.GetChild(i).GetChild(1).gameObject.SetActive(true);
        }
        durunow(2);     //여기까지 두루마리 코드
        duruCenter_num_now = 2;
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////두루마리 코드<>
    private void OnMouseDown()      //클릭Down
    {
        is_touch = true;
    }
    Coroutine ForMapmove;
    private void OnMouseDrag()      //드래그할 때, 가운데 오는 두루마리가 열리게
    {
        for (int i = 0; i < duruContent.childCount; i++)
        {
            if (duruContent.GetChild(i).position.x >= 755 && duruContent.GetChild(i).position.x < 1155)
            {
                is_touch = false;
                if (duruCenter_num == i) return;
                duruCenter.GetComponent<DuruMove0>().Closing();
                durunow(i);

                StartCoroutine(ContentMove(i));
                if (ForMapmove != null) StopCoroutine(ForMapmove);
                ForMapmove = StartCoroutine(MapMove(i-2));

                i = duruContent.childCount - 1;
                //Debug.Log(i);
            }
        }
        //Debug.Log(duruContent.position);
    }
    private void OnMouseUp()        //손가락 뗄때, 두루마리 위치 정렬
    {
        for (int i = 0; i < duruContent.childCount; i++)
        {
            if(duruContent.GetChild(i).position.x >= 755 && duruContent.GetChild(i).position.x < 1155)
            {
                //Debug.Log("a : " + duruContent.GetChild(i).position.x);
                is_touch = false;
                duruCenter_num_now = i;
                StartCoroutine(ContentMove(i));
                
                i = duruContent.childCount - 1;
            }
        }
        StartCoroutine(MapMove(duruCenter_num_now-2));
    }
    IEnumerator ContentMove(int a)
    {
        Vector2 pos = new Vector2(-360 * (a - 2), duruContent.position.y);
        //Debug.Log(pos);
        while (true)
        {
            if (is_touch || duruContent.position == (Vector3)pos) break;
            duruContent.position = Vector2.MoveTowards(duruContent.position, pos,Time.deltaTime * 500f);
            yield return null;
        }
    }
    void durunow(int a)
    {
        if (a == duruCenter_num) return;
        duruContent.GetChild(a).GetComponent<DuruMove0>().Opening();
        duruCenter = duruContent.GetChild(a);
        duruCenter_num = a;
    }
   
    IEnumerator MapMove(int a)
    {
        Transform maincam = Camera.main.transform;
        while (true)
        {
            maincam.position = Vector3.Lerp(maincam.position, new Vector3(19.2f * a,0,-10),Time.deltaTime * 20f);
            if (is_touch || Vector3.Distance(maincam.position, new Vector3(19.2f * a, 0, -10)) < 0.1f) break;
            yield return null;
        }
        //Debug.Log("끔");
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////두루마리 코드</>




    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////씬 이동(전투씬으로)<> 
    
    public void ToFightScene()
    {
        if (duruContent.GetChild(duruCenter_num_now).GetChild(0).gameObject.activeSelf)
        {
            
            switch (duruCenter_num_now - 2)
            {
                case 0:
                    GameObject a0 = new GameObject("Fight_Information");
                    a0.AddComponent<FightInformation0>().stagenum = 0; //스테이지 번호 입력
                    DontDestroyOnLoad(a0); //씬 전환시에도 사라지지않게
                    SceneManager.LoadScene(2); //씬 전환
                    break;
                case 1:
                    GameObject a1 = new GameObject("Fight_Information");
                    a1.AddComponent<FightInformation0>().stagenum = 1;       
                    DontDestroyOnLoad(a1);                                   
                    SceneManager.LoadScene(2);                              
                    break;
                case 2:
                    Debug.Log("두억시니 스테이지");
                    SceneManager.LoadScene(3);
                    break;
            }
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////씬 이동(전투씬으로)</>
}
