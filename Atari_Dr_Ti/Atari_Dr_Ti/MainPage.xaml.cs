﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace Atari_Dr_Ti
{
    public partial class MainPage : ContentPage
    {
        List<Action> actions = new List<Action>();
        //List<Brick> bricks = new List<Brick>();
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
            this.screenHeight = Application.Current.MainPage.Height;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Timer timer = new Timer();
            timer.Interval = 5;
            timer.Enabled = true;
            timer.Start();

        }



    }
}

