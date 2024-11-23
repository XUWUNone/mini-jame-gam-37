using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeLine : MonoBehaviour
{
    public GameObject timePrefab;
    public List<ThisTime> timelines = new List<ThisTime>();
    public float Speed = 480;
    private RectTransform rect;
    public SleepControl sleepControl;
    public void Awake()
    {
        rect = GetComponent<RectTransform>();
        for (int i = 0; i < 25; i++)
        {
            GameObject obj = Instantiate(timePrefab , this.transform);
            ThisTime time = obj.GetComponent<ThisTime>();
            time.text.text = (i%24 + 1).ToString();
            time.NowTime = (i % 24 + 1);
            timelines.Add(time);
        }
    }

    public void StartMovingLeft(float x, RectTransform arrow)
    {
        StartCoroutine(MovingLeft(x,arrow));
    }


    IEnumerator MovingLeft(float x , RectTransform arrow)
    {
        float range = 0;
        Debug.Log(x);
        while(range < x)
        {
            Vector3 p = rect.anchoredPosition;
            Vector3 ArrowP = arrow.anchoredPosition;
            ArrowP.x -= Time.deltaTime * Speed;
            p.x -= Time.deltaTime * Speed;
            range += Time.deltaTime * Speed;
            if (p.x < -80)
            {
                p.x += 80;
                ThisTime t = timelines[0];
                int i = timelines[timelines.Count - 1].NowTime;
                t.text.text = (i % 24 + 1).ToString();
                t.NowTime = (i % 24 + 1);
                timelines.RemoveAt(0);
                timelines.Add(t);
                t.transform.SetSiblingIndex(transform.childCount - 1);
            }
            arrow.anchoredPosition = ArrowP;
            rect.anchoredPosition = p;
            yield return null;
        }

        {
            Vector3 p = rect.anchoredPosition;
            Vector3 ArrowP = arrow.anchoredPosition;
            ArrowP.x -= x - range;
            p.x -= x - range;

            if (p.x < -80)
            {
                p.x += 80;
                ThisTime t = timelines[0];
                int i = timelines[timelines.Count - 1].NowTime;
                t.text.text = (i % 24 + 1).ToString();
                t.NowTime = (i % 24 + 1);
                timelines.RemoveAt(0);
                timelines.Add(t);
                t.transform.SetSiblingIndex(transform.childCount - 1);
            }
            arrow.anchoredPosition = ArrowP;
            rect.anchoredPosition = p;
        }

        sleepControl.Reset();
    }
}
