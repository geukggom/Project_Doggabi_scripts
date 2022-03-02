using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PlayerStat : MonoBehaviour
{
    public string[] playerType;
    public string myType;
    public string myName;
    public int myOwnNumber;
    public float attack;
    public float defense;
    public float hill;
    public float distance;
    public float fullhp;
    public Vector3 damage_text_pos;


    //캐릭터 애니메이션//
    public SkeletonAnimation myanimation;
    public AnimationReferenceAsset[] anim;
    public int anim_num;

    private void Awake()
    {
        myanimation = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        playerType = new string[3];
        playerType[0] = "Tanker";
        playerType[1] = "Dealer";
        playerType[2] = "Hiller";
    }

    public void Player_Stat(int a)
    {
        switch (a)
        {
            case 0:                     //물도깨비
                myType = playerType[0];
                myName = "물 도깨비";
                myOwnNumber = 0;
                attack = 80;
                defense = 0.9f;
                hill = 0;
                distance = 1.8f;
                fullhp = 800;

                GameObject watershield_prefab = Resources.Load<GameObject>(GlobalAtoZ.Prefab_Watershield);
                GameObject shield = Instantiate(watershield_prefab, transform.position + Vector3.down * 1.7f, Quaternion.identity);
                shield.transform.SetParent(transform);
                shield.GetComponent<SpriteRenderer>().sortingOrder = transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder + 1;
                FindAtoZ.Find_tag(GlobalAtoZ.Tag_WATER_Doggabi_Shield).SetActive(false);

                anim = new AnimationReferenceAsset[9];
                anim[0] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_WaterDoggabi + "/chr_water_fight_idle");     //싸움 준비자세
                anim[1] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_WaterDoggabi + "/chr_water_fight_dmg");      //공격받음
                anim[2] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_WaterDoggabi + "/chr_water_fight_atk");      //기본 공격
                anim[3] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_WaterDoggabi + "/chr_water_fight_skill01");  //스킬1
                anim[4] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_WaterDoggabi + "/chr_water_fight_skill01");  //스킬2 --------- 물도깨비는 스킬2 안 씀.
                anim[5] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_WaterDoggabi + "/chr_water_fight_run");      //뛰는거
                anim[6] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_WaterDoggabi + "/chr_water_fight_die");      //죽는거
                anim[7] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_WaterDoggabi + "/chr_water_fight_win");      //이기는거
                anim[8] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_WaterDoggabi + "/chr_water_fight_idle");     //가만히 서있는거.----아직 없음.
                break;
            case 1:                     //불도깨비
                myType = playerType[1];
                myName = "불 도깨비";
                myOwnNumber = 1;
                attack = 120;
                defense = 1f;
                hill = 0;
                distance = 5;
                fullhp = 600;

                GameObject fireball_prefab0 = Resources.Load<GameObject>(GlobalAtoZ.Prefab_Fireball0);
                GameObject fireball_prefab1 = Resources.Load<GameObject>(GlobalAtoZ.Prefab_Fireball1);
                Instantiate(fireball_prefab0, new Vector3(-13f, 0, 0), Quaternion.identity);
                Instantiate(fireball_prefab1, new Vector3(-13f, 0, 0), Quaternion.identity);   //불도깨비는 스킬에 쓸 불덩어리를 만듬 - 스킬1 불덩이

                anim = new AnimationReferenceAsset[9];
                anim[0] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_FireDoggabi + "/chr_fire_fight_idle");       //싸움 준비자세
                anim[1] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_FireDoggabi + "/chr_fire_fight_dmg");        //공격받음
                anim[2] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_FireDoggabi + "/chr_fire_fight_atk");        //기본 공격
                anim[3] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_FireDoggabi + "/chr_fire_fight_skill01");    //스킬1
                anim[4] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_FireDoggabi + "/chr_fire_fight_skill01");    //스킬2 --------- 아직 없어서 일단 스킬1넣어둠
                anim[5] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_FireDoggabi + "/chr_fire_fight_run");        //뛰는거
                anim[6] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_FireDoggabi + "/chr_fire_fight_die");        //죽는거
                anim[7] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_FireDoggabi + "/chr_fire_fight_win");        //이기는거
                anim[8] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_FireDoggabi + "/chr_fire_idle");             //가만히 서있는거.
                
                break;
            case 2:                     //풀도깨비
                myType = playerType[2];
                myName = "풀 도깨비";
                myOwnNumber = 2;
                attack = 50;
                defense = 1.1f;
                hill = 100;
                distance = 7;
                fullhp = 500;

                GameObject grasshill_prefab = Resources.Load<GameObject>(GlobalAtoZ.Prefab_Grasshill);
                GameObject grassball_prefab = Resources.Load<GameObject>(GlobalAtoZ.Prefab_GrassBall0);
                Instantiate(grasshill_prefab, new Vector3(-13f, 0, 0), Quaternion.identity).transform.SetParent(transform);
                Instantiate(grassball_prefab, new Vector3(-13f, 0, 0), Quaternion.identity);

                anim = new AnimationReferenceAsset[9];
                anim[0] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_GrassDoggabi + "/chr_grass_fight_idle");         //싸움 준비자세
                anim[1] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_GrassDoggabi + "/chr_grass_fight_dmg");          //공격받음
                anim[2] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_GrassDoggabi + "/chr_grass_fight_atk");          //기본 공격
                anim[3] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_GrassDoggabi + "/chr_grass_fight_skill01");      //스킬1
                anim[4] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_GrassDoggabi + "/chr_grass_fight_skill01");      //스킬2 --------- 풀도깨비는 스킬2 안 씀.
                anim[5] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_GrassDoggabi + "/chr_grass_fight_run");          //뛰는거
                anim[6] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_GrassDoggabi + "/chr_grass_fight_die");          //죽는거
                anim[7] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_GrassDoggabi + "/chr_grass_fight_win");          //이기는거
                anim[8] = Resources.Load<AnimationReferenceAsset>(GlobalAtoZ.Animation_GrassDoggabi + "/chr_grass_fight_idle");         //가만히 서있는거.----아직 없음.
                break;
        }
    }

    public void now_anim(int a,int b, bool loop, float timeScale)
    {
        if (anim_num == 6) return;
        if (anim_num >= 2 && anim_num <= 4 && a == 1) return;

        StopCoroutine("anim_idle");

        if (anim_num == a && anim_num!= 1) return;
        //Debug.Log(myOwnNumber +" : "+a);
        anim_num = a;
        myanimation.state.SetAnimation(0, anim[a], loop);
        myanimation.loop = loop;
        myanimation.timeScale = timeScale;
        if(!loop) StartCoroutine("anim_idle",b);
    }
    IEnumerator anim_idle(int aninum)
    {
        float a = anim[anim_num].Animation.Duration;
        if (aninum >= 0)
        {
            yield return new WaitForSeconds(a);
            if(transform.GetComponent<PlayerManager0>().is_fight) now_anim(aninum, 0, true, 1f);
            else now_anim(5, 0, true, 1f);      //드래그했을때 공격받으면 받은 후에 다시 뛰게
        }
    }

    public IEnumerator damaged_Color()
    {
        for (int i = 0; i < 3; i++)
        {
            myanimation.skeleton.SetColor(new Color(1f, 0.7f, 0.7f));
            yield return new WaitForSeconds(0.1f);
            myanimation.skeleton.SetColor(Color.white);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
