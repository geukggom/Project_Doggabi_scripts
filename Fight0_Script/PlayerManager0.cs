using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;
using System;

public class PlayerManager0 : PlayerStat
{
    public Sprite[] players_sprite;                 //UI 플레이어 이미지
    public Sprite[] player_Job_sprite;              //플레이어 이름 옆 탱/딜/힐 마크
    FightScene0_Manager FightScene0_Manager;
    Damage_Manager Damage_Manager;

    public float hp;
    public int playernum = -1;
    public bool enter_playernum = false;            //UI에서 몇번째 플레이어인지 SceneManager에서 받아옴
    public bool is_fight = false;                   //캐릭터 이동까지 전부 끝난 후 전투 시작
    public bool target_select = true;               //타겟을 내가 지정하는지, 자동으로 지정하는지
    bool is_moving = true;                          //드래그로 플레이어가 움직일 때 중첩되지 않게
    float player_speed = 5f;

    Transform Canvas;
    public Button[] Skill_buttons = new Button[2]; 
    Image hp_bar1;
    Slider hp_bar2;
    Text hp_now_text;

    public Transform attack_target;
    public Transform hill_target;

    public List<Transform> enemy_list;
    public List<Transform> player_list;

    float attack_time = 3f;                 //기본공격을 2초에 한 번 하도록.
    float my_time = 0f;
    
    bool skill_use = false;
    bool target_selected = false;
    bool target_move = false;

    void Start()
    {
        hill_target = transform;
        Damage_Manager = GameObject.FindObjectOfType<Damage_Manager>();
        players_sprite = Resources.LoadAll<Sprite>(GlobalAtoZ.Sprite_Player_img);
        player_Job_sprite = Resources.LoadAll<Sprite>(GlobalAtoZ.Sprite_Player_Job_img);
        FightScene0_Manager = GameObject.FindObjectOfType<FightScene0_Manager>();

        if (gameObject.tag == GlobalAtoZ.Tag_player_WATER_Doggabi) Player_Stat(0);
        else if(gameObject.tag == GlobalAtoZ.Tag_player_FIRE_Doggabi) Player_Stat(1);
        else if(gameObject.tag == GlobalAtoZ.Tag_player_GRASS_Doggabi) Player_Stat(2);

        Canvas = FindAtoZ.Find_tag(GlobalAtoZ.Tag_Canvas_FightScene).transform;
        hp = fullhp;
        hp_bar1 = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
        StartCoroutine(CanvasSetting());

        now_anim(5,0,true,1f);
    }
    IEnumerator CanvasSetting()
    {
        while (true)
        {
            if(enter_playernum)
            {
                myanimation.state.Event += myAniKey;

                Canvas.GetChild(0).GetChild(playernum).GetChild(0).GetComponent<Image>().sprite = players_sprite[myOwnNumber];                  //플레이어 얼굴 이미지
                for (int i = 0; i < 2; i++) Skill_buttons[i] = Canvas.GetChild(0).GetChild(playernum).GetChild(i + 1).GetComponent<Button>();   //플레이어 스킬 버튼 연결
                Skill_buttons[0].onClick.AddListener(Skill1);
                Skill_buttons[1].gameObject.SetActive(false);
                //Skill_buttons[1].onClick.AddListener(Skill2);                                                                                   //스킬1,2 버튼에 함수 연결. 밑에있음.
                for (int i = 0; i < 3; i++)
                {
                    if (myName == playerType[i]) 
                        Canvas.GetChild(0).GetChild(playernum).GetChild(3).GetComponent<Image>().sprite = player_Job_sprite[i];                 //플레이어 직업 이미지
                }
                Canvas.GetChild(0).GetChild(playernum).GetChild(4).GetComponent<Text>().text = myName;                                          //플레이어 이름 입력
                hp_bar2 = Canvas.GetChild(0).GetChild(playernum).GetChild(5).GetComponent<Slider>();                                            //플레이어 hp_bar 연결
                hp_now_text = hp_bar2.transform.GetChild(2).GetComponent<Text>();                                                               //플레이어 hp text 연결
                hp_now_text.text = hp.ToString() + "/" + fullhp.ToString();
                //배경, 적, 플레이어 세팅 전부 완료된 상태임
                if (playernum == 0) FightScene0_Manager.Player_Setting_over = true;

                for (int i = 0; i < 3; i++)
                {
                    if(transform.parent.GetChild(i).gameObject.activeSelf) player_list.Add(transform.parent.GetChild(i));           //플레이어리스트 추가(켜져있는 것만)
                }

                //Player_plus(myOwnNumber);
                break;
            }
            yield return null;
        }
        StartCoroutine(Button_inactive(Skill_buttons[0], 7f));
    }

    void Update()
    {
        if (!is_fight) return;

        if (hp <= 0) return;

        if (attack_target == null)
        {
            my_time = 0;
        }

        if (!skill_use)
        {
            my_time += Time.deltaTime;
            if (my_time > attack_time && anim_num != 1 && attack_target != null)
            {
                now_anim(2, 0, false, 1f);
                my_time = 0;
            }
        }

        if (target_select)      //타겟 자동 지정
        {
            target_selecting();
        }
    }

    public void hp_minus(float attack,float a)
    {
        if (hp <= 0) return;
        hp -= attack * defense;
        FightScene0_Manager.ShakingCoroutine(0.05f);
        if (hp < 0) hp = 0;
        hp_bar1.fillAmount = hp / fullhp;
        hp_bar2.value = hp / fullhp;
        hp_now_text.text = Mathf.RoundToInt(hp).ToString() + "/" + fullhp.ToString();
        Damage_Manager.use_number(transform.position + damage_text_pos, (int)attack, false,a);
        Damage_Manager.use_ani(transform.position,transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder +1);
        if (hp == 0)
        {
            StartCoroutine(character_die());
            Debug.Log(transform.name + " : " + player_list.Count);
            for (int i = 0; i < enemy_list.Count; i++)
            {
                enemy_list[i].GetComponent<EnemyManager0>().player_list.Remove(transform);
            }
            for (int i = 0; i < player_list.Count; i++)
            {
                Debug.Log(player_list[i] + " : " + player_list[i].GetComponent<PlayerManager0>());
                player_list[i].GetComponent<PlayerManager0>().player_list.Remove(transform);
            }
        }
        else
        {
            //if(anim_num < 2 || anim_num >4) 
            StartCoroutine(damaged_Color());
            if(is_fight) now_anim(1,0, false, 1f);
        }
    }
    IEnumerator character_die()
    {
        now_anim(6, -1, false, 1f);
        yield return new WaitForSeconds(anim[6].Animation.Duration);
        Skill_buttons[0].enabled = false;
        Skill_buttons[1].enabled = false;
        gameObject.SetActive(false);
    }
    public void hp_plus(float hill)
    {
        hp += hill;
        if (hp > fullhp) hp = fullhp;
        hp_bar1.fillAmount = hp / fullhp;
        hp_bar2.value = hp / fullhp;
    }

    void target_selecting()
    {
        if (skill_use) return;
        if (enemy_list.Count == 0) { attack_target = null; return; }
        attack_target = enemy_list[0];
        for (int i = 1; i < enemy_list.Count; i++)
        {
            if (Vector3.Distance(transform.position, attack_target.position) > Vector3.Distance(transform.position, enemy_list[i].position)) attack_target = enemy_list[i];
        }
        target_selected = true;
        
        if (Vector3.Distance(transform.position, attack_target.position) > distance)
        {
            transform.position = Vector3.MoveTowards(transform.position, attack_target.position, Time.deltaTime * player_speed); 
            my_time = 0f;
            if (!target_move) { now_anim(5, 0, true, 1f); target_move = true; }
        }
        else 
        {
            if (target_move) { now_anim(0, 0, true, 1f); target_move = false; }
        }
        if (attack_target.position.x - transform.position.x >= 0) transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
        else if (attack_target.position.x - transform.position.x < 0) transform.GetChild(0).rotation = Quaternion.Euler(0, 180, 0);
    }


    ////////////////////////////////////////////////////////////////////// <드래그로 플레이어 위치 정렬>
    Coroutine runCo = null;
    public void playermove(float a)
    {
        if (hp == 0) return;

        is_fight = false;
        //StopCoroutine("playermoving");                //1번 방법. 해당 string이름의 코루틴 모두 정지
        //StartCoroutine("playermoving",a);

        if (runCo != null) StopCoroutine(runCo);
        runCo = StartCoroutine(playermoving(a));        //2번 방법. 원하는 코루틴만 정지
    }

    IEnumerator playermoving(float a)
    {
        now_anim(5,0,true,1f);
        float time = 0;
        Vector3 pos = new Vector3(a, transform.position.y, transform.position.z);
        while (true)
        {
            time = Time.deltaTime;
            if (time > 2f) break;

            transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * player_speed);

            if (pos.x - transform.position.x >= 0) transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
            else if (pos.x - transform.position.x < 0) transform.GetChild(0).rotation = Quaternion.Euler(0, 180, 0);

            if (Vector3.Distance(transform.position, pos) < 0.1f) break;

            yield return null;
        }
        now_anim(0, 0, true, 1f);
        is_fight = true;
    }
    ////////////////////////////////////////////////////////////////////// </드래그로 플레이어 위치 정렬>

    ////////////////////////////////////////////////////////////////////// <스킬>
    /// <summary>
    /// by 지선, 1번 스킬 버튼을 누르면 호출
    /// </summary>
    public void Skill1() 
    {
        if (!is_fight || enemy_list.Count == 0 || attack_target == null || target_move) return;
        skill_use = true;
        now_anim(3,0, false, 1f);
        float zoomTime = 0f;
        switch (myOwnNumber)
        {
            case 0: //물도깨비
                zoomTime = 2f; break;
            case 1: //불도깨비
                zoomTime = 2f; break;
            case 2: //풀도깨비
                zoomTime = 1f; break;
        }
        FightScene0_Manager.zoomCoroutine(transform, zoomTime);
        StartCoroutine(Button_inactive(Skill_buttons[0], 7f));
    }

    public void Skill2() 
    {
        if (!is_fight || enemy_list.Count == 0 || attack_target == null || target_move) return;
        skill_use = true;

        FightScene0_Manager.zoomCoroutine(transform, 2f);

        switch (myOwnNumber)
        {
            case 0:                     //물도깨비
                Debug.Log("얘는 두번째 스킬이 없음");
                break;
            case 1:                     //불도깨비
                skill_use = true;
                now_anim(4, 0, false, 1f);
                StartCoroutine(FireDoggabi_Skill2());
                break;
            case 2:                     //풀도깨비
                Debug.Log("얘도 두번째 스킬이 없음");
                break;
        }
    }

    Transform FindObj(string tag_name)          //tag이름으로 물체 찾기 (ex)Firaball
    {
        return FindAtoZ.Find_tag(tag_name).transform;
    }

    void WaterDoggabi_baseAttack()
    {
        if(attack_target != null) attack_target.GetComponent<EnemyManager0>().hp_minus(attack, 0.5f);
    }

    IEnumerator FireDoggabi_baseAttack()
    {
        Transform fireball = FindObj(GlobalAtoZ.Tag_Attack_FIRE_Doggabi_Fireball0);
        if (attack_target != null) 
        { 
            fireball.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = attack_target.GetChild(0).GetComponent<MeshRenderer>().sortingOrder + 1;
            float a = attack_target.position.x - transform.position.x;
            if (a < 0) fireball.rotation = Quaternion.Euler(0, 180, 0);
            else fireball.rotation = Quaternion.Euler(0, 0, 0);
            fireball.position = new Vector3(transform.position.x, -1.8f, 0);
        }
        
        while (true)
        {
            if(attack_target == null) break;
            else
            {
                fireball.position = Vector3.MoveTowards(fireball.position, new Vector3(attack_target.position.x, -1.8f,0), Time.deltaTime * 10f);
                if (Vector3.Distance(fireball.position, new Vector3(attack_target.position.x, -1.8f, 0)) < 0.1f) break;
            }
            yield return null;
        }
        //Debug.Log("불덩이끝남");
        fireball.position = new Vector3(-16, 0, 0);
        if (attack_target != null) attack_target.GetComponent<EnemyManager0>().hp_minus(attack * 0.5f, 0.5f);
        my_time = 0;
    }
    IEnumerator GrassDoggabi_baseAttack()
    {
        Transform grassball = FindObj(GlobalAtoZ.Tag_Grass_Doggabi_ball0);
        if (attack_target != null) 
        {
            grassball.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = attack_target.GetChild(0).GetComponent<MeshRenderer>().sortingOrder + 1;
            float a = attack_target.position.x - transform.position.x;
            if (a < 0) grassball.rotation = Quaternion.Euler(0, 0, 0);
            else grassball.rotation = Quaternion.Euler(0, 180, 0);
            grassball.position = new Vector3(transform.position.x, -1.8f, 0);
        } 
        while (true)
        {
            if (attack_target == null) break;
            else
            {
                grassball.position = Vector3.MoveTowards(grassball.position, new Vector3(attack_target.position.x, -1.8f, 0), Time.deltaTime * 10f);
                if (Vector3.Distance(grassball.position, new Vector3(attack_target.position.x, -1.8f, 0)) < 0.1f) break;
            }
            yield return null;
        }
        grassball.position = new Vector3(-16, 0, 0);
        if (attack_target != null) attack_target.GetComponent<EnemyManager0>().hp_minus(attack * 0.5f, 0.5f);
        my_time = 0;
    }

    IEnumerator WaterDoggabi_Skill1()           //물도깨비 스킬 1 - 도발, 방어력 증가
    {
        for (int i = 0; i < enemy_list.Count; i++)
        {
            enemy_list[i].GetComponent<EnemyManager0>().target_select = false;
            enemy_list[i].GetComponent<EnemyManager0>().attack_target = transform;
        }
        float def_save = defense;
        defense = defense * 0.7f;                      //방어력 증가 수치는 미정인데, 임의로 넣었음.
        BaseAttack_active();

        GameObject shield = transform.GetChild(2).gameObject;

        shield.SetActive(true);

        yield return new WaitForSeconds(4.5f);

        shield.gameObject.SetActive(false);
        defense = def_save;
    }

    IEnumerator FireDoggabi_Skill1()            //불도깨비 스킬 1 - 직선 공격 + 5초간 지속 데미지
    {
        Transform fireball = FindObj(GlobalAtoZ.Tag_Attack_FIRE_Doggabi_Fireball1);
        fireball.position = new Vector3(transform.position.x,-1.8f,0);
        float a = attack_target.position.x - transform.position.x;
        if (a < 0) fireball.rotation = Quaternion.Euler(0, 180, 0);
        else fireball.rotation = Quaternion.Euler(0, 0, 0);
        while (true)
        {
            fireball.position = Vector3.MoveTowards(fireball.position, fireball.position + new Vector3(a, 0, 0).normalized, Time.deltaTime * 10f);
            for (int i = 0; i < enemy_list.Count; i++)
            {
                if (Mathf.Abs(fireball.position.x - enemy_list[i].position.x) < 0.3f) { enemy_list[i].GetComponent<EnemyManager0>().hp_minus_continue(attack, 5, 1f);}
            }
            if (fireball.position.x < -16f || fireball.position.x > 16f) break;
            
            yield return null;
        }
        BaseAttack_active();
    }
    IEnumerator FireDoggabi_Skill2()            //불도깨비 스킬 2 - 전체 공격 + 지속데미지
    {
        StartCoroutine(Button_inactive(Skill_buttons[1], 7f));
        float speed = 0.1f;
        while (true)
        {
            speed += Time.deltaTime;
            int count = 0;
            for (int i = 0; i < enemy_list.Count; i++)
            {
                if (Vector3.Distance(enemy_list[i].position, transform.position) > 2f)
                {
                    enemy_list[i].position = Vector3.MoveTowards(enemy_list[i].position, transform.position, Time.deltaTime * speed * speed);
                }
                else count++;
            }
            if (count == enemy_list.Count) break;
            yield return null;
        }
        for (int i = 0; i < enemy_list.Count; i++)
        {
            enemy_list[i].GetComponent<EnemyManager0>().hp_minus_continue(attack * 1.5f, 5, 1f);
        }
        yield return new WaitForSeconds(5f);
        BaseAttack_active();
    }

    IEnumerator GrassDoggabi_Skill1()            //풀도깨비 스킬 1 - 힐(일단은 hp가 가장 많이 줄어든 사람으로 자동 타겟임.)
    {
        //Time.timeScale = 0.05f;

        GameObject hill = hill_target.GetChild(hill_target.childCount-1).gameObject;

        for (int i = 0; i < player_list.Count; i++)
        {
            if (player_list[i].gameObject.activeSelf) 
            {
                if (hill_target == null) { hill_target = player_list[i]; Debug.Log(player_list[i].name); }
                else
                {
                    if(hill_target.GetComponent<PlayerManager0>().hp_bar1.fillAmount > player_list[i].GetComponent<PlayerManager0>().hp_bar1.fillAmount) hill_target = player_list[i];
                }
            }
        }
        hill.GetComponent<SpriteRenderer>().sortingOrder = hill_target.GetChild(0).GetComponent<MeshRenderer>().sortingOrder + 1;
        
        hill.transform.position = hill_target.position + Vector3.down * 1.28f;
        hill.transform.SetParent(hill_target);
        hill.SetActive(true);
        hill_target.GetComponent<PlayerManager0>().hp_plus(hill_target.GetComponent<PlayerManager0>().fullhp * 0.3f);
        yield return new WaitForSeconds(0.3f);
        FightScene0_Manager.zoomCoroutine(hill_target, 1f);
        BaseAttack_active();

        //Time.timeScale = 1f;
    }

    private void myAniKey(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "f_atk00")  //불도깨비 기본 공격 타이밍
            StartCoroutine(FireDoggabi_baseAttack());
        else if (e.Data.Name == "f_atk01") //불도깨비 스킬1 불덩이 타이밍
            StartCoroutine(FireDoggabi_Skill1());
        else if(e.Data.Name == "w_atk00") //물도깨비 기본공격.
            WaterDoggabi_baseAttack();          
        else if (e.Data.Name == "w_atk01") //물도깨비 스킬1 물실드 타이밍.
            StartCoroutine(WaterDoggabi_Skill1()); 
        else if (e.Data.Name == "g_atk00") //풀도깨비 기본공격 타이밍.
            StartCoroutine(GrassDoggabi_baseAttack()); 
        else if (e.Data.Name == "g_atk01") //풀도깨비 스킬1 힐 타이밍.
            StartCoroutine(GrassDoggabi_Skill1()); 
    }

    /// <summary>
    /// by지선, 버튼 비활성화 + 쿨타임
    /// </summary>
    /// <param name="button"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator Button_inactive(Button button, float time) //일정 시간동안 버튼을 사용할 수 없게 함.
    {
        float cooltime = 0;
        button.interactable = false; //버튼 비활성화
        button.transform.GetChild(0).GetComponent<Image>().enabled = true; //쿨타임 이미지
        while (true)
        {
            cooltime += Time.deltaTime;
            button.transform.GetChild(0).GetComponent<Image>().fillAmount = cooltime / time; //쿨타임 이미지 fillamount 증가
            if (cooltime > time)
            {
                button.interactable = true; //쿨타임 다 지나서 버튼 활성화
                button.transform.GetChild(0).GetComponent<Image>().fillAmount = 0; //fillamount가 1이 되는 순간 0으로 만들어준 후,
                button.transform.GetChild(0).GetComponent<Image>().enabled = false; //쿨타임 이미지 꺼줌
                break;
            }
            yield return null;
        }
    }
    void BaseAttack_active()                                                            //기본 공격이 가능해지는 타이밍.
    {
        skill_use = false;
        my_time = 0;
    }
    ////////////////////////////////////////////////////////////////////// </스킬>
}
