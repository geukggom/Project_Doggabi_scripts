using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Manager : MonoBehaviour
{
    public GameObject sprite_ani_prefab;
    Sprite[] number_red;
    Sprite[] number_blue;
    public List<Transform> damage_num_list;
    public List<Transform> damage_sprite_list;

    int num_count = 0;
    int ds_count = 0;

    void Awake()
    {
        number_red = Resources.LoadAll<Sprite>(GlobalAtoZ.Sprite_Number_red);
        number_blue = Resources.LoadAll<Sprite>(GlobalAtoZ.Sprite_Number_blue);
        sprite_ani_prefab = Resources.Load<GameObject>(GlobalAtoZ.Anima_Damaged);

        for (int j = 0; j < 30; j++)
        {
            GameObject a = new GameObject("Number_parent");
            a.transform.SetParent(transform);
            for (int i = 0; i < 6; i++)
            {
                GameObject b = new GameObject("num_" + i);
                b.AddComponent<SpriteRenderer>().sortingOrder = 100;
                b.transform.SetParent(a.transform);
                b.SetActive(false);
            }
            a.transform.localScale = Vector3.one * 0.5f;
            damage_num_list.Add(a.transform);
            a.SetActive(false);
        }

        GameObject k = new GameObject("DamageSprite_parent");
        for (int i = 0; i < 30; i++)
        {
            GameObject aa = Instantiate(sprite_ani_prefab);
            aa.transform.SetParent(k.transform);
            damage_sprite_list.Add(aa.transform);
            aa.gameObject.SetActive(false);
        }
    }

    public void use_number(Vector3 pos, int damage,bool is_enemy,float a)
    {
        damage_num_list[num_count].gameObject.SetActive(true);
        damage_num_list[num_count].localScale = Vector3.one * a;
        damage_num_list[num_count].transform.position = pos;
        //Debug.Log(damage);

        string intToString = damage.ToString();
        char[] num_array = intToString.ToCharArray();
        for (int i = 0; i < num_array.Length; i++)
        {
            damage_num_list[num_count].transform.GetChild(i).gameObject.SetActive(true);
            damage_num_list[num_count].GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            if (is_enemy) damage_num_list[num_count].transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = number_blue[int.Parse(num_array[i].ToString())];
            else damage_num_list[num_count].transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = number_red[int.Parse(num_array[i].ToString())];

            damage_num_list[num_count].transform.GetChild(i).localPosition = new Vector3(num_array.Length * (-0.5f) + i + 0.5f,0,0);
        }
        StartCoroutine(num_move(num_count));
        num_count++;
        if (num_count >= damage_num_list.Count) num_count = 0;
    }

    IEnumerator num_move(int count)
    {
        Vector3 pos = damage_num_list[count].transform.position;
        while (true)
        {
            damage_num_list[count].transform.position = Vector3.Lerp(damage_num_list[count].transform.position, pos + Vector3.up * 3.5f,Time.deltaTime * 1f);

            if(damage_num_list[count].transform.position.y >= pos.y + 1.5f)
            {
                float a = pos.y + 2.5f - damage_num_list[count].transform.position.y;
                for (int i = 0; i < damage_num_list[count].childCount; i++)
                {
                    damage_num_list[count].GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, a);
                }
            }

            if (Vector3.Distance(damage_num_list[count].transform.position, pos + Vector3.up * 2.5f) < 0.1f) break;
            yield return null;
        }
        for (int i = 0; i < damage_num_list[count].childCount; i++)
        {
            if (damage_num_list[count].GetChild(i).gameObject.activeSelf) damage_num_list[count].GetChild(i).gameObject.SetActive(false);
        }
        damage_num_list[count].gameObject.SetActive(false);
    }

    public void use_ani(Vector3 pos, int layer)
    {
        int num = Random.Range(0, 360);
        damage_sprite_list[ds_count].gameObject.SetActive(true);
        damage_sprite_list[ds_count].GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = layer;
        damage_sprite_list[ds_count].position = pos + Vector3.down * 1.5f;
        damage_sprite_list[ds_count].rotation = Quaternion.Euler(0,0,num);
        ds_count++;
        if (ds_count >= damage_sprite_list.Count) ds_count = 0;
    }
}
