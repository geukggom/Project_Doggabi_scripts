using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAni_script : MonoBehaviour
{
    public void ani_parent_over()
    {
        transform.parent.gameObject.SetActive(false);
    }
    public void grass_ani_over()
    {
        transform.gameObject.SetActive(false);
    }
}
