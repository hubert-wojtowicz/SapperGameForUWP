﻿using Saper.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SapperGameView
{
    public delegate void GameFinishedEvent(object sender, EventArgs e);

    public class SapperPanelView : Canvas
    {
        // event occurs when one round of game is finished
        public event GameFinishedEvent GameFinished;

        ISapperGamePanelOperations panelLogic;
        Coordinate StartDrawGamePanelAt = null;
        List<Button> panelTile;

        Button startBtn;
        Clock clockTextBox;

        public SapperPanelView()
        {
            clockTextBox = new Clock(this, new Coordinate(170, 15));

            startBtn = new Button();
            startBtn.SetValue(Canvas.TopProperty, 10);
            startBtn.SetValue(Canvas.LeftProperty, 10);
            startBtn.Content = "Start a game !";
            startBtn.Height = 40;
            startBtn.Width = 150;
            startBtn.Click += GamePanel_Recreate;
            startBtn.Click += StartButton_Clicked;
            Children.Add((UIElement)startBtn);


            StartDrawGamePanelAt = new Coordinate(TilesMargin, 70);

            this.Loaded += SapperPanel_Loaded;
        }

        #region Panel operations

        private void GenerateNewGamePanelAndModel()
        {
            panelTile = new List<Button>();
            panelLogic = new SapperGamePanelModel(
                Convert.ToUInt16(HorizontalTilesNumber),
                Convert.ToUInt16(VerticalTilesNumber),
                Convert.ToUInt16(BombDensityPercent));

            for (int j = 0; j < VerticalTilesNumber; j++)
            {
                for (int i = 0; i < HorizontalTilesNumber; i++)
                {
                    panelTile.Add(new Button());
                    int currentPos = Trans2DTo1D(i, j);
                    panelTile[currentPos].Height = SquareTileSize;
                    panelTile[currentPos].Width = SquareTileSize;

                    panelTile[currentPos].SetValue(Canvas.TopProperty, StartDrawGamePanelAt.vertical + j * (SquareTileSize + TilesMargin));
                    panelTile[currentPos].SetValue(Canvas.LeftProperty, StartDrawGamePanelAt.horizontal + i * (SquareTileSize + TilesMargin));

                    panelTile[currentPos].Content = "?";
                    panelTile[currentPos].FontSize -= 5;

                    panelTile[currentPos].Click += SapperTile_Clicked;

                    Children.Add(panelTile[currentPos]);
                }
            }

            DeactiveAllTiles();
        }

        private void DestroyCurrentGamePanelAndModel()
        {
            if (panelTile != null)
            {
                foreach (var item in panelTile)
                {
                    Children?.Remove((UIElement)item);
                }
            }

            panelTile?.Clear();

            panelLogic = null;

            clockTextBox.Stop(this, null);
        }

        private void DeactiveAllTiles()
        {
            foreach (var item in panelTile)
            {
                item.IsEnabled = false;
            }
        }

        private void ActivateAllTiles()
        {
            foreach (var item in panelTile)
            {
                item.IsEnabled = true;
            }
        }

        #endregion

        #region Event handlers

        private void StartButton_Clicked(object sender, RoutedEventArgs e)
        {
            ActivateAllTiles();
            clockTextBox.Start();
        }

        private void SapperPanel_Loaded(object sender, RoutedEventArgs e)
        {
            GamePanel_Recreate(this, new RoutedEventArgs());
            DeactiveAllTiles();
        }

        private async void ButtonShowMessageDialog_Click(object sender, RoutedEventArgs e)
        {
            clockTextBox.Stop(sender, e);

            var dialog = new Windows.UI.Popups.MessageDialog(
                        "You have uncovered all bombs in time. Congratulations! Do you want start new game?");

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Yes") { Id = 0 });
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("No") { Id = 1 });

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            if ((int)result.Id == 0)
            {
                DestroyCurrentGamePanelAndModel();
                GenerateNewGamePanelAndModel();
            }
            else
            {
                DeactiveAllTiles();
            }
        }

        private void SapperTile_Clicked(object sender, RoutedEventArgs e)
        {
            Button currentBtn = (Button)sender;

            int pos = panelTile.FindIndex(x => x == currentBtn);

            Coordinate coordOfClicked = Trans1DTo2D(pos);


            if (!panelLogic.IsFieldUncovered(coordOfClicked))
            {
                if (!panelLogic.IsBombInside(coordOfClicked))
                {
                    int numOfAdjBombs = panelLogic.GetNumberOfAdjacentBombsIn(coordOfClicked);

                    if (numOfAdjBombs > 0)
                    {
                        panelLogic.UncoverOneIn(coordOfClicked);
                        panelTile[Trans2DTo1D(coordOfClicked.horizontal, coordOfClicked.vertical)].Content = numOfAdjBombs.ToString();
                        panelTile[Trans2DTo1D(coordOfClicked.horizontal, coordOfClicked.vertical)].Background = this.Background;

                        panelTile[Trans2DTo1D(coordOfClicked.horizontal, coordOfClicked.vertical)].Foreground = AssignFontColorToUncovered(numOfAdjBombs);
                        panelTile[Trans2DTo1D(coordOfClicked.horizontal, coordOfClicked.vertical)].FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        List<Coordinate> lista = new List<Coordinate>();
                        panelLogic.UncoverZerosAndAdjacentIn(coordOfClicked, ref lista);


                        foreach (var item in lista)
                        {
                            int currentBombNum = panelLogic.GetNumberOfAdjacentBombsIn(item);

                            if (currentBombNum != 0)
                                panelTile[Trans2DTo1D(item.horizontal, item.vertical)].Content =
                                currentBombNum.ToString();
                            else
                                panelTile[Trans2DTo1D(item.horizontal, item.vertical)].Content = String.Empty;

                            panelTile[Trans2DTo1D(item.horizontal, item.vertical)].Background = this.Background;

                            panelTile[Trans2DTo1D(item.horizontal, item.vertical)].Foreground = AssignFontColorToUncovered(currentBombNum);
                            panelTile[Trans2DTo1D(item.horizontal, item.vertical)].FontWeight = FontWeights.Bold;
                        }
                    }

                    if (!panelLogic.LeftFieldsUncoveredWithoutBomb())
                    {
                        ButtonShowMessageDialog_Click(this, e);
                    }
                }
                else
                {
                    panelTile[Trans2DTo1D(coordOfClicked.horizontal, coordOfClicked.vertical)].Content = "*";
                    panelTile[Trans2DTo1D(coordOfClicked.horizontal, coordOfClicked.vertical)].Background = new SolidColorBrush(Colors.Red);
                    clockTextBox.Stop(null, null);
                    DeactiveAllTiles();

                    GameFinished?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void GamePanel_Recreate(object sender, RoutedEventArgs e)
        {
            DestroyCurrentGamePanelAndModel();
            GenerateNewGamePanelAndModel();
        }

        #endregion

        #region Dependency properties
        public int HorizontalTilesNumber
        {
            get { return (int)GetValue(HorizontalTilesNumberProperty); }
            set { SetValue(HorizontalTilesNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalTilesNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalTilesNumberProperty =
            DependencyProperty.Register("HorizontalTilesNumber", typeof(int), typeof(SapperPanelView), new PropertyMetadata(8));



        public int VerticalTilesNumber
        {
            get { return (int)GetValue(VerticalTilesNumberProperty); }
            set { SetValue(VerticalTilesNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalTilesNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalTilesNumberProperty =
            DependencyProperty.Register("VerticalTilesNumber", typeof(int), typeof(SapperPanelView), new PropertyMetadata(8));


        public int SquareTileSize
        {
            get { return (int)GetValue(SquareTileSizeProperty); }
            set { SetValue(SquareTileSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SquareTileSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SquareTileSizeProperty =
            DependencyProperty.Register("SquareTileSize", typeof(int), typeof(SapperPanelView), new PropertyMetadata(30));



        public int BombDensityPercent
        {
            get { return (int)GetValue(BombDensityPercentProperty); }
            set { SetValue(BombDensityPercentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BombDensityPercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BombDensityPercentProperty =
            DependencyProperty.Register("BombDensityPercent", typeof(int), typeof(SapperPanelView), new PropertyMetadata(20));



        public int TilesMargin
        {
            get { return (int)GetValue(TilesMarginProperty); }
            set { SetValue(TilesMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TilesMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TilesMarginProperty =
            DependencyProperty.Register("TilesMargin", typeof(int), typeof(SapperPanelView), new PropertyMetadata(3));


        #endregion

        #region Helper methods

        private int Trans2DTo1D(int horzontalCoordinate, int verticalCoordinate)
        {
            return HorizontalTilesNumber * verticalCoordinate + horzontalCoordinate;
        }

        private Coordinate Trans1DTo2D(int collectionPos)
        {
            return new Coordinate(collectionPos % HorizontalTilesNumber, collectionPos / HorizontalTilesNumber);
        }

        private SolidColorBrush AssignFontColorToUncovered(int numberOfNeighbours)
        {
            switch (numberOfNeighbours)
            {
                case 1:
                    return new SolidColorBrush(Colors.Blue);
                case 2:
                    return new SolidColorBrush(Colors.Green);
                case 3:
                    return new SolidColorBrush(Colors.Orange);
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.Black);
            }
        }

        #endregion
    }
}
