using UnityEngine;
using UnityEditor;

namespace Assets.Scripts
{
    public class MyCard
    {
        public CardBase card { get; set; }
        public Deck deck { get; set; }
        public int id { get; set; }
    }
}