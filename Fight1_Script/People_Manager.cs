using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class People_Manager : MonoBehaviour
{
    FightScene1_Manager FightScene1_Manager;
    SkeletonAnimation my_anim;
    AnimationReferenceAsset[] anim = new AnimationReferenceAsset[5];
    public int now_animnum;
    public bool is_inactive = true;

    void Start()
    {
        FightScene1_Manager = GameObject.FindObjectOfType<FightScene1_Manager>();
        my_anim = transform.GetComponent<SkeletonAnimation>();

        anim[0] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_People + "/chr_student_run");                //마을사람 뛰는거
        anim[1] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_People + "/chr_student_idle");               //마을사람 가만히 서있는거
        anim[2] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_People + "/chr_student_die_before");         //마을사람 죽는거
        anim[3] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_People + "/chr_student_die_after");          //마을사람 시체
        anim[4] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_People + "/chr_student_surprised");          //마을사람 놀람

        People_now_anim(1, true, 1f);
    }

    private void Update()
    {
        if (now_animnum == 1) is_inactive = true;
        else is_inactive = false;
    }

    public void People_now_anim(int a, bool loop, float timeScale)
    {
        if (FightScene1_Manager.die) return;

        if(now_animnum == 3)
        {
            if (a != 1) return;
        }
        now_animnum = a;
        
        bool seven_active = false;
        if(a == 7)
        {
            seven_active = true;
            a = 2;
        }

        my_anim.state.SetAnimation(0, anim[a], loop);
        my_anim.loop = loop;
        my_anim.timeScale = timeScale;

        if (a == 2)
        {
            if(!seven_active) StartCoroutine(person_dead2());
            StartCoroutine(person_dead1());
        }
        else if (a == 4 && !loop)
        {
            StartCoroutine(person_run());
        }
    }
    IEnumerator person_dead1()
    {
        yield return new WaitForSeconds(anim[2].Animation.Duration);
        People_now_anim(3, false, 1f);
    }
    IEnumerator person_dead2()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left, Time.deltaTime * 5f);
            yield return null;
            if (transform.position.x < -13f)
            {
                transform.position = new Vector3(11, -2.6f, 0);
                now_animnum = 1;
                People_now_anim(1, true, 1f);
                break;
            }
        }
        FightScene1_Manager.people_list.Remove(transform);
    }
    IEnumerator person_run()
    {
        yield return new WaitForSeconds(anim[4].Animation.Duration);
        People_now_anim(0, true, 1f);
    }
}
