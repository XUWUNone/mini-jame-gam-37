using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public delegate void BasicDelegate();
public class SleepControl : MonoBehaviour
{
    public InputAction StartPressAction;
    public InputAction EndPressAction;
    public Intro intro;
    public float time = 0f;
    public bool IsPress = false;
    public bool IsMovingArrow = false;
    public RectTransform Arrow;
    public RectTransform RedRange;
    public TimeLine TimeLine;
    public float TimeMulti = 960f;
    public Image SleepImage;
    public List<Sprite> SleepSprites;
    public float Speed = 480;
    public int scoreSum = 0;
    private int HighestScore = 0;
    public int thisSleepScore = 100;
    public TextMeshProUGUI HighestScoreText;
    public TextMeshProUGUI scoreText;
    public GameObject ScorePrefab;
    public Transform canvasTransform;
    // Start is called before the first frame update

    public void Awake()
    {
        scoreText.gameObject.SetActive(false);
        HighestScoreText.gameObject.SetActive(false);
    }
    public void GameStart()
    {
        EnableAction();
        SleepImage.sprite = SleepSprites[1];
        Reset();
        scoreText.gameObject.SetActive(true);
        HighestScoreText.gameObject.SetActive(true);
    }
    public void EnableAction()
    {
        StartPressAction.Enable();
        EndPressAction.Enable();
        StartPressAction.performed += OnStartPress;
        EndPressAction.performed += OnEndPress;
    }
    public void DisableAction()
    {
        StartPressAction.Disable();
        EndPressAction.Disable();
        StartPressAction.performed -= OnStartPress;
        EndPressAction.performed -= OnEndPress;
    }
    // Update is called once per frame
    void Update()
    {
        if(IsPress == true && IsMovingArrow == false)
        {
            time += Time.deltaTime;
        }
    }

    public void OnStartPress( InputAction.CallbackContext context )
    {
        if(IsMovingArrow == false)
        {
            AuidoManager.instance.PlaySfxById(1);
            IsPress = true;
            SleepImage.sprite = SleepSprites[0];
        }
    }
    public void OnEndPress(InputAction.CallbackContext context)
    {
        if (IsPress == true && IsMovingArrow == false)
        {
            IsPress = false;
            IsMovingArrow = true;
            StartCoroutine(MovingArrow(time));
            time = 0f;
        }
    }


    IEnumerator MovingArrow(float time )
    {
        float startX = Arrow.anchoredPosition.x;
        float rangeX = time * TimeMulti;
        Vector3 position = Arrow.anchoredPosition;
        float t = 0 + Time.deltaTime;
        while(t < time)
        {
            position.x = startX + rangeX * Mathf.Sqrt(t / time);
            Arrow.anchoredPosition = position;
            yield return null;
            t += Time.deltaTime;
        }
        position.x = startX + rangeX;
        Arrow.anchoredPosition = position;
        SleepImage.sprite = SleepSprites[1];
        //Vector2 size = RedRange.sizeDelta;
        //size.x = 0;
        //RedRange.sizeDelta = size;
        SleepType type = JudgeRange();
        if(type == SleepType.before)
        {
            AuidoManager.instance.PlaySfxById(2);
            thisSleepScore -= 20;
            StartMovingType_Before(rangeX);
        }else if( type == SleepType.spotOn)
        {
            AuidoManager.instance.PlaySfxById(4);
            Vector3 middleP = RedRange.transform.position + new Vector3( RedRange.sizeDelta.x /100 ,0 ,0 );
            GameObject obj = Instantiate(ScorePrefab, middleP, Quaternion.identity, canvasTransform);
            FloatScore score = obj.GetComponent<FloatScore>();
            score.Init(this,thisSleepScore);
            for (float tt = 0; tt < 0.5f; tt += Time.deltaTime)
            {
                Vector3 p = score.rect.anchoredPosition;
                p.y += Time.deltaTime * 360;
                score.rect.anchoredPosition = p;
                yield return null;
            }
            score.isFreeMoving = true;
            Vector2 size = RedRange.sizeDelta;
            size.x = 0;
            RedRange.sizeDelta = size;
            StartMovingSpotOn(rangeX);
        }
        else if (type == SleepType.after)
        {
            AuidoManager.instance.PlaySfxById(3);
            Vector3 middleP = RedRange.transform.position + new Vector3(RedRange.sizeDelta.x / 100, 0, 0);
            GameObject obj = Instantiate(ScorePrefab, middleP, Quaternion.identity, canvasTransform);
            FloatScore score = obj.GetComponent<FloatScore>();
            score.Init(this, -scoreSum);
            score.text.color = new Color(0.75f, 0.3f,0.4f,1f);
            for (float tt = 0; tt < 0.5f; tt += Time.deltaTime)
            {
                Vector3 p = score.rect.anchoredPosition;
                p.y += Time.deltaTime * 480;
                score.rect.anchoredPosition = p;
                yield return null;
            }
            score.isFreeMoving = true;
            Vector2 size = RedRange.sizeDelta;
            size.x = 0;
            RedRange.sizeDelta = size;
            StartMovingSpotOn(rangeX);
            intro.ShowFailChat();
            if(HighestScore < scoreSum)
            {
                HighestScore = scoreSum;
                HighestScoreText.text = HighestScore.ToString();
            }
        }
        
    }
    public void StartMovingSpotOn(float range)
    {
        StartCoroutine(Moving(range, AfterSpotOn));
    }

    public void AfterSpotOn()
    {
        StartCoroutine(MovingTime(TimeLine.GetNextTimeRange()));
    }
    IEnumerator MovingTime(float range)
    {
        float nowRange = 0;
        while (nowRange < range)
        {
            float distance = Time.deltaTime * Speed * 2;
            nowRange += distance;
            TimeLine.MovetoLeft(distance);
            yield return null;
        }
        {
            float distance = range - nowRange;
            TimeLine.MovetoLeft(distance);
        }
        Reset();
    }

    public void StartMovingType_Before(float range)
    {
        StartCoroutine(Moving(range, () => { 
            IsMovingArrow = false;
            SleepImage.sprite = SleepSprites[2];
        }
        
        ));
    }


    IEnumerator Moving(float range , BasicDelegate callBack)
    {
        float nowRange = 0;
        while (nowRange < range)
        {
            float distance = Time.deltaTime * Speed;
            nowRange += distance;
            TimeLine.MovetoLeft(distance);
            Vector3 ArrowP = Arrow.anchoredPosition;
            ArrowP.x -= distance;
            Arrow.anchoredPosition = ArrowP;

            Vector3 redP = RedRange.anchoredPosition;
            redP.x -= distance;
            RedRange.anchoredPosition = redP; 
            yield return null;
        }

        {
            float distance = range - nowRange;
            TimeLine.MovetoLeft(distance);
            Vector3 ArrowP = Arrow.anchoredPosition;
            ArrowP.x -= distance;
            Arrow.anchoredPosition = ArrowP;

            Vector3 redP = RedRange.anchoredPosition;
            redP.x -= distance;
            RedRange.anchoredPosition = redP;
        }

        callBack?.Invoke();
    }

    public void Reset()
    {
        IsMovingArrow = false;
        SleepImage.sprite = SleepSprites[2];
        Vector3 position = RedRange.anchoredPosition;
        position.x = Arrow.anchoredPosition.x + Random.Range(6f, 10f) * 80;
        RedRange.anchoredPosition = position;
        Vector2 size = RedRange.sizeDelta;
        size.x = Random.Range(1f, 5f) * 80;
        RedRange.sizeDelta = size;
        thisSleepScore = 100;
    }

    public SleepType JudgeRange()
    {
        if(Arrow.anchoredPosition.x > RedRange.anchoredPosition.x
            && Arrow.anchoredPosition.x < RedRange.anchoredPosition.x + RedRange.sizeDelta.x
            )
        {

            return SleepType.spotOn;
        }
        else if (Arrow.anchoredPosition.x >= RedRange.anchoredPosition.x + RedRange.sizeDelta.x)
        {
            return SleepType.after;
        }
        else
        {
            return SleepType.before;
        }

    }

    public void AddScore(int value)
    {
        scoreSum += value;
        scoreText.text = scoreSum.ToString();
    }
}
public enum SleepType
{
    before = 1,
    spotOn = 0,
    after = -1,
}