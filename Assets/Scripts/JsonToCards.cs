using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class JsonToCards
    {
        public JPagination pagination { get; set; }
        public List<MyCard> data { get; set; }

        public MyCard GetCard(int id)
        {
            MyCard found = data.Find(item => item.id == id);
            data.Remove(found);
            return found;
        }
        public int GetCardsCount()
        {
            return data.Count;
        }
    }
}