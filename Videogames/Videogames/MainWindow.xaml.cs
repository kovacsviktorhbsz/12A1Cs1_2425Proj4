using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Videogames.Database;

namespace Videogames
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            using (var context = new VideoGamesContext())
            {
                var games = context.Games.ToList();
                foreach(var item in games)
                {
                    GamesListbox.Items.Add(item.Title);
                }
            }
        }

        /*private void LoadGamesButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new VideoGamesContext())
            {
                var games = context.Games.ToList();
                GamesListBox.Items.Add = games;
            }
        }*/

        private void GamesListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
