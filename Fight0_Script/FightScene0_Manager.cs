using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.SceneManagement;

public class FightScene0_Manager : MonoBehaviour
{
    Player_drag0 Player_drag0;
    FightInformation0 FightInformation;
    Transform Player_Move_Canvas;
    GameObject enemy_parent;
    public PlayerManager0[] player_manager;
    float playerspeed;
    int wavenum = 0;
    float boss_distance;
    int now_wavenum;
    Transform map;
    public bool Player_Setting_over = false;
    public bool Enemy_Setting_over = false;
    bool scene_start = true;
    public bool is_fight = false;
    public bool is_real_fight = false;

    Transform Canvas_Jjin;
    GameObject stage_img;
    Transform stage_mark;
    Transform Victory_panel;                //승리패널이랑 보스 패널은 임시임!
    Transform Boss_panel;
    Button PauseButton;
    Transform PausePanel;
    int stagenum;
    Transform camerapos;

    void Start()
    {
        camerapos = FindAtoZ.Find_tag(GlobalAtoZ.Tag_CameraPosition).transform;
        Player_drag0 = GameObject.FindObjectOfType<Player_drag0>();
        Canvas_Jjin = FindAtoZ.Find_tag(GlobalAtoZ.Tag_Canvas_FightScene).transform;
        Victory_panel = Canvas_Jjin.GetChild(2);
        Boss_panel = Canvas_Jjin.GetChild(3);

        stage_img = Canvas_Jjin.GetChild(1).GetChild(0).gameObject;
        stage_mark = Canvas_Jjin.GetChild(1).GetChild(1);

        PauseButton = Canvas_Jjin.GetChild(1).GetChild(2).GetComponent<Button>();
        PauseButton.onClick.AddListener(PauseButtonOn);

        PausePanel = Canvas_Jjin.GetChild(1).GetChild(3).transform;
        PausePanel.GetChild(0).GetComponent<Button>().onClick.AddListener(PauseButtonOff);

        FightInformation = GameObject.FindObjectOfType<FightInformation0>();
        Player_Move_Canvas = FindAtoZ.Find_tag(GlobalAtoZ.Tag_Canvas_PlayerMove).transform;
        Player_Move_Canvas.gameObject.SetActive(false);
        now_wavenum = -1;
        map = FindAtoZ.Find_tag(GlobalAtoZ.Tag_FightScene_bg).transform;

        StartCoroutine(EnemyInformationSetting());
        StartCoroutine(PlayerSetting());
    }
    IEnumerator EnemyInformationSetting()
    {
        while (true)
        {
            if (FightInformation.is_complete)         //적 정보 생성 완료되면 적 만들기 시작
            {
                wavenum = FightInformation.Wave_num;
                boss_distance = FightInformation.boss_distance;
                stagenum = FightInformation.stagenum+2;
                if(stagenum == 2)
                {
                    stage_img.SetActive(false);
                    stage_mark.gameObject.SetActive(false);
                }
                //FindAtoZ.Find_tag(GlobalAtoZ.Tag_FightScene_bg).GetComponent<SpriteRenderer>().sprite = FightInformation.Stage_bg;      //배경화면 넣어줌
                break; 
            }
            yield return null;
        }
        EnemyMaking();
    }
    List<Transform> enemy_list0000 = new List<Transform>();
    int enemycount = 0;
    int enemycount_now = 0;
    public Text enemycc;
    void EnemyMaking()
    {
        List<int>[] Wave_Enemynum = new List<int>[wavenum];
        for (int i = 0; i < wavenum; i++)
        {
            Wave_Enemynum[i] = new List<int>();
        }
        Wave_Enemynum = FightInformation.Wave_Enemynum;
        enemy_parent = new GameObject("EnemyParent");
        int layer_num = 2;
        for (int i = 0; i < wavenum; i++)
        {
            GameObject a = new GameObject("EnemyWave" + i);
            a.transform.SetParent(enemy_parent.transform);

            enemycount += Wave_Enemynum[i].Count;
            for (int j = 0; j < Wave_Enemynum[i].Count; j++)
            {
                GameObject b = Instantiate(FightInformation.Enemy[Wave_Enemynum[i][j]]);
                enemy_list0000.Add(b.transform);
                b.AddComponent<EnemyManager0>().EnemyNum = FightInformation.EnemyNum[Wave_Enemynum[i][j]];
                b.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = layer_num;
                layer_num += 2;
                b.transform.SetParent(a.transform);
            }
        }
        Debug.Log(enemy_list0000.Count);
        enemycount_now = enemycount;
        enemy_parent.transform.position = new Vector3(14, 0, 0);
    }

    IEnumerator PlayerSetting()
    {
        while (true)
        {
            if (FindAtoZ.Find_tag(GlobalAtoZ.Tag_PlayerOBJ))
            {
                player_manager = FindAtoZ.Find_tag(GlobalAtoZ.Tag_PlayerOBJ).GetComponentsInChildren<PlayerManager0>();
                for (int i = 0; i < player_manager.Length; i++)
                {
                    player_manager[i].playernum = i;
                    player_manager[i].enter_playernum = true;
                }
                Enemy_Setting_over = true;
                break;
            }
            yield return null;
        }
    }

    private void Update()
    {
        int now_enemycount = 0;
        for (int i = 0; i < enemy_list0000.Count; i++)
        {
            if (enemy_list0000[i].gameObject.activeSelf) now_enemycount++;
        }
        enemycc.text = "Enemy : " + now_enemycount.ToString() + " / " + enemycount.ToString();
        //Debug.Log(now_enemycount);

        if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(CameraShaking(1f,5f));
        if (!Player_Setting_over || !Enemy_Setting_over) return;

        if (scene_start)            //씬 처음 시작하고 플레이어, 맵 이동.
        {
            scene_start = false;
            StartCoroutine(SceneStart0());
        }

        if (is_fight)           //wave가 하나 끝날 때마다(또는 시작할 때) 다음 wave를 준비
        {
            is_fight = false;
            if (now_wavenum >= 0)
            {
                for (int i = 0; i < enemy_parent.transform.GetChild(now_wavenum).childCount; i++)
                {
                    if (enemy_parent.transform.GetChild(now_wavenum).GetChild(i).gameObject.activeSelf) return;     //wave가 진짜 끝났는지
                }
                for (int i = 0; i < player_manager.Length; i++)
                {
                    player_manager[i].is_fight = false;
                }
            }
            
            FindWave();
        }
    }

    IEnumerator SceneStart0()
    {
        playerspeed = 5f;
        while (true)
        {
            if (player_manager[0].transform.position.x >= 2) break;
            for (int i = 0; i < player_manager.Length; i++)
            {
                if (i == 0) player_manager[i].transform.position = Vector3.MoveTowards(player_manager[i].transform.position, player_manager[i].transform.position + Vector3.right, Time.deltaTime * playerspeed);
                else player_manager[i].transform.position = player_manager[i - 1].transform.position + Vector3.left * 1.4f;
            }
            yield return null;
        }
        while (true)
        {
            if (map.position.x <= 6) break;
            map.position = Vector3.MoveTowards(map.position, map.position + Vector3.left, Time.deltaTime * playerspeed);
            yield return null;
        }
        for (int i = 0; i < player_manager.Length; i++)
        {
            player_manager[i].now_anim(0,0,true,1);
        }
        is_fight = true;
    }

    void FindWave()
    {
        now_wavenum++;

        if (now_wavenum == wavenum) 
        {
            for (int i = 0; i < player_manager.Length; i++)
            {
                player_manager[i].now_anim(7,0,true,1f);
            }
            Victory_panel.gameObject.SetActive(true);
            FindAtoZ.Find_tag(GlobalAtoZ.Tag_Canvas_PlayerMove).transform.GetChild(0).gameObject.SetActive(false);
            StartCoroutine(SelectSceneAgain());
            return; 
        }                                                                                                               //웨이브 모두 끝남 - 전투끝(승리 포즈)

        if(now_wavenum != 0) stage_mark.position += Vector3.right * 180;                                                                     //몇번째 웨이브인지 표시.

        for (int i = 0; i < enemy_parent.transform.GetChild(now_wavenum).childCount; i++)
        {
            int num = Random.Range(0, 2);           //적 위치 랜덤 생성(0이면 왼쪽, 1이면 오른쪽)
            if (now_wavenum == wavenum - 1 && i == enemy_parent.transform.GetChild(now_wavenum).childCount - 1) 
            {
                enemy_parent.transform.GetChild(now_wavenum).GetChild(i).GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 0;
                num = 1;         //보스는 무조건 오른쪽 생성
            }

            if(num == 0)
            {
                enemy_parent.transform.GetChild(now_wavenum).GetChild(i).position = new Vector3(-14, 0, 0);
                enemy_parent.transform.GetChild(now_wavenum).GetChild(i).rotation = Quaternion.Euler(0,180,0);
                enemy_parent.transform.GetChild(now_wavenum).GetChild(i).GetChild(1).rotation = Quaternion.Euler(0,0,0);

            }
            else
            {
                enemy_parent.transform.GetChild(now_wavenum).GetChild(i).position = new Vector3(13, 0, 0);
            }
            enemy_parent.transform.GetChild(now_wavenum).GetChild(i).GetComponent<EnemyManager0>().player_list = player_manager[0].player_list;     //적들의 플레이어리스트 넣어줌
        }

        StartCoroutine(WaveTurn());
    }
    IEnumerator WaveTurn()
    {
        if(now_wavenum != 0)
        {
            Vector3 destination = map.position + Vector3.left * 6f;
            for (int i = 0; i < player_manager.Length; i++)
            {
                player_manager[i].now_anim(5, 0, true, 1f);
                player_manager[i].transform.GetChild(0).rotation = Quaternion.Euler(Vector3.zero);
            }
            while (true)        //플레이어들 가운데로 모이고, 맵은 8만큼 이동하고.
            {
                for (int i = 0; i < player_manager.Length; i++)
                {
                    if (i == 0) player_manager[i].transform.position
                            = Vector3.MoveTowards(player_manager[i].transform.position, new Vector3(2, player_manager[i].transform.position.y, player_manager[i].transform.position.z),
                            Time.deltaTime * playerspeed);
                    else
                    {
                        player_manager[i].transform.position
                            = Vector3.MoveTowards(player_manager[i].transform.position
                            , new Vector3(player_manager[i - 1].transform.position.x - 2, player_manager[i].transform.position.y, player_manager[i].transform.position.z)
                            , Time.deltaTime * playerspeed * 2);
                    }
                }
                map.position = Vector3.MoveTowards(map.position, destination, Time.deltaTime * playerspeed);
                if (Vector3.Distance(destination, map.position) < 0.1f) break;
                yield return null;
            }
            for (int i = 0; i < player_manager.Length; i++)
            {
                player_manager[i].now_anim(0, 0, true, 1f);
            }
        }

        Player_Move_Canvas.gameObject.SetActive(true);
        StartCoroutine(Enemy_move());
    }
    IEnumerator Enemy_move()
    {
        int a = enemy_parent.transform.GetChild(now_wavenum).childCount-1;
        bool a_changed = true;
        while (true)
        {
            if(a_changed) 
            {
                a_changed = false;
                enemy_parent.transform.GetChild(now_wavenum).GetChild(a).GetComponent<EnemyManager0>().now_anim(1, 0, true, 1f);        //처음 싸울 때 달리는 모션 넣어줌.
            }
            enemy_parent.transform.GetChild(now_wavenum).GetChild(a).position 
                = Vector3.MoveTowards(enemy_parent.transform.GetChild(now_wavenum).GetChild(a).position, Vector3.zero, Time.deltaTime * playerspeed);
            if(Mathf.Abs(enemy_parent.transform.GetChild(now_wavenum).GetChild(a).position.x) < 7f)                     //한마리씩 화면으로 등장하면 싸움 시작
            {
                if(now_wavenum == wavenum -1 && a == enemy_parent.transform.GetChild(now_wavenum).childCount - 1)      //보스 패널 등장
                {
                    StartCoroutine(Camera_zoomInOut());
                    enemy_parent.transform.GetChild(now_wavenum).GetChild(a).GetComponent<EnemyManager0>().now_anim(4,0,false,1f);

                    while (true)
                    {
                        yield return null;
                        for (int i = 0; i < player_manager.Length; i++)
                        {
                            player_manager[i].transform.position 
                                = Vector3.MoveTowards(player_manager[i].transform.position, player_manager[i].transform.position+Vector3.right * boss_distance,Time.deltaTime * 5f);
                        }
                        enemy_parent.transform.GetChild(now_wavenum).GetChild(a).position
                             = Vector3.MoveTowards(enemy_parent.transform.GetChild(now_wavenum).GetChild(a).position,
                             enemy_parent.transform.GetChild(now_wavenum).GetChild(a).position + Vector3.right * boss_distance, Time.deltaTime * 5f);
                        map.position = Vector3.MoveTowards(map.position, map.position + Vector3.right * boss_distance, Time.deltaTime * 5f);
                        if (enemy_parent.transform.GetChild(now_wavenum).GetChild(a).position.x < 7f + boss_distance) { yield return new WaitForSeconds(1f); break; }
                    }

                    Player_drag0.gameObject.SetActive(false);
                    Boss_panel.gameObject.SetActive(true);
                    yield return new WaitForSeconds(1.5f);
                    Boss_panel.gameObject.SetActive(false);
                    Player_drag0.gameObject.SetActive(true);
                }

                if(a == enemy_parent.transform.GetChild(now_wavenum).childCount - 1)                                    //첫 적이 등장했을 때 플레이어 전투 시작
                {
                    for (int i = 0; i < player_manager.Length; i++)
                    {
                        player_manager[i].is_fight = true;
                    }
                }

                for (int i = 0; i < player_manager.Length; i++)                                                         //적 리스트에 넣어줌
                {
                    player_manager[i].enemy_list.Add(enemy_parent.transform.GetChild(now_wavenum).GetChild(a));
                }
                for (int i = 0; i < enemy_parent.transform.GetChild(now_wavenum).childCount; i++)
                {
                    enemy_parent.transform.GetChild(now_wavenum).GetChild(i).GetComponent<EnemyManager0>().enemy_list.Add(enemy_parent.transform.GetChild(now_wavenum).GetChild(a));
                }
                enemy_parent.transform.GetChild(now_wavenum).GetChild(a).GetComponent<EnemyManager0>().is_fight = true;
                a--;
                a_changed = true;
                yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            }
            if (a < 0) break;
            yield return null;
        }
    }
    IEnumerator Camera_zoomInOut()      //보스 등장시 쓰는 카메라줌아웃 코르틴
    {
        float time_ = 0;
        float speed = 4f;
        Camera cam = Camera.main;
        while (true)
        {
            time_ += Time.deltaTime;
            if (time_ > 2f) break; 
            yield return null;

            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(3.5f, -1f, -10), Time.deltaTime * speed);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,4f, Time.deltaTime * speed);
        }

        time_ = 0;
        while (true)
        {
            time_ += Time.deltaTime;
            if (time_ > 2f) break;
            yield return null;
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(0, 0, -10), Time.deltaTime * speed);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 5.3f, Time.deltaTime * speed);
        }
    }
    
    void PauseButtonOn()
    {
        if(!Boss_panel.gameObject.activeSelf && !Victory_panel.gameObject.activeSelf)
        {
            Time.timeScale = 0;
            PausePanel.gameObject.SetActive(true);
        }
    }
    void PauseButtonOff()
    {
        Time.timeScale = 1;
        PausePanel.gameObject.SetActive(false);
    }

    IEnumerator SelectSceneAgain()
    {
        yield return new WaitForSeconds(5f);
        if(PlayerPrefs.GetInt(GlobalAtoZ.String_STAGENUM) < stagenum) PlayerPrefs.SetInt(GlobalAtoZ.String_STAGENUM,stagenum);
        SceneManager.LoadScene(1);
    }

    Coroutine ForcamraShaking;
    Vector3 oripos = new Vector3(0, 0, -10);
    /// <summary>
    /// by지선, 공격당했을 때, 카메라 흔들림
    /// </summary>
    /// <param name="time"></param>
    public void ShakingCoroutine(float time)
    {
        if (ForcamraShaking != null) StopCoroutine(ForcamraShaking);
        ForcamraShaking = StartCoroutine(CameraShaking(1f, time));
    }
    IEnumerator CameraShaking(float a,float time)
    {
        float mytime = 0;
        Camera maincam = Camera.main;
        oripos = new Vector3(0, 0, -10);
        while (true)
        {
            if (now_zoom) oripos = fororipos;
            else oripos = new Vector3(0, 0, -10);
            //카메라가 줌인하고 있을 경우, 줌인한 위치를 중심으로, 그렇지 않을 경우 중앙을 중심으로
            mytime += Time.deltaTime;
            if (mytime > time) break;
            yield return null;
            float bb = Random.Range(-0.1f, 0.1f); //랜덤하게 흔들리는 정도
            float b = Random.Range(-0.1f, 0.1f);
            maincam.transform.position = oripos + Vector3.up * bb *a + Vector3.right * b*a;
        }
        maincam.transform.position = oripos;
        ForcamraShaking = null;
    }

    Coroutine zoominoutCoroutine;
    bool now_zoom = false;
    Vector3 fororipos;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="me"> 줌인할 대상 </param>
    /// <param name="time"> 줌인 시간 </param>
    public void zoomCoroutine(Transform me, float time)
    {
        if (zoominoutCoroutine != null) StopCoroutine(zoominoutCoroutine);
        zoominoutCoroutine = StartCoroutine(Player_zoomInOut(me, time));
    }
    IEnumerator Player_zoomInOut(Transform me, float time)      //플레이어가 스킬을 쓸 때 줌인아웃
    {
        float mytime = 0;
        float speed = 4f;
        Camera cam = Camera.main;
         
        now_zoom = true;
        while (true)        //줌인 while문
        {
            mytime += Time.deltaTime;
            if (mytime > time) break;
            yield return null;
            fororipos = camerapos.position;
            cam.transform.position 
                = Vector3.Lerp(cam.transform.position, new Vector3(Mathf.Clamp(me.position.x, -3.5f, 3.5f), me.position.y - 1f, -10), Time.deltaTime * speed);
            camerapos.position 
                = Vector3.Lerp(cam.transform.position, new Vector3(Mathf.Clamp(me.position.x, -3.5f, 3.5f), me.position.y - 1f, -10), Time.deltaTime * speed);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 3.5f, Time.deltaTime * speed);
        }
        mytime = 0;
        while (true)        //줌아웃 while문
        {
            mytime += Time.deltaTime;
            if (mytime > time) break;
            yield return null;
            fororipos = camerapos.position;
            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(0, 0, -10), Time.deltaTime * speed);
            camerapos.position = Vector3.Lerp(cam.transform.position, new Vector3(0, 0, -10), Time.deltaTime * speed);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 5.3f, Time.deltaTime * speed);
        }
        now_zoom = false;
        zoominoutCoroutine = null;
    }

    bool is_fast = false;
    public Sprite[] button_x12;
    public Image buttonimg_x12;
    public void timescale_change()
    {
        if (!is_fast)
        {
            is_fast = true;
            Time.timeScale = 2f;
            buttonimg_x12.sprite = button_x12[1];
        }
        else
        {
            is_fast = false;
            Time.timeScale = 1f;
            buttonimg_x12.sprite = button_x12[0];
        }
    }
}