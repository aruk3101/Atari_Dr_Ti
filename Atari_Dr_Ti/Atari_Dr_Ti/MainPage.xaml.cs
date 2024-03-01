using System;
using System.Collections.Generic;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Atari_Dr_Ti
{
    public partial class MainPage : ContentPage
    {
        List<Action> actions = new List<Action>();
        List<Brick> bricks = new List<Brick>();
        double screenWidth;
        double screenHeight;

        double objectWidth = 20;
        double objectHeight = 20;
        int stepx = 1, stepy = 1;

        Rectangle currentBallBounds;
        Rectangle playerBounds;

        public MainPage()
        {
            InitializeComponent();
            this.SizeChanged += MainPage_SizeChanged;
        }

        private void MainPage_SizeChanged(object sender, EventArgs e)
        {
            this.screenHeight = Application.Current.MainPage.Height;
            this.screenWidth = Application.Current.MainPage.Width;
            GenerateBricks();
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            if (playerBounds != null && playerBounds.Y != 0)
            {
                float accX = -e.Reading.Acceleration.X;
                Console.WriteLine(accX);
                if (accX < 0)
                {
                    if (playerBounds.X < 5)
                        accX = -accX;
                    playerBounds.X += Math.Floor(accX) * 5;
                }
                else
                {
                    if (playerBounds.X + playerBounds.Width > screenWidth - 5) accX = -accX;
                    playerBounds.X += Math.Ceiling(accX) * 5;
                }
                AbsoluteLayout.SetLayoutBounds(player, playerBounds);
            }
            Console.WriteLine(playerBounds.X);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                foreach (var action in actions)
                {
                    action.Invoke();
                }
            });
        }

        private void GenerateBricks()
        {
            int no = (int)(screenWidth / 50);
            for (int i = (int)(screenWidth - (no * 50)) / 2; i <= screenWidth - 50; i += 50)
            {
                for (int j = 0; j < 60; j += 20)
                {
                    Brick b = new Brick();
                    layout.Children.Add(b);
                    AbsoluteLayout.SetLayoutBounds(b, new Rectangle(i, j, 50, 20));
                    bricks.Add(b);
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Timer timer = new Timer();
            timer.Interval = 10;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            Accelerometer.Start(SensorSpeed.UI);

            actions.Add(() =>
            {
                currentBallBounds = AbsoluteLayout.GetLayoutBounds(ball);
                playerBounds = AbsoluteLayout.GetLayoutBounds(player);
            });

            Action ballMoving = new Action(() =>
            {
                currentBallBounds.X += stepx;
                currentBallBounds.Y += stepy;
                AbsoluteLayout.SetLayoutBounds(ball, currentBallBounds);
            });
            actions.Add(ballMoving);

            actions.Add(() =>
            {
                if (currentBallBounds.Y + currentBallBounds.Height >= screenHeight - 20)
                {
                    DisplayAlert("Przegrana", "Przykro mi, nie udało ci się wygrać!", "OK");
                    timer.Stop();
                }
                if (bricks.Count == 0)
                {
                    DisplayAlert("Wygrana", "Gratulacje, wygrałeś naszą grę!", "OK");
                    timer.Stop();
                }
            });

            actions.Add(() =>
            {
                List<Brick> toRemove = new List<Brick>();
                for (int i = 0; i < bricks.Count; i++)
                {
                    Brick b = bricks[i];
                    if (AbsoluteLayout.GetLayoutBounds(b).IntersectsWith(currentBallBounds))
                    {
                        stepy = -stepy;
                        toRemove.Add(b);
                        break;
                    }
                }
                foreach (Brick b in toRemove)
                {
                    layout.Children.Remove(b);
                    bricks.Remove(b);
                }

            });

            int padding = 5;
            actions.Add(new Action(() =>
            {
                if (currentBallBounds.X + padding < 0
                        || currentBallBounds.X + objectWidth + padding > screenWidth) stepx = -stepx;
                if (currentBallBounds.Y + padding < 0
                        || currentBallBounds.Y + padding + objectHeight > screenHeight) stepy = -stepy;
            }));

            actions.Add(new Action(() =>
            {
                Rectangle playerBounds = AbsoluteLayout.GetLayoutBounds(player);
                if (currentBallBounds.IntersectsWith(playerBounds))
                {
                    stepy = -stepy;
                }
            }));
        }
        }
}

