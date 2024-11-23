using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SleepControl : MonoBehaviour
{
    public InputAction StartPressAction;
    public InputAction EndPressAction;
    public float time = 0f;
    public bool IsPress = false;
    public bool IsMovingArrow = false;
    public RectTransform Arrow;
    public RectTransform RedRange;
    public TimeLine TimeLine;
    public float TimeMulti = 960f;
    public Image SleepImage;
    public List<Sprite> SleepSprites;
    // Start is called before the first frame update

    private void OnEnable()
    {
        StartPressAction.Enable();
        EndPressAction.Enable();
        StartPressAction.performed += OnStartPress;
        EndPressAction.performed += OnEndPress;
        SleepImage.sprite = SleepSprites[1];
        Reset();
    }
    private void OnDisable()
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


    IEnumerator MovingArrow(float time)
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
        Vector2 size = RedRange.sizeDelta;
        size.x = 0;
        RedRange.sizeDelta = size;

        TimeLine.StartMovingLeft(time * TimeMulti , Arrow);
    }


    public void Reset()
    {
        IsMovingArrow = false;
        Vector3 position = RedRange.anchoredPosition;
        position.x = Arrow.anchoredPosition.x + Random.Range(6f, 10f) * 80;
        RedRange.anchoredPosition = position;
        Vector2 size = RedRange.sizeDelta;
        size.x = Random.Range(2f, 5f) * 80;
        RedRange.sizeDelta = size;
    }
}
