using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAtoZ : MonoBehaviour
{
    public static GameObject Find_name(string name)
    {
        return GameObject.Find(name);
    }

    public static GameObject Find_tag(string tag_name)
    {
        return GameObject.FindGameObjectWithTag(tag_name);
    }

    public static GameObject[] Find_tagArray(string tag_name)
    {
        return GameObject.FindGameObjectsWithTag(tag_name);
    }
}
