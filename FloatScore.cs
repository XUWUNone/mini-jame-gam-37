using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatScore : MonoBehaviour
{
    public SleepControl control = null;
    public RectTransform rect;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 1f;
    public int score = 100;
    public bool isFreeMoving = false;
    public TextMeshProUGUI text;

    public void Init(SleepControl c, int scoreValue)
    {
        control = c;
        score = scoreValue;
        text.text = score.ToString();
    }
    public void Update()
    {
        if(control != null  && isFreeMoving == true)
        {
            Vector3 targetP = control.scoreText.transform.position;
            if((targetP - transform.position).magnitude < 2f)
            {
                isFreeMoving = false;
                StartCoroutine(Fading());
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetP, ref velocity, smoothTime);
            }
        }
    }

    IEnumerator Fading()
    {
        float ogSize = text.fontSize;
        for(float time = 0; time < 0.5f; time += Time.deltaTime)
        {
            Color color = text.color;
            color.a = 1 - time / 0.5f;
            text.color = color;

            text.fontSize = (1 + Mathf.Sqrt(time / 0.5f)) * ogSize;
            yield return null;
        }
        control.AddScore(score);
        Destroy(gameObject);
    }
}
