using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Energy
{

    private int _amount = 0;
    public int Amount
    {
        get => _amount;
        set
        {
            _amount = value;

            Notify?.Invoke("SetEnergy");
        }
    }
    private int _maxEnergy = 0;
    public int MaxEnergy
    {
        get => _maxEnergy;
        set
        {
            _maxEnergy = value;

            Notify?.Invoke("SetEnergy");
        }
    }
    public event Action<string> Notify;

    public Energy()
    {
        initEnergy();
    }

    void initEnergy()
    {
        MaxEnergy = 2;
        Amount = MaxEnergy;
    }

    public void addMax(int amount)
    {
        MaxEnergy = MaxEnergy + amount;
    }

    public void useEnergy(int amount)
    {
        int result = Amount - amount;
        if (result >= 0)
        {
            Amount = result;
        }
    }

    public void Reset()
    {
        Amount = MaxEnergy;
    }

    public bool CheckPrice(int amount)
    {
        return (Amount - amount) >= 0;
    }
}
