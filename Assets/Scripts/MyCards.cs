using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCards : MonoBehaviour
{

    [SerializeField]
    private Transform SpawnPoint = null;

    //[SerializeField]
    //private GameObject item = null;

    [SerializeField]
    private RectTransform content = null;

    [SerializeField]
    private int numberOfItems = 3;
    [SerializeField]
    private List<GameObject> Cards;

    public GameObject gameController;

    public string[] itemNames = null;
    public Sprite[] itemImages = null;

    // Use this for initialization
    void Start()
    {

        //setContent Holder Height;
        
    }

    public List<GameObject> RenderCards(List<GameObject> items)
    {
        //Debug.Log(items.Count);
        
        //content.sizeDelta = new Vector2(0, items.Count * 60);
        int i = 0;
        foreach (GameObject item in items)
        {
            //Debug.Log(item.GetComponent<RectTransform>().sizeDelta.y);
            item.GetComponent<Card>().setSize(gameController.GetComponent<GameController>().InterfaceObject.transform);
            float difference = item.GetComponent<RectTransform>().sizeDelta.x - content.sizeDelta.y;
            // 60 width of item
            float spawnX = (i * item.GetComponent<RectTransform>().sizeDelta.x) + (item.GetComponent<RectTransform>().sizeDelta.x * 0.2f);
            //newSpawn Position
            Vector3 pos = new Vector3(spawnX, -(item.GetComponent<RectTransform>().sizeDelta.y * 0.6f), 0);
            //instantiate item
            GameObject SpawnedItem = Instantiate(item, pos, Quaternion.identity);
            //setParent
            SpawnedItem.transform.SetParent(SpawnPoint, false);
            //get ItemDetails Component
            Card itemDetails = SpawnedItem.GetComponent<Card>();
            SpawnedItem.GetComponent<Card>().setIndex(i);
            SpawnedItem.GetComponent<Button>().onClick.AddListener(() => gameController.GetComponent<GameController>().DropCard(SpawnedItem.GetComponent<Card>().index));
            //Debug.Log(SpawnedItem.GetComponent<RectTransform>().sizeDelta.y);
            //set name
            //itemDetails.text.text = itemNames[i];
            //set image
            //itemDetails.image.sprite = itemImages[i];
                
            Cards.Add(SpawnedItem);
            SpawnedItem.SetActive(true);
            i++;
        }

        return Cards;
    }
}