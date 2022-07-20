using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Overflow.Model
{
    public class ScoreData : MonoBehaviour
    {
        public AsyncReactiveProperty<float> Score;
        public AsyncReactiveProperty<int> StockScore;
        public AsyncReactiveProperty<float> IncrementValue;
        public AsyncReactiveProperty<int> AddButtonComprehensiveValue;
        public List<AsyncReactiveProperty<int>> AddButtonValue;
    }
}