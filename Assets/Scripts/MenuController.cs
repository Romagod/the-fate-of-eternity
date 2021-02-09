using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using System;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject RegisterPanel;
    public string IUsername;
    public string IPassword;
    public string IEmail;

    private string TMPIUsername;
    private string TMPIPassword;
    private string TMPIEmail;

    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
    public class Token
    {
        public string type { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
    public class DBPlayer
    {
        public User user { get; set; }
        public Token access_token { get; set; }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("User"))
        {
            showLogin();
            Debug.Log(PlayerPrefs.GetString("Token"));
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    public void Login()
    {
        Debug.Log(IUsername);
        Debug.Log(IPassword);

        HTTPRequest request = new HTTPRequest(new Uri("http://tfoe.loldev.ru/api/auth/login"), HTTPMethods.Post, OnRequestFinished);
        request.AddField("username", IUsername);
        request.AddField("password", IPassword);
        request.Send();
    }

    public void Register()
    {
        HTTPRequest request = new HTTPRequest(new Uri("http://tfoe.loldev.ru/api/auth/signup"), HTTPMethods.Post, OnRequestFinished);
        request.AddField("email", IEmail);
        request.AddField("username", IUsername);
        request.AddField("password", IPassword);
        request.Send();
    }

    public void showLogin()
    {
        NulableCredits();
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
    }

    public void showRegister()
    {
        NulableCredits();
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }

    public void OnUsernameChange(string value)
    {
        TMPIUsername = value;
    }

    public void OnUsernameEndEdit()
    {
        IUsername = TMPIUsername;
    }

    public void OnPasswordChange(string value)
    {
        TMPIPassword = value;
    }

    public void OnPasswordEndEdit()
    {
        IPassword = TMPIPassword;
    }

    public void OnEmailChange(string value)
    {
        TMPIEmail = value;
    }

    public void OnEmailEndEdit()
    {
        IEmail = TMPIEmail;
    }

    private void NulableCredits()
    {
        IUsername = null;
        IPassword = null;
        IEmail = null;
    }

    void OnRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        switch (request.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (response.IsSuccess)
                {
                    Debug.Log("Request Finished! Text received: " + response.DataAsText);
                    var player = JsonConvert.DeserializeObject<DBPlayer>(response.DataAsText);
                    if (player.access_token.token != null)
                    {
                        PlayerPrefs.SetString("User", JsonConvert.SerializeObject(player.user));
                        PlayerPrefs.SetString("Token", player.access_token.token);

                    }
                    PlayerPrefs.Save();
                    Debug.Log("Request Finished! Text received: " + response.DataAsText);
                    SceneManager.LoadScene(1);
                }
                else
                {
                    Debug.LogWarning(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                    response.StatusCode,
                                                    response.Message,
                                                    response.DataAsText));
                }
                break;

            // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
            case HTTPRequestStates.Error:
                Debug.LogError("Request Finished with Error! " + (request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception"));
                break;

            // The request aborted, initiated by the user.
            case HTTPRequestStates.Aborted:
                Debug.LogWarning("Request Aborted!");
                break;

            // Connecting to the server is timed out.
            case HTTPRequestStates.ConnectionTimedOut:
                Debug.LogError("Connection Timed Out!");
                break;

            // The request didn't finished in the given time.
            case HTTPRequestStates.TimedOut:
                Debug.LogError("Processing the request Timed Out!");
                break;
        }

        
    }
}
