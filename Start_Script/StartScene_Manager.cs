using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene_Manager : MonoBehaviour
{
    public void Start_SelectScene()
    {
        PlayerPrefs.SetInt(GlobalAtoZ.String_STAGENUM, 1);
        SceneManager.LoadScene(1);
    }

    //private void Update()
    //{
    //    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    if (Input.GetMouseButtonDown(0) && pos.x>-2.6f) Start_SelectScene();
    //}
}
