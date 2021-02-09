using UnityEngine;
using UnityEditor;

namespace Assets.Scripts
{
    public class CardBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string cardType { get; set; }
        public string fraction { get; set; }
        public float attack { get; set; }
        public float heal { get; set; }
        public float power { get; set; }
        public float price { get; set; }
        
    }
}