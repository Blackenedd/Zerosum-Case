using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region Singleton
    public static InputManager instance = null;
    private void Awake()
    {
        if (instance == null) { instance = this; }
    }
    #endregion

    public PointerEventData eventData;

    private float maxDistance = 100f;
    private float precisionDistance = 100f;
    private float tapTime = 0.2f;
    private float holdTime = 1f;

    private float time;
    private bool swiped = false;

    private Vector2 startedPos;
    private Vector2 lastPos;

    [HideInInspector] public Vector2 delta;
    [HideInInspector] public Vector2 input;

    [HideInInspector] public UnityEvent swipeLeft = new UnityEvent();
    [HideInInspector] public UnityEvent swipeRight = new UnityEvent();
    [HideInInspector] public UnityEvent swipeUp = new UnityEvent();
    [HideInInspector] public UnityEvent swipeDown = new UnityEvent();
    [HideInInspector] public UnityEvent tap = new UnityEvent();

    private void Start()
    {
        ListenEvents();
    }
    private void Update()
    {
        InputDelta();
    }
    private void InputDelta()
    {
        if (eventData == null) return;
        delta = eventData.position - lastPos;
        delta.x = Mathf.Clamp(delta.x, -maxDistance, maxDistance);
        delta.y = Mathf.Clamp(delta.y, -maxDistance, maxDistance);
        input = delta / maxDistance;
        lastPos = eventData.position;
    }
    private void ListenEvents()
    {
        swipeLeft.AddListener(() => Debug.Log("left"));
        swipeRight.AddListener(() => Debug.Log("right"));
        swipeUp.AddListener(() => Debug.Log("up"));
        swipeDown.AddListener(() => Debug.Log("down"));
        tap.AddListener(() => Debug.Log("tap"));
    }
    public void OnPointerDown(PointerEventData _eventData)
    {
        eventData = _eventData;
        startedPos = lastPos = eventData.position;
        time = Time.time;
        swiped = false;
    }
    public void OnPointerUp(PointerEventData _eventData)
    {
        Vector2 diff = eventData.position - startedPos;

        if (Time.time - time < holdTime)
        {
            if (diff.magnitude >= precisionDistance)
            {
                swiped = true;
                if (Mathf.Abs(diff.x) * 3 > Mathf.Abs(diff.y))
                {
                    if (diff.x > 0) swipeRight.Invoke();
                    else swipeLeft.Invoke();
                }
            }

            if (diff.magnitude >= precisionDistance * 3f)
            {
                if (Mathf.Abs(diff.x) * 3 < Mathf.Abs(diff.y))
                {
                    if (diff.y > 0) swipeUp.Invoke();
                    else swipeDown.Invoke();
                }
            }
        }

        if (!swiped && Time.time - time <= tapTime) tap.Invoke();

        eventData = null;
        delta = Vector2.zero;
        startedPos = Vector2.zero;
        input = Vector2.zero;
    }
    #region Delay
    private Coroutine Delay(float _waitTime = 1f, UnityAction onComplete = null)
    {
        return StartCoroutine(DelayCoroutine(_waitTime, onComplete));
    }
    private IEnumerator DelayCoroutine(float t, UnityAction ua)
    {
        yield return new WaitForSeconds(t);
        ua?.Invoke();
    }
    #endregion
}
