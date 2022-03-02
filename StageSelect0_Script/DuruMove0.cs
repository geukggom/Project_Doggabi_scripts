using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuruMove0 : MonoBehaviour
{
    public RectTransform leftDuru;
    public RectTransform rightDuru;
    public RectTransform centerDuru;        //3개 다 dran&drop으로 연결시켜줌

    bool is_center;
    float duru_speed = 3f;



    public void Opening()           //두루마리 펼쳐짐.
    {
        is_center = true;
        StartCoroutine(duruopening());
    }
    IEnumerator duruopening()
    {
        while (true)
        {
            if ((leftDuru.anchoredPosition.x < -85.9 && rightDuru.anchoredPosition.x > 85.9) || !is_center) break;

            leftDuru.anchoredPosition = Vector3.Lerp(leftDuru.anchoredPosition,new Vector3(-88,0,0),Time.deltaTime * duru_speed);
            rightDuru.anchoredPosition = Vector3.Lerp(rightDuru.anchoredPosition,new Vector3(88,0,0),Time.deltaTime * duru_speed);
            centerDuru.sizeDelta = Vector2.Lerp(centerDuru.sizeDelta,new Vector2(160,79), Time.deltaTime * duru_speed);

            yield return null;
        }
    }



    public void Closing()           //두루마리 닫음.
    {
        is_center = false;
        StartCoroutine(duruclosing());
    }
    IEnumerator duruclosing()
    {
        while (true)
        {
            if ((leftDuru.anchoredPosition.x >= -10 && rightDuru.anchoredPosition.x <= 10) || is_center) break;

            leftDuru.anchoredPosition = Vector3.Lerp(leftDuru.anchoredPosition, new Vector3(-5, 0, 0), Time.deltaTime * duru_speed);
            rightDuru.anchoredPosition = Vector3.Lerp(rightDuru.anchoredPosition, new Vector3(5, 0, 0), Time.deltaTime * duru_speed);
            centerDuru.sizeDelta = Vector2.Lerp(centerDuru.sizeDelta, new Vector2(30, 79), Time.deltaTime * duru_speed);

            yield return null;
        }
    }


}
