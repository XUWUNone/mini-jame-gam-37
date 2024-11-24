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
            GameObject obj = Instantiate(timePrefab, this.transform);
            ThisTime time = obj.GetComponent<ThisTime>();
            time.text.text = (i % 24 + 1).ToString();
            time.NowTime = (i % 24 + 1);
            timelines.Add(time);
        }
    }

    public void MovetoLeft(float x)
    {
        Vector3 p = rect.anchoredPosition;
        p.x -= x;
        while (p.x < -80)
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

        rect.anchoredPosition = p;
    }

    public float GetNextTimeRange()
    {
        float randTime = Random.Range(20f, 25f);
        float range = (randTime - timelines[0].NowTime) * 80;
        return range;


    }
}


