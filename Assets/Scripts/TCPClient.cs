using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using System;
using BestHTTP;
using BestHTTP.WebSocket;
//using BestHTTP.SocketIO;
using SysSocket = System.Net.Sockets;


[System.Serializable]
public class TCPClient : MonoBehaviour
{
    //private SocketManager Manager;
    //private Socket Socket;
    private WebSocket webSocket = null;
    private bool open = false;
    private bool reconnect = false;
    private string m_PlayerToken;
    public string m_PlayerName;
    public string m_PlayerPass;
    Timer timer;
    Timer pinger;
    public float targetTime = 0.0f;
    public float pingerTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        SetupServer();
    }

    // Update is called once per frame
    void Update()
    {
        
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    try
        //    {


        //        Debug.Log("1!");
        //        SendEvent("message", "hello");

        //    }
        //    catch (SysSocket.SocketException ex)
        //    {
        //        Debug.Log("e");
        //        Debug.Log(ex.Message);
        //    }
        //}
    }

    private void checkSocket()
    {
        if (webSocket != null && !webSocket.IsOpen && reconnect)
        {
            webSocket = null;
            open = false;
            SetupServer();
            reconnect = false;
        }
    }

    private void SetupServer()
    {
        try
        {
            if (m_PlayerToken == null)
            {
                m_PlayerToken = PlayerPrefs.GetString("Token");
            }
            
            if (m_PlayerToken == null)
            {
                if (m_PlayerName == null)
                {
                    m_PlayerName = PlayerPrefs.GetString("Name", null);

                    if (m_PlayerName == null)
                    {
                        return;
                    }
                }
                if (m_PlayerPass == null)
                {
                    m_PlayerPass = PlayerPrefs.GetString("Pass", null);

                    if (m_PlayerPass == null)
                    {
                        return;
                    }
                }

                webSocket = new WebSocket(new Uri("ws://tfoe.loldev.ru/adonis-ws?name=" + m_PlayerName + "&pass=" + m_PlayerPass));
            }
            else
            {
                Debug.Log("connecting...");
                webSocket = new WebSocket(new Uri("ws://tfoe.loldev.ru/adonis-ws?token=" + m_PlayerToken));

            }
            Debug.Log(m_PlayerName);
            Debug.Log(m_PlayerPass);
            webSocket.OnOpen += OnOpen;
            webSocket.OnMessage += OnMessageReceived;
            webSocket.OnClosed += OnClosed;
            webSocket.OnError += OnError;
            if (!open)
            {
                Debug.Log("Opening...");
                webSocket.Open();
            }

        }
        catch (SysSocket.SocketException ex)
        {
            Debug.Log("e");
            Debug.Log(ex.Message);
        }

    }
    void OnOpen(WebSocket ws)
    {
        if (!open)
        {
            ws.Send("{\"t\": 1, \"d\": {\"topic\": \"game\", \"data\": {\"userId\": 1} }}");
            open = true;
            pinger = new Timer(PingTimerCallback, null, 100, 100);
        }
        Debug.Log("WebSocket Open!");

    }

    public void SendEvent(string eventName, string data, string topic = "game")
    {
        if (open)
        {
            webSocket.Send("{\"t\": 7, \"d\": {\"topic\": \"" + topic + "\", \"event\": \"" + eventName + "\", \"data\": " + data + " }}");
            Debug.Log("Event sended!");
        }
    }



    void OnMessageReceived(WebSocket ws, string message)
    {
        Debug.Log(string.Format("Message received: <color=yellow>{0}</color>", message));
    }

    void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        Debug.Log(string.Format("WebSocket closed! Code: {0} Message: {1}", code, message));
        open = false;
        webSocket = null;
    }

    /// <summary>
    /// Called when an error occured on client side
    /// </summary>
    void OnError(WebSocket ws, string error)
    {
        Debug.Log(string.Format("An error occured: <color=red>{0}</color>", error));
        reconnect = true;
        if (webSocket != null)
        {
            webSocket.Close();

            timer = new Timer(TimerCallback, null, 100, 100);
        }

        /* webSocket = null;*/
    }

    private void TimerCallback(object o)
    {
        // Display the date/time when this method got called. 

        targetTime += 0.1f;
        //slider.SetValueWithoutNotify(targetTime);
        //slider.normalizedValue = targetTime;
        if (targetTime >= 5.0f)
        {
            timer.Dispose();
            checkSocket();
            targetTime = 0.0f;
        }
        // Force a garbage collection to occur for this demo.
        GC.Collect();
    }

    private void PingTimerCallback(object o)
    {
        // Display the date/time when this method got called. 

        pingerTime += 0.1f;
        //slider.SetValueWithoutNotify(targetTime);
        //slider.normalizedValue = targetTime;
        if (pingerTime >= 5.0f)
        {
            
            Ping();
            pingerTime = 0.0f;
        }
        // Force a garbage collection to occur for this demo.
        GC.Collect();
    }

    private void Ping ()
    {
        if (open)
        {
            SendEvent("message", "{\"event\": \"Ping\"}");
        }
    }

    public void OnDestroy()
    {
        Debug.Log("onDestroy");
        if (webSocket != null)
        {
            Debug.Log("close");
            pinger.Dispose();
            reconnect = false;
            webSocket.Close();
            webSocket = null;
        }
    }
}
