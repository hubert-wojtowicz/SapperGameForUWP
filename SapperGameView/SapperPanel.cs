using Saper.Model;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SapperGameView
{
    public class SapperPanelView : Canvas
    {

        #region dependency properties
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

        #endregion

        Button startGameBtm;
        TextBlock ex = new TextBlock();
        Coordinate startGUI = null;

        ISapperGamePanelOperations panelLogic;

        List<Button> panelTile;


        public SapperPanelView()
        {
            startGameBtm = new Button();
            startGameBtm.SetValue(Canvas.TopProperty, 10);
            startGameBtm.SetValue(Canvas.LeftProperty, 10);

            startGameBtm.Height = 50;
            startGameBtm.Width = 150;
            startGameBtm.Content = "Start";
            startGameBtm.Click += RecreateGamePanel_EventHandler;

            Children.Add(startGameBtm);

            startGUI = new Coordinate(10, 70);

            this.Loaded += SapperPanelView_Loaded;
        }

        private void SapperPanelView_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateNewPanel();
        }

        private int Trans2DTo1D(int horzontalCoordinate, int verticalCoordinate)
        {
            return HorizontalTilesNumber * verticalCoordinate + horzontalCoordinate;
        }

        private Coordinate Trans1DTo2D(int collectionPos)
        {
            return new Coordinate(collectionPos % HorizontalTilesNumber, collectionPos / HorizontalTilesNumber);
        }

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

                    panelTile[currentPos].SetValue(Canvas.TopProperty, startGUI.vertical + j * (SquareTileSize + 5));
                    panelTile[currentPos].SetValue(Canvas.LeftProperty, startGUI.horizontal + i * (SquareTileSize + 5));

                    panelTile[currentPos].Content = "?";
                    panelTile[currentPos].FontSize -= 5;

                    panelTile[currentPos].Click += SapperPanelView_Click;

                    Children.Add(panelTile[currentPos]);
                }
            }
        }

        private void destroyCurrentPanel()
        {
            Children.Clear();
            panelTile.Clear();
        }

        private void SapperPanelView_Click(object sender, RoutedEventArgs e)
        {
            Button currentBtn = (Button)sender;

            int pos = panelTile.FindIndex(x => x == currentBtn);

            Coordinate cord = Trans1DTo2D(pos);

            startGameBtm.Content = $"({cord.horizontal},{cord.vertical})";

        }

        private void RecreateGamePanel_EventHandler(object sender, RoutedEventArgs e)
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

            ((Button)sender).Content = $"h={panelLogic.GetHorizontalSize}; v={panelLogic.GetVerticalSize}; p={panelLogic.GetBombDensityPercent}";
        }
    }
}
