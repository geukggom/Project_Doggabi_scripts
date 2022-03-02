using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyManager0 : EnemyStat
{
    Damage_Manager Damage_Manager;
    FightScene0_Manager FightScene0_Manager;
    public float hp;
    public int EnemyNum = -1;
    Image hp_bar;

    public bool is_fight = false;           //적이 화면에 등장 후, 전투 시작
    public bool target_select = true;       //자동으로 적 지정하는지(아니면 도발로 지정)

    public Transform attack_target;
    public Transform hill_target;

    public List<Transform> player_list;
    public List<Transform> enemy_list = new List<Transform>();

    float attack_time = 2f;                 //기본공격을 2초에 한 번 하도록.
    float my_time = 0f;
    bool continue_attack = true;
    float speed = 5f;

    void Start()
    {
        Damage_Manager = GameObject.FindObjectOfType<Damage_Manager>();
        FightScene0_Manager = GameObject.FindObjectOfType<FightScene0_Manager>();
        hp_bar = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
        StartCoroutine(SetStat());
    }
    IEnumerator SetStat()
    {
        while (true)
        {
            if(EnemyNum >= 0)
            {
                base.Enemy_Stat(EnemyNum);
                hp = fullhp;
                
                break;
            }
            yield return null;
        }
    }

    bool player_zero = true;
    void Update()
    {
        if (!is_fight) return;

        if (hp <= 0) return;

        if(player_list.Count == 0 && player_zero)
        {
            player_zero = false;
            now_anim(4,0,true,1f);
            StartCoroutine(Enemy_WinPanel());
        }

        my_time += Time.deltaTime;
        if(my_time > attack_time && attack_target != null)
        {
            my_time = 0;
            now_anim(2,0,false,1f);
            attack_target.GetComponent<PlayerManager0>().hp_minus(attack * 0.5f,0.5f);
        }
        if (target_select)          //타겟 자동 지정(가까운 적)
        {
            target_selecting();
        }
    }

    public void hp_minus(float attack, float a)
    {
        if (hp <= 0) return;
        hp -= attack;
        FightScene0_Manager.ShakingCoroutine(0.05f);
        if (hp > 0) now_anim(3, 0, false, 1f);
        hp_bar.fillAmount = hp / fullhp;
        Damage_Manager.use_number(transform.position + damage_text_pos, (int)attack, true,a);
        Damage_Manager.use_ani(transform.position, transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder + 1);
        if (hp <= 0)
        {
            StartCoroutine(enemy_die());
            for (int i = 0; i < player_list.Count; i++)
            {
                player_list[i].GetComponent<PlayerManager0>().enemy_list.Remove(transform);
            }
            for (int i = 0; i < enemy_list.Count; i++)
            {
                enemy_list[i].GetComponent<EnemyManager0>().enemy_list.Remove(transform);
            }
            //Debug.Log("여기임");
        }
    }
    IEnumerator enemy_die()
    {
        now_anim(5,-1,false,1f);
        yield return new WaitForSeconds(anim[5].Animation.Duration);
        gameObject.SetActive(false);
        FightScene0_Manager.is_fight = true;
    }
    public void hp_plus(float hill)
    {
        hp += hill;
        if (hp > fullhp) hp = fullhp;
        hp_bar.fillAmount = hp / fullhp;
    }

    public void hp_minus_continue(float attack, int Count, float times)
    {
        if(continue_attack) StartCoroutine(hp_minus_continue_Couroutine(attack, Count, times));
    }
    IEnumerator hp_minus_continue_Couroutine(float attack, int Count, float times)
    {
        continue_attack = false;                                                                        //코루틴이 연속으로 여러번 돌지 않게.
        for (int i = 0; i < Count; i++)
        {
            if (i == 0) hp_minus(attack,0.5f);
            else hp_minus(attack * 0.3f,0.3f);
            yield return new WaitForSeconds(times);
        }
        continue_attack = true;
    }

    bool target_mulda = false;
    void target_selecting()
    {
        if (player_list.Count == 0) { attack_target = null; return; }

        attack_target = player_list[0];
        for (int i = 1; i < player_list.Count; i++)
        {
            if (Vector3.Distance(transform.position, attack_target.position) > Vector3.Distance(transform.position, player_list[i].position)) { attack_target = player_list[i];}
        }
        if (Vector3.Distance(transform.position, attack_target.position) > distance)                     //타겟이랑 거리가 멀면 달림.
        { 
            transform.position = Vector3.MoveTowards(transform.position, attack_target.position, Time.deltaTime * speed); my_time = 0f;

            if (target_mulda) 
            { 
                now_anim(1, 0, true, 1f);
                target_mulda = false;
            }
        }
        else        //타겟이랑 가까워져서 멈춤.
        {
            if (!target_mulda)
            {
                now_anim(0, 0, true, 1f);
                target_mulda = true;
            }
        }

        if (attack_target.position.x - transform.position.x < 0) 
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.GetChild(1).GetChild(0).rotation = Quaternion.Euler(0, 0, 0);     //hp바 고정
            transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (attack_target.position.x - transform.position.x >= 0) 
        { 
            transform.rotation = Quaternion.Euler(0, 180, 0);
            transform.GetChild(1).GetChild(0).rotation = Quaternion.Euler(0, 0, 0);     //고정
            transform.GetChild(0).rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    IEnumerator Enemy_WinPanel()
    {
        GameObject losepanel = FindAtoZ.Find_tag(GlobalAtoZ.Tag_Canvas_FightScene).transform.GetChild(4).gameObject;
        if (!losepanel.activeSelf)
        { 
            losepanel.SetActive(true); 
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene(1);
        }
        yield return null;
    }
}
