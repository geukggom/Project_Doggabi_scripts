using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class EnemyStat : MonoBehaviour
{
    public float attack;
    public float hill;
    public float distance;
    public float fullhp;
    public Vector3 damage_text_pos;
    public SkeletonAnimation myAni;
    public AnimationReferenceAsset[] anim;


    public void Enemy_Stat(int a)
    {
        myAni = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        anim = new AnimationReferenceAsset[6];
        switch (a)
        {
            case 0:
                attack = 40;
                hill = 0;
                distance = 2f;
                fullhp = 1000;
                damage_text_pos = Vector3.up * 2.5f;

                anim[0] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Boar + "/mon_boar_idle");    //가만 있는거
                anim[1] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Boar + "/mon_boar_run");     //달리는거
                anim[2] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Boar + "/mon_boar_atk");     //공격
                anim[3] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Boar + "/mon_boar_dmg");     //공격받음
                anim[4] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Boar + "/mon_boar_win");     //이김
                anim[5] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Boar + "/mon_boar_die");     //죽음
                break;
            case 1:                     //늑대
                attack = 35;
                hill = 0;
                distance = 2;
                fullhp = 600;
                damage_text_pos = Vector3.up * -0.5f;

                anim[0] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_idle");        //가만 있는거
                anim[1] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_run");        //달리는거
                anim[2] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_atk");         //공격
                anim[3] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_dmg");        //공격받음
                anim[4] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_win");        //이김
                anim[5] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_die");        //죽음
                break;
            case 2:                     //보스늑대
                attack = 60;
                hill = 0;
                distance = 4.5f;
                fullhp = 2000;
                damage_text_pos = Vector3.up * 0.5f;
                myAni.skeleton.SetSkin("boss");
                transform.GetChild(0).localScale = new Vector2(0.25f, 0.25f);
                transform.GetChild(1).position += Vector3.up;

                anim[0] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_idle");        //가만 있는거
                anim[1] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_run");        //달리는거
                anim[2] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_atk");         //공격
                anim[3] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_dmg");        //공격받음
                anim[4] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_win");        //이김
                anim[5] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_Wolf + "/wolf_die");        //죽음
                break;
        }
    }

    public int anim_num;
    public void now_anim(int a, int b, bool loop, float timeScale)
    {
        if (anim_num == 5) return;
        StopCoroutine("anim_idle");

        anim_num = a;
        myAni.state.SetAnimation(0, anim[a], loop);
        myAni.loop = loop;
        myAni.timeScale = timeScale;
        if (!loop) StartCoroutine("anim_idle", b);
    }
    IEnumerator anim_idle(int b)
    {
        if(b >= 0)
        {
            float a = anim[b].Animation.Duration;
            yield return new WaitForSeconds(a);
            now_anim(b, 0, true, 1f);
        }
    }
}
