﻿namespace SuperUser
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState) =>
            new Window(new AppShell())
            {
                Width = 700,
                Height = 1200,
                X = 100,
                Y = 100
            };
        }
}
