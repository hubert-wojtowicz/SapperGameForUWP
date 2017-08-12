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
            DependencyProperty.Register("HorizontalTilesNumber", typeof(int), typeof(SapperPanelView), new PropertyMetadata(200));



        public int VerticalTilesNumber
        {
            get { return (int)GetValue(VerticalTilesNumberProperty); }
            set { SetValue(VerticalTilesNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalTilesNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalTilesNumberProperty =
            DependencyProperty.Register("VerticalTilesNumber", typeof(int), typeof(SapperPanelView), new PropertyMetadata(200));


        public int SquareTileSize
        {
            get { return (int)GetValue(SquareTileSizeProperty); }
            set { SetValue(SquareTileSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SquareTileSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SquareTileSizeProperty =
            DependencyProperty.Register("SquareTileSize", typeof(int), typeof(SapperPanelView), new PropertyMetadata(5));



        public int BombDensityPercent
        {
            get { return (int)GetValue(BombDensityPercentProperty); }
            set { SetValue(BombDensityPercentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BombDensityPercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BombDensityPercentProperty =
            DependencyProperty.Register("BombDensityPercent", typeof(int), typeof(SapperPanelView), new PropertyMetadata(0));

        #endregion

        Button startGameBtm;

        ISapperGamePanelOperations panelLogic;

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
                panelLogic = new SapperGamePanelModel(10,10);
            }

            ((Button)sender).Content = $"h={panelLogic.GetHorizontalSize}; v={panelLogic.GetVerticalSize}; p={panelLogic.GetBombDensityPercent}";
        }
    }
}
