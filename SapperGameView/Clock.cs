using Saper.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SapperGameView
{
    public class Clock
    {
        protected TextBlock clockText;

        protected DateTime startTime;
        protected TimeSpan currentTime;
        protected DispatcherTimer gameTimer;

        public Clock(Canvas ctx, Coordinate startPosition)
        {
            clockText = new TextBlock();
            clockText.SetValue(Canvas.TopProperty, 10);
            clockText.SetValue(Canvas.LeftProperty, 170);
            clockText.Text = "00:00:00.00";
            clockText.Height = 40;
            clockText.Width = 150;

            ctx.Children.Add(clockText);

            gameTimer = new DispatcherTimer();
            gameTimer.Tick += new EventHandler<object>(DoWork);
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
        }

        protected virtual void DoWork(object sender, object e)
        {
            currentTime = DateTime.Now - startTime;
            clockText.Text = currentTime.ToString(@"hh\:mm\:ss\.ff");
        }

        public void Start()
        {
            startTime = DateTime.Now;
            gameTimer.Start();
        }

        public void Stop(object sender, object e)
        {
            gameTimer.Stop();
        }
    }
}
