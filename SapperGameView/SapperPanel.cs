using Saper.Model;
using System;
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

        Button[,] panelTile;

        public SapperPanelView()
        {
            startGameBtm = new Button();
            startGameBtm.SetValue(Canvas.TopProperty, 10);
            startGameBtm.SetValue(Canvas.LeftProperty, 10);

            startGameBtm.Height = 50;
            startGameBtm.Width = 150;
            startGameBtm.Content = "Start";
            startGameBtm.Click += RecreateGamePanel_EventHandler;

            startGUI = new Coordinate(10, 70);

            this.Loaded += SapperPanelView_Loaded;
        }

        private void SapperPanelView_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateNewPanel();
        }

        private void GenerateNewPanel()
        {
            panelTile = new Button[HorizontalTilesNumber, VerticalTilesNumber];

            for (int i = 0; i < VerticalTilesNumber; i++)
            {
                for (int j = 0; j < HorizontalTilesNumber; j++)
                {
                    panelTile[i, j] = new Button();
                    panelTile[i, j].Height = SquareTileSize;
                    panelTile[i, j].Width = SquareTileSize;

                    panelTile[i, j].SetValue(Canvas.TopProperty, startGUI.vertical + i * (SquareTileSize + 5));
                    panelTile[i, j].SetValue(Canvas.LeftProperty, startGUI.horizontal + j * (SquareTileSize + 5));

                    panelTile[i, j].Content = "?";
                    panelTile[i, j].FontSize -= 5;

                    panelTile[i, j].Click += SapperPanelView_Click;

                    Children.Add(panelTile[i, j]);
                }
            }
        }

        private void SapperPanelView_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
