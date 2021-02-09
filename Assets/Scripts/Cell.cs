using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int Type;
    public int Id;
    public int MirrorId;
    public Material ActiveMaterial;
    public Material EnemyMaterial;
    public Material ResourceMaterial;
    public GameObject CardObject;
    public GameObject Unit = null;

    private Color oldColor;
    private int defaultType;

    const int CELL_WITH_RESOURCE = 2;

    Material m_Material;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.parent.GetComponent<Board>().AddCell(this);
        if (Type == null)
        {
            Type = 1;
        }
        m_Material = GetComponent<Renderer>().material;
        defaultType = Type;
    }

    // Update is called once per frame
    void Update()
    {
        CheckType();
    }

    void CheckType()
    {
        switch (Type)
        {
            case 1:
                if (m_Material != null)
                {
                    GetComponent<Renderer>().material = m_Material;
                }
                break;
            case 2:
                if (ResourceMaterial != null)
                {
                    GetComponent<Renderer>().material = ResourceMaterial;
                }
                break;
            case 3:
                if (ActiveMaterial != null)
                {
                    GetComponent<Renderer>().material = ActiveMaterial;
                }
                break;
            case 4:
                if (EnemyMaterial != null)
                {
                    GetComponent<Renderer>().material = EnemyMaterial;
                }
                break;
            default:
                break;    
        }
    }

    public void SetType(int number = 1)
    {
        Type = number;
    }

    public Cell SelectCell()
    {
        return this;
    }

    public void replaceUnit(GameObject cell)
    {
        Vector3 a = GetComponent<Transform>().position;
        Vector3 b = cell.GetComponent<Transform>().position;
        Vector3 ab = new Vector3(b.x - a.x, b.y - a.y, b.z - a.z);

        double vectorLength = Math.Round(Math.Sqrt(Math.Pow(ab.x, 2) + Math.Pow(ab.y, 2) + Math.Pow(ab.z, 2)), 1);
        if (vectorLength != 0 && vectorLength <= 1.9)
        {
            cell.GetComponent<Cell>().SetUnit(Unit);

            Unit = null;
        }
    }

    public void SetUnit(GameObject unit)
    {
        Unit = unit;
        Unit.transform.SetParent(GetComponent<Transform>(), false);
        startAction();
    }

    public void PutCard(GameObject item)
    {
        if (Type == 3)
        {
            CardObject = item;
            
            Unit = GameObject.Find("Squad"); 
            Vector3 pos = new Vector3(0, Unit.GetComponent<Transform>().lossyScale.y, 0);
            GameObject SpawnedItem = Instantiate(Unit, pos, Quaternion.identity);
            Unit = SpawnedItem;
            Unit.transform.SetParent(GetComponent<Transform>(), false);
            Unit.GetComponent<UnitInfo>().Card = CardObject;
            Debug.Log("Put Card");
        }
    }

    public Card getCard()
    {
        return Unit.GetComponent<UnitInfo>().Card.GetComponent<Card>();
    }

    public void startAction()
    {
        if (Unit.GetComponent<UnitInfo>().Type == "squad")
        {
            SetType(3);
        }
    }

    public bool HasResource()
    {
        return defaultType == CELL_WITH_RESOURCE;
    }

    public void ComeHere()
    {
        Debug.Log("Come Here");
    }

    public void Hold()
    {
        Color color = GetComponent<Renderer>().material.color;
        oldColor = new Color(color.r, color.g, color.b, color.a);
        GetComponent<Renderer>().material.color = new Color(oldColor.r + 0.1f, oldColor.g + 0.1f, oldColor.b, oldColor.a);
    }

    public void HoldStop()
    {
        GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, oldColor.a);
    }

}
