using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightInformation0 : MonoBehaviour
{
    public int stagenum = -1;               //씬 전환할 때 StageSelect0_Manager여기서 스테이지 번호 할당받음.
    public bool is_complete = false;

    private void Awake()
    {
        StartCoroutine(stageSetting());
    }

    /// <summary>
    /// 스테이지 번호 할당받을 때까지 도는 코루틴.
    /// </summary>
    /// <returns></returns>
    IEnumerator stageSetting() 
    {
        while (true)
        {
            if(stagenum >= 0)
            {
                stageSetting2();
                break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 스테이지 세팅 -> 스테이지 배경, 적 종류, Wave개수, 각 Wave 당 적의 종류와 수
    /// </summary>
    void stageSetting2() 
    {
        switch (stagenum)
        {
            case 0:
                Stage_bg = Resources.Load<Sprite>(GlobalAtoZ.Sprite_Fight_bg + "/Stage1_map");
                Wave_num = 1;
                Enemy_kind = 1;
                Enemy = new GameObject[Enemy_kind];
                EnemyNum = new int[Enemy_kind];
                Enemy[0] = Resources.Load<GameObject>(GlobalAtoZ.Prefab_Boar);
                EnemyNum[0] = 0;

                Wave_Enemynum = new List<int>[Wave_num];
                Wave_Enemynum[0] = new List<int>();
                Wave_Enemynum[0].Add(0);
                boss_distance = -1.5f;
                is_complete = true;         //세팅 완료 신호
                break;

            case 1:
                Stage_bg = Resources.Load<Sprite>(GlobalAtoZ.Sprite_Fight_bg + "/Stage2_map");
                Wave_num = 3;
                Enemy_kind = 2;
                Enemy = new GameObject[Enemy_kind];
                EnemyNum = new int[Enemy_kind];
                Enemy[0] = Resources.Load<GameObject>(GlobalAtoZ.Prefab_Wolf);
                Enemy[1] = Resources.Load<GameObject>(GlobalAtoZ.Prefab_Wolf); //보스는 스킨만 다르게.
                EnemyNum[0] = 1;
                EnemyNum[1] = 2;

                Wave_Enemynum = new List<int>[Wave_num];
                for (int i = 0; i < 3; i++)
                {
                    Wave_Enemynum[i] = new List<int>();
                }
                for (int i = 0; i < 4; i++)
                {
                    Wave_Enemynum[0].Add(0);
                    Wave_Enemynum[1].Add(0);
                    if (i <= 1) Wave_Enemynum[2].Add(0);
                    else if (i == 2) Wave_Enemynum[2].Add(1);
                }
                boss_distance = -1.5f;
                is_complete = true;         //세팅 완료 신호
                break;
            case 2:
                Debug.Log("스테이지3세팅");
                break;
        }
    }
    public Sprite Stage_bg;
    public int Wave_num;
    public int Enemy_kind;
    public GameObject[] Enemy;
    public int[] EnemyNum;
    public List<int>[] Wave_Enemynum;
    public float boss_distance;
}
