using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [System.Serializable]
    public class Settings
    {
        public Image imageCard;
        public Vector2 cardSize;

        public void setCardSize(Vector2 size)
        {
            cardSize = size;
        }
    }

    public GameObject cardObject;

    public Settings preferences = new Settings();

    public int index, id, attack, heal, power, price;

    public string type, cardName, description, img;

    // Start is called before the first frame update
    void Start()
    {
        //preferences.cardSize = new Vector2(cardObject.GetComponent<RectTransform>().sizeDelta.x, cardObject.GetComponent<RectTransform>().sizeDelta.y);
        
    }

    // Update is called once per frame
    void Update()
    {
        CardState();
    }

    public void initCard()
    {
        cardObject.GetComponent<RectTransform>().sizeDelta = preferences.cardSize;
        price = 1;
    }

    public void setIndex(int value)
    {
        index = value;
    }

    public void setSize(Transform parent)
    {
        Vector2 size = parent.GetComponent<RectTransform>().sizeDelta;
        float X = ((size.x * 0.4f) * 0.3f);
        float Y = (size.y * 0.27f);
        preferences.setCardSize(new Vector2(X, Y));
        initCard();
        //Debug.Log(preferences.cardSize.x);
    }

    private void CardState()
    {
        Vector2 CardSize = cardObject.GetComponent<RectTransform>().sizeDelta;
        if (CardSize.x != preferences.cardSize.x || CardSize.y != preferences.cardSize.y)
        {
            initCard();
        }
    }
}
