using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Overflow.View
{
    public interface IView
    {
        public List<float> AddScoreValues { get; }
        public List<float> AddCostValues { get; }
        public ChannelReader<int> AddScoreChannelReader { get; }
        public ChannelReader<int> MenuChannelReader { get; }
        public ChannelReader<int> SaveChannelReader { get; }
        public int ButtonValue { get; }
        public void AutoSaveText();
        public void ScoreValue(float value);
        public void ScoreStock(float value);
        public void AddScoreValue(float value);
        public void AddButtonValue(int value, int x);
        public void ScreenChange(int t);
    }
}