using UnityEngine;
using UnityEditor;

namespace Assets.Scripts
{
    public class JPagination
    {
        public int total { get; set; }
        public int perPage { get; set; }
        public int page { get; set; }
        public int lastPage { get; set; }
    }
}