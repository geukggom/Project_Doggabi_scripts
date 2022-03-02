using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class FireSkill1_Manager : MonoBehaviour
{
    public SkeletonAnimation myanim;
    public AnimationReferenceAsset atk;
    
    public void duok_skill_use(float x)
    {
        myanim.state.SetAnimation(0, atk, false);
        myanim.loop = false;
        myanim.timeScale = 1f;
        StartCoroutine(skilltime());
        transform.position = Vector3.zero + Vector3.right * x;
    }
    IEnumerator skilltime()
    {
        yield return new WaitForSeconds(atk.Animation.Duration);
        transform.position = Vector3.up * 10f;
    }
}
