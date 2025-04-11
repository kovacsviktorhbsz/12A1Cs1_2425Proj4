using _4.projektmunka.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
        private VideoGamesContext ctx = new VideoGamesContext();
        public ObservableCollection<Game> Games { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Games = new ObservableCollection<Game>(ctx.Games.ToList());
            GamesListBox.ItemsSource = Games;
        }
        private void GameToFields(Game game)
        {
            if (game == null)
                return;

            var gameWithDetails = ctx.Games
                .Include(g => g.Platforms)   
                .Include(g => g.Reviews)     
                .FirstOrDefault(g => g.GameID == game.GameID);  

            if (gameWithDetails == null)
            {
                tbPlatform.Text = "N/A";
                tbReview.Text = "N/A";
                return;
            }

            tbId.Text = gameWithDetails.GameID.ToString();
            tbTitle.Text = gameWithDetails.Title;
            tbReleaseYear.Text = gameWithDetails.ReleaseYear.ToString();
            tbDeveloper.Text = gameWithDetails.Developer?.Name ?? "N/A";
            tbCountry.Text = gameWithDetails.Developer?.Country ?? "N/A";
            tbPlatform.Text = gameWithDetails.Platforms.Any() ? string.Join(", ", gameWithDetails.Platforms.Select(p => p.PlatformName)) : "N/A";
            tbReview.Text = gameWithDetails.Reviews.Any() ? string.Join("\n", gameWithDetails.Reviews.Select(r => r.Comment)) : "N/A";
        }


        private Game FieldsToGames()
        {
            int year = 0;
            if (tbReleaseYear.Text != "")
                year = int.Parse(tbReleaseYear.Text);

            var developer = ctx.Developers
                         .FirstOrDefault(x => x.Country == tbCountry.Text);

            var platforms = ctx.Platforms
                         .Where(p => tbPlatform.Text.Contains(p.PlatformName)) 
                         .ToList();

            var review = new Review
            {
                Rating = 8, 
                Comment = tbReview.Text,
                UserName = "admin"
            };

            var game = new Game
            {
                Title = tbTitle.Text,
                ReleaseYear = year,
                Developer = developer,
                DeveloperID = developer?.DeveloperID ?? 0,
                Reviews = new List<Review> { review },
                Platforms = platforms
            };
            return game;
        }

        private void RefreshListBox()
        {
            Games = new ObservableCollection<Game>(ctx.Games.ToList());
            GamesListBox.ItemsSource = Games;
        }

        private void GamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGame = (Game)GamesListBox.SelectedItem;
            GameToFields(selectedGame);
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var game = FieldsToGames();
            ctx.Games.Add(game);
            ctx.SaveChanges();
            Games.Add(game);  
            GameToFields(game);
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var id = int.Parse(tbId.Text);
            var fields = FieldsToGames();
            var game = ctx.Games.Where(x => x.GameID == id).FirstOrDefault();

            if (game != null)
            {
                game.Title = fields.Title;
                game.ReleaseYear = fields.ReleaseYear;
                game.Developer = fields.Developer;
                game.Developer.Country = fields.Developer.Country;
                game.Platforms = fields.Platforms;
                game.Reviews = fields.Reviews;
                ctx.SaveChanges();
                GamesListBox.ItemsSource = new ObservableCollection<Game>(ctx.Games.ToList()); // Automatikusan frissíti a ListBox-ot
            }
            else
            {
                MessageBox.Show("Nincs ilyen azonosítóval film eltárolva!");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var id = int.Parse(tbId.Text);
            var game = ctx.Games.Where(x => x.GameID == id).FirstOrDefault();

            if (game != null)
            {
                ctx.Games.Remove(game);
                ctx.SaveChanges();
                RefreshListBox();
            }
            else
            {
                MessageBox.Show("Nincs ilyen azonosítóval játék eltárolva!");
            }
        }

        private void tbTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            var title = tbTitle.Text;
            var res = ctx.Games.Where(x => x.Title.Contains(title)).ToList();

            Games.Clear();
            foreach (var item in res)
            {
                Games.Add(item);
            }
        }
    }
}
