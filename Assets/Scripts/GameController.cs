using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP;
using System;
using Newtonsoft.Json;
using Assets.Scripts;

public class GameController : MonoBehaviour
{
    public GameObject InterfaceObject;
    public GameObject ViewContainer;
    public GameObject FirstCard;
    public GameObject MyCardsContainer;
    public GameObject CameraAnimator;
    public bool giveCards = true;
    public GameObject Board;
    public Energy energy = new Energy();
    public GameMove gameMove = new GameMove();
    public Slider slider = null;
    public Text MyResources = null;
    public GameObject Client;

    public GameObject ViewCard;
    public GameObject SelectedCell;

    public JsonToCards Cards;
    
    Vector2 LastMin;
    Vector2 LastMax;

    private List<GameObject> MyCards;
    private Ray raycast;


    // Start is called before the first frame update
    void Start()
    {
        FirstCard.SetActive(false);
        MyCards = new List<GameObject>();
        gameMove.startHandlers();
        gameMove.Notify += GameMoveHandler;
        energy.Notify += EnergyHandler;
        MyResources.text = $"{energy.Amount} / {energy.MaxEnergy}";
        GetGameInfo();
        //MyCards.Add(FirstCard);
    }

    // Update is called once per frame
    void Update()
    {
        ProcessClick();
        CheckCards();

        slider.SetValueWithoutNotify(gameMove.targetTime);
    }

    private void GetGameInfo()
    {
        Debug.Log("GetInfo...");
        if (PlayerPrefs.GetString("Token") == null)
        {
            Debug.LogError("Token doesn't exists");
            return;
        }
        HTTPRequest request = new HTTPRequest(new Uri("http://tfoe.loldev.ru/api/cards"), HTTPMethods.Get, OnRequestFinished);
        request.SetHeader("Authorization", "Bearer " + PlayerPrefs.GetString("Token"));
        request.Send();
    }

    //
    private void GameMoveHandler(string message)
    {
        if (message == "End")
        {
            Client.GetComponent<TCPClient>().SendEvent("message", "{\"event\": \"moveEnded\"}");
            energy.Reset();
        }
    }

    //
    private void EnergyHandler(string message)
    {
        if (message == "SetEnergy")
        {
            MyResources.text = $"{energy.Amount} / {energy.MaxEnergy}";
        }
    }

    public void DropCard(int index)
    {
        if (ViewCard != null)
        {
            
            ViewCard.transform.SetParent(MyCardsContainer.transform, false);
            ViewCard.GetComponent<RectTransform>().offsetMin = LastMin;
            ViewCard.GetComponent<RectTransform>().offsetMax = LastMax;
            if (ViewCard.GetComponent<Card>().index != index)
            {
                CardToView(index);
            } else
            {
                CameraAnimator.GetComponent<CameraAnimator>().StartReverseAnimation();
                MyCardsContainer.SetActive(true);
                ViewCard = null;
            }
        } else
        {
            CardToView(index);
            MyCardsContainer.SetActive(false);
        }
        
    }

    void CardToView(int index)
    {
        ViewCard = MyCards[index];
        LastMin = MyCards[index].GetComponent<RectTransform>().offsetMin;
        LastMax = MyCards[index].GetComponent<RectTransform>().offsetMax;
        ViewCard.transform.SetParent(ViewContainer.transform, false);
        float X = ViewCard.GetComponent<RectTransform>().sizeDelta.x;
        float Y = ViewCard.GetComponent<RectTransform>().sizeDelta.y;
        float step = (((X / 2) + 25f));
        int delimeter = 2;
        ViewCard.GetComponent<RectTransform>().offsetMin = new Vector2(-(X / delimeter), -(Y / delimeter));
        ViewCard.GetComponent<RectTransform>().offsetMax = new Vector2((X / delimeter), (Y / delimeter));
        ViewCard.GetComponent<RectTransform>().offsetMin = new Vector2(ViewCard.GetComponent<RectTransform>().offsetMin.x + step/2, ViewCard.GetComponent<RectTransform>().offsetMin.y - (Y / delimeter)*2);
        ViewCard.GetComponent<RectTransform>().offsetMax = new Vector2(ViewCard.GetComponent<RectTransform>().offsetMax.x + step*2, ViewCard.GetComponent<RectTransform>().offsetMax.y - (Y / delimeter)/2);

        CameraAnimator.GetComponent<CameraAnimator>().StartAnimation();
        
    }

    void CheckCards()
    {
        
        if (giveCards)
        {
            if (MyCards.Count >= 1)
            {
                giveCards = false;
                List<GameObject> MyNewCards = MyCardsContainer.GetComponent<MyCards>().RenderCards(MyCards);
                MyCards = MyNewCards;
            }
            else
            {
                MyCards.Add(FirstCard);
            }
        }
        
    }

    void ProcessClick()
    {
        //this.handleTouch();
        this.handleClick();
    }

    void handleTouch()
    {   
        if ((Input.touchCount > 0))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit raycastHit;
                if (Physics.Raycast(raycast, out raycastHit))
                {
                    this.Hit(raycast, raycastHit);
                }
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                
            }
        }
    }

    void handleClick()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit raycastHit;
        //    if (Physics.Raycast(raycast, out raycastHit))
        //    {
        //        this.Hold(raycast, raycastHit);
        //    }
        //}
        if (Input.GetMouseButtonDown(0))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                this.Hit(raycast, raycastHit);
            }
        }

    }

    void takeCard(Cell cell)
    {
        if (ViewCard != null && energy.CheckPrice(ViewCard.GetComponent<Card>().price))
        {
            GameObject found = MyCards.Find(item => item.GetComponent<Card>().index == ViewCard.GetComponent<Card>().index);
            MyCards.Remove(found);
            cell.PutCard(ViewCard);
            Destroy(ViewCard.GetComponent<Image>());
            Debug.Log("Price:");
            Debug.Log(ViewCard.GetComponent<Card>().price);
            energy.useEnergy(ViewCard.GetComponent<Card>().price);
            ViewCard = null;
            CameraAnimator.GetComponent<CameraAnimator>().StartReverseAnimation();
            MyCardsContainer.SetActive(true);
            MyCardsContainer.SetActive(true);
            Client.GetComponent<TCPClient>().SendEvent("message", "{\"cellId\": " + cell.Id + ", \"cardId\": "+ cell.CardObject.GetComponent<Card>().id + ", \"event\": \"takeCard\"}");
        }
    }

    void selectCell(GameObject cell)
    {
        if (SelectedCell == null)
        {
            if (cell.GetComponent<Cell>().Unit != null)
            {
                CameraAnimator.GetComponent<CameraAnimator>().StartAnimation();
                MyCardsContainer.SetActive(false);

                SelectedCell = cell;
            }
        } 
        else
        {
            SelectedCell.GetComponent<Cell>().replaceUnit(cell);
            Client.GetComponent<TCPClient>().SendEvent("message", "{\"fromCellId\": " + SelectedCell.GetComponent<Cell>().Id + ", \"cellId\": " + cell.GetComponent<Cell>().Id + ", \"cardId\": " + cell.GetComponent<Cell>().Unit.GetComponent<UnitInfo>().Card.GetComponent<Card>().id + ", \"event\": \"moveUnit\"}");
            
            energy.useEnergy(1);
            Client.GetComponent<TCPClient>().SendEvent("message", "{\"event\": \"useEnergy\", \"count\": \"1\", }");


            if (cell.GetComponent<Cell>().HasResource())
            {
                energy.addMax(1);
                Client.GetComponent<TCPClient>().SendEvent("message", "{\"event\": \"addMaxEnergy\", \"count\": \"1\", }");

            }

            SelectedCell = null;
            CameraAnimator.GetComponent<CameraAnimator>().StartReverseAnimation();
            MyCardsContainer.SetActive(true);
        }
    }

    void Hit(Ray raycast, RaycastHit raycastHit)
    {
        if (raycastHit.collider.CompareTag("Cell"))
        {
            GameObject cell = raycastHit.collider.gameObject;
            if (ViewCard != null)
            {
                takeCard(cell.GetComponent<Cell>());
            }
            else
            {
                if (energy.CheckPrice(1))
                {
                    selectCell(cell);
                }
            }
            //raycastHit.collider.gameObject.GetComponent<Cell>().SetType(3);
            cell.GetComponent<Cell>().HoldStop();
            Debug.Log("Cell Hit");
        }
        else
        {
            Debug.Log("Something Hit");
        }
    }

    void Hold(Ray raycast, RaycastHit raycastHit)
    {
        if (raycastHit.collider.CompareTag("Cell"))
        {
            GameObject cell = raycastHit.collider.gameObject;
            //if (ViewCard != null)
            //{
            //    takeCard(cell.GetComponent<Cell>());
            //}
            //else
            //{
            //    selectCell(cell);
            //}
            //raycastHit.collider.gameObject.GetComponent<Cell>().SetType(3);
            cell.GetComponent<Cell>().Hold();
            Debug.Log("Cell Hold");
        }
        else
        {
            Debug.Log("Something Hold");
        }
    }

    void OnRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        switch (request.State)
        {
            // The request finished without any problem.
            case HTTPRequestStates.Finished:
                if (response.IsSuccess)
                {
                    Cards = JsonConvert.DeserializeObject<JsonToCards>(response.DataAsText);
                    
                    Debug.Log("Request Finished! Text received: " + response.DataAsText);
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
