using Saper.Model;
using System;
using System.Collections.Generic;
using Windows.UI;
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
            clockTextBox = new Clock(this, new Coordinate(10, 170));

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

        private void GenerateNewPanel()
        {
            panelTile = new List<Button>();

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
        }

        private void DestroyCurrentPanel()
        {
            if (panelTile != null)
            {
                foreach (var item in panelTile)
                {
                    Children?.Remove((UIElement)item);
                }
            }

            panelTile?.Clear();
        }

        private void DeactiveAllTiles()
        {
            foreach (var item in panelTile)
            {
                item.IsEnabled = false;
            }
        }

        #endregion

        #region Event handlers

        private void StartButton_Clicked(object sender, RoutedEventArgs e)
        {
            clockTextBox.Start();
        }

        private void SapperPanel_Loaded(object sender, RoutedEventArgs e)
        {
            GamePanel_Recreate(this, new RoutedEventArgs());

            DeactiveAllTiles();
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
                    }
                    else
                    {
                        List<Coordinate> lista = new List<Coordinate>();
                        panelLogic.UncoverZerosAndAdjacentIn(coordOfClicked, ref lista);


                        foreach (var item in lista)
                        {
                            panelTile[Trans2DTo1D(item.horizontal, item.vertical)].Content =
                                panelLogic.GetNumberOfAdjacentBombsIn(item).ToString();

                            panelTile[Trans2DTo1D(item.horizontal, item.vertical)].Background = this.Background;
                        }
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

        private void GamePanel_Recreate(object sender, RoutedEventArgs e)
        {
            try
            {
                ushort h = Convert.ToUInt16(HorizontalTilesNumber);
                ushort v = Convert.ToUInt16(VerticalTilesNumber);
                ushort p = Convert.ToUInt16(BombDensityPercent);

                panelLogic = new SapperGamePanelModel(h, v, p);
            }
            catch (Exception ex)
            {
                panelLogic = new SapperGamePanelModel(10, 10);
            }
            finally
            {
                DestroyCurrentPanel();
                GenerateNewPanel();
            }
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

        #endregion
    }
}
