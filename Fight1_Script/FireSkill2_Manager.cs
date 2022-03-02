using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class FireSkill2_Manager : MonoBehaviour
{
    FightScene1_Manager FightScene1_Manager;
    public SkeletonAnimation myanim;
    public AnimationReferenceAsset atk;

    private void Start()
    {
        FightScene1_Manager = GameObject.FindObjectOfType<FightScene1_Manager>();
    }
    public void skill_use()
    {
        myanim.state.SetAnimation(0, atk, false);
        myanim.loop = false;
        myanim.timeScale = 1f;
        StartCoroutine(skilltime());
        transform.position = Vector3.zero;
    }
    IEnumerator skilltime()
    {
        yield return new WaitForSeconds(atk.Animation.Duration);
        transform.position = Vector3.up * 10f;
        //FightScene1_Manager.people_all_die();
    }
}
