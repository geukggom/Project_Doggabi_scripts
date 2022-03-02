using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using System;
using UnityEngine.UI;

public class FightScene1_Manager : MonoBehaviour
{
    FireSkill1_Manager FireSkill1_Manager;
    FireSkill2_Manager FireSkill2_Manager;

    public GameObject DuOkSiNi;
    SkeletonAnimation Duok_Animation;
    public GameObject People_parent;
    public AnimationReferenceAsset[] anim;
    public float skill_time = 0;
    public bool is_skill = false;

    public GameObject fireball0;
    public List<Transform> people_list;
    public Transform Duok_skill1_parent;
    Button skill1;
    Button skill2;

    Transform DUOK_AttackRange;
    bool is_skill1 = false;
    float skill_range_x;

    public Transform[] bg = new Transform[3];

    void Start()
    {
        FireSkill1_Manager = GameObject.FindObjectOfType<FireSkill1_Manager>();
        FireSkill2_Manager = GameObject.FindObjectOfType<FireSkill2_Manager>();
        DUOK_AttackRange = FindAtoZ.Find_tag(GlobalAtoZ.Tag_DUOK_AttackRange).transform;
        //fireball0 = Instantiate(Resources.Load<GameObject>(GlobalAtoZ.Prefab_Fireball0));
        fireball0.transform.position = new Vector3(11, 0, 0);
        Duok_skill1_parent = FindAtoZ.Find_tag(GlobalAtoZ.Tag_DuOkSiNi_FireSkill1).transform;
        skill1 = FindAtoZ.Find_tag(GlobalAtoZ.Tag_Canvas_FightScene).transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>();
        skill1.onClick.AddListener(Duok_skill1);
        skill2 = skill1.transform.parent.GetChild(2).GetComponent<Button>();                    //skill2는 아직 연결 안 함
        skill2.onClick.AddListener(Duok_skill2);
        StartCoroutine(skill_cooltime(skill1, 0f));
        StartCoroutine(skill_cooltime(skill2, 0f));             ///////////////////////////////////////////////////////////

        Duok_Animation = DuOkSiNi.transform.GetChild(0).GetComponent<SkeletonAnimation>();
        Duok_Animation.state.Event += Duok_event;
        anim = new AnimationReferenceAsset[5];
        anim[0] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Duoksini + "/chr_f_re_idle");       //뛰는거
        anim[1] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Duoksini + "/chr_f_re_atk");        //기본공격
        anim[2] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Duoksini + "/chr_f_re_skill_1");    //스킬1
        anim[3] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Duoksini + "/chr_f_re_skill_2");    //스킬2
        anim[4] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Duoksini + "/chr_f_re_idle");       //싸움 준비자세

        StartCoroutine("Run");
    }

    IEnumerator Run()
    {
        Duok_now_anim(0, 0, true, 1f);
        while (true)                                                    //두억시니 화면에 나타나서 멈출때까지
        {
            yield return null;
            DuOkSiNi.transform.position = Vector3.MoveTowards(DuOkSiNi.transform.position, DuOkSiNi.transform.position+Vector3.right, Time.deltaTime * 5f);
            if (DuOkSiNi.transform.position.x > -5f) break;
        }
        StartCoroutine(bg_move());
        yield return new WaitForSeconds(1f);

        is_skill = true;
        int parent_childcount = 0;
        while (true)                                                    //사람들 5명씩 순차적으로 화면으로 보냄
        {
            if(people_list.Count < 5)
            {
                float a = UnityEngine.Random.Range(0.5f, 2f);
                StartCoroutine("person_move", People_parent.transform.GetChild(parent_childcount));
                People_parent.transform.GetChild(parent_childcount).GetComponent<People_Manager>().is_inactive = false;
                people_list.Add(People_parent.transform.GetChild(parent_childcount));
                parent_childcount++;
                if (parent_childcount == People_parent.transform.childCount) parent_childcount = 0;
                yield return new WaitForSeconds(a);
            }
            yield return null;
        }
    }

    IEnumerator person_move(Transform person)           //사람들 화면에 나타났다가 달릴때까지
    {
        float speed = 5f;
        float x = UnityEngine.Random.Range(0f,8f);
        while (true)
        {
            person.position = Vector3.MoveTowards(person.position, person.position + Vector3.left, Time.deltaTime * speed);
            if (person.position.x < x) break;
            yield return null;
        }
        person.GetComponent<People_Manager>().People_now_anim(4, false, 1f);
    }

    int now_Duok_ani;
    void Duok_now_anim(int a, int b, bool loop, float timeScale)       //두억시니 애니메이션 관리
    {
        now_Duok_ani = a;
        Duok_Animation.state.SetAnimation(0, anim[a], loop);
        Duok_Animation.loop = loop;
        Duok_Animation.timeScale = timeScale;
        if (!loop) StartCoroutine("Duok_anim_idle", b);
    }
    IEnumerator Duok_anim_idle(int b)
    {
        yield return new WaitForSeconds(anim[now_Duok_ani].Animation.Duration);
        Duok_now_anim(0, 0, true, 1f);
    }

    

    void Update()
    {
        if (is_skill1)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            DUOK_AttackRange.position = new Vector3(pos.x, 0, 0);
            if (Input.GetMouseButtonDown(0)) 
            {
                is_skill1 = false;
                DUOK_AttackRange.position = new Vector3(0, 10, 0);
                Time.timeScale = 1f;
                skill_range_x = pos.x;
            }
        }

        if (is_skill && people_list.Count != 0 && !is_skill1) 
        { 
            skill_time += Time.deltaTime;
            if (skill_time > 5f) 
            {
                is_skill = false;
                skill_time = 0;

                Duok_now_anim(1, 0, false, 1f); 
            }
        }
    }
    private void Duok_event(TrackEntry trackEntry, Spine.Event e)
    {
        //Debug.Log(e.Data.Name);
        if (e.Data.Name == "f_r_atk")               //두억시니 기본공격.
        {
            StartCoroutine(Duok_baseattack());
        }
        else if (e.Data.Name == "f_r_skill_1")      //두억시니 스킬1
        {
            if (is_skill1)
            {
                is_skill1 = false;
                Time.timeScale = 1f;
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                skill_range_x = pos.x;
                DUOK_AttackRange.position = Vector3.up * 10f;
            }
            FireSkill1_Manager.duok_skill_use(skill_range_x);
            StartCoroutine(skill_cooltime(skill1, 10f));
            StartCoroutine(Duok_skill1_Coroutine());
        }
        else if(e.Data.Name == "f_r_skill_1_dmg")
        {
            Debug.Log("쥬굼");
            
        }
        else if (e.Data.Name == "f_r_skill_2_1")    //두억시니 스킬2 끌어오기
        {
            bg_stop = true;
            peopleDrag();
        }
        else if (e.Data.Name == "f_r_skill_2_2")    //두억시니 스킬2 태우기
        {
            FireSkill2_Manager.skill_use();
            StartCoroutine(Finish());
        }
    }

    public GameObject Finish_panel;
    IEnumerator Finish()
    {
        yield return new WaitForSeconds(anim[3].Animation.Duration);
        yield return new WaitForSeconds(3f);
        Finish_panel.SetActive(true);
    }

    IEnumerator Duok_baseattack()
    {
        if (people_list.Count != 0)
        {
            Transform target = people_list[0];
            for (int i = 1; i < people_list.Count; i++)
            {
                if (target.position.x > people_list[i].position.x) target = people_list[i];         //불덩이 타겟 설정
            }
            fireball0.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<MeshRenderer>().sortingOrder + 1;
            fireball0.transform.position = DuOkSiNi.transform.position + Vector3.down * 2f;     //불덩이 초기 위치
            while (true)
            {
                fireball0.transform.position = Vector3.MoveTowards(fireball0.transform.position, fireball0.transform.position + Vector3.right, Time.deltaTime * 8f);
                if(fireball0.transform.position.x >= target.transform.position.x - 1f)
                {
                    fireball0.transform.position = new Vector3(11, 0, 0);
                    target.GetComponent<People_Manager>().People_now_anim(2, false,1f);
                    //People_now_anim(target.GetComponent<SkeletonAnimation>(), 7, 6, false, 1f);
                    break;
                }
                yield return null;
            }
        }
        is_skill = true;
        skill_time = 0;
        yield return null;
    }
    void Duok_skill1()
    {
        if (now_Duok_ani == 1) return;
        skill_time = 0;
        skill1.interactable = false;
        Time.timeScale = 0.2f;
        DUOK_AttackRange.position = Vector3.zero;
        is_skill1 = true;
        Duok_now_anim(2, 0, false, 1f);
    }
    /// <summary>
    /// 
    /// </summary>
    public IEnumerator Duok_skill1_Coroutine()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < people_list.Count; i++)
        {
            float x = people_list[i].position.x;
            if (x > skill_range_x - 2.75f && x < skill_range_x + 2.75f)
            people_list[i].GetComponent<People_Manager>().People_now_anim(2, false, 1f);
        }
    }

    void Duok_skill2()
    {
        if (now_Duok_ani == 1) return;
        StopCoroutine("Run");
        is_skill = false;
        skill_time = 0;
        skill2.interactable = false;
        Duok_now_anim(3, 0, false, 1f);
    }
    void peopleDrag()
    {
        
        List<Transform> new_plist = new List<Transform>();
        for (int i = 0; i < people_list.Count; i++)
        {
            if (people_list[i].GetComponent<People_Manager>().now_animnum != 2 || people_list[i].GetComponent<People_Manager>().now_animnum != 3) new_plist.Add(people_list[i]);
        }

        for (int i = 0; i < People_parent.transform.childCount; i++)
        {
            if (People_parent.transform.GetChild(i).GetComponent<People_Manager>().is_inactive) 
            { 
                new_plist.Add(People_parent.transform.GetChild(i));
                People_parent.transform.GetChild(i).position = new Vector3(11 + UnityEngine.Random.Range(0f,3f), -2.6f, 0);
            }
            if (new_plist.Count > 5) i = People_parent.transform.childCount - 1;
        }

        for (int i = 0; i < new_plist.Count; i++)
        {
            new_plist[i].GetComponent<People_Manager>().People_now_anim(4, true, 1f);
            StartCoroutine(drag_person(new_plist[i]));
        }
    }
    IEnumerator drag_person(Transform a)
    {
        float mytime = 2;
        Vector3 b = new Vector3(UnityEngine.Random.Range(-2f, 0f), -2.6f, 0);
        while (true)
        {
            mytime += Time.deltaTime * 3f;
            a.position = Vector3.MoveTowards(a.position, b, Time.deltaTime * mytime * mytime);
            if (a.position == b) break;
            yield return null;
        }
        a.GetComponent<People_Manager>().People_now_anim(7, false, 1f);
    }

    IEnumerator skill_cooltime(Button skillbutton, float time)
    {
        skillbutton.interactable = false;
        skillbutton.transform.GetChild(0).GetComponent<Image>().enabled = true;
        float cooltime = 0;
        while (true)
        {
            cooltime += Time.deltaTime;
            skillbutton.transform.GetChild(0).GetComponent<Image>().fillAmount = cooltime / time;
            yield return null;
            if(cooltime > time)
            {
                skillbutton.transform.GetChild(0).GetComponent<Image>().fillAmount = 0;
                skillbutton.transform.GetChild(0).GetComponent<Image>().enabled = false;
                skillbutton.interactable = true;
                break;
            }
        }
    }

    bool bg_stop = false;
    IEnumerator bg_move()
    {
        while (true)
        {
            for (int i = 0; i < bg.Length; i++)
            {
                bg[i].position = Vector3.MoveTowards(bg[i].position, bg[i].position+Vector3.left, Time.deltaTime * 5f);
                if(bg[i].position.x<-19.2f) bg[i].position += Vector3.right * 57.6f; 
            }
            yield return null;
            if (bg_stop) break;
        }
    }

    public bool die = false;
    public void people_all_die()
    {
        die = true;
        for (int i = 0; i < people_list.Count; i++)
        {
            people_list[i].GetComponent<People_Manager>().People_now_anim(2,false,1f);
        }
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
