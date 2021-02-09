using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameMove
{
    public GameObject button = null;
    public bool state = false;
    public int count = 0;
    public event Action<string> Notify;

    Timer timer;
    public float targetTime = 30.0f; 

    //public GameMove()
    //{
    //    initGameMove();
    //}

    //void initGameMove()
    //{

    //}

    public void startHandlers()
    {

        button.GetComponent<Button>().onClick.AddListener(() => handleClick());
        button.GetComponentInChildren<Text>().text = "Старт";

    }

    public void handleClick()
    {
        state = !state; 

        if(!state)
        {
            End();
            button.GetComponentInChildren<Text>().text = "Ход противника";
        } else
        {
            button.GetComponentInChildren<Text>().text = "Завершить ход";
            Debug.Log("else");
            timer = new Timer(TimerCallback, null, 100, 100);
            //timer1.Change(5000, 5000);

        }
    }

    public void End()
    {
        if (state)
        {
            state = false;
        }
        Notify?.Invoke("End");
        if (timer != null)
        {
            timer.Dispose();
        }
        SetTimer();
        GC.Collect();
        count++;
    }

    private void SetTimer()
    {
        targetTime = 30.0f;
        //slider.normalizedValue = targetTime;
    }

    private void timerEnded()
    {
        End();
    }

    private void TimerCallback(object o)
    {
        // Display the date/time when this method got called. 

        targetTime -= 0.1f;
        //slider.SetValueWithoutNotify(targetTime);
        //slider.normalizedValue = targetTime;
        if (targetTime <= 0.0f)
        {
            timer.Dispose();
            timerEnded();
            SetTimer();
        }
        // Force a garbage collection to occur for this demo.
        GC.Collect();
    }
}
