using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SapperGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void RootSplitView_PaneClosed(SplitView sender, object args)
        {
            GameField.HorizontalTilesNumber = (int)HorSlider.Value;
            GameField.VerticalTilesNumber = (int)VertSlider.Value;
            GameField.BombDensityPercent = (int)BombDens.Value;

            GameField.GamePanel_Recreate(sender, new RoutedEventArgs());
        }

        private void RootSplitView_Loading(FrameworkElement sender, object args)
        {
            HorSlider.Value = GameField.HorizontalTilesNumber;
            VertSlider.Value = GameField.VerticalTilesNumber;
            BombDens.Value = GameField.BombDensityPercent;
        }
    }
}
