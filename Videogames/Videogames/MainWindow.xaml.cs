using _4.projektmunka.Models;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
            {
                foreach(var item in ctx.Games)
                {
                    GamesListBox.Items.Add(item.Title);
                }
            }
        }
        private void GameToFields(Game game)
        {
            if (game == null)
                return;

            // Lekérdezed az adatbázisból a játékot, platformokat és véleményeket
            var gameWithDetails = ctx.Games
                .Include(g => g.Platforms)   // Betölti a kapcsolódó platformokat
                .Include(g => g.Reviews)     // Betölti a kapcsolódó véleményeket
                .FirstOrDefault(g => g.GameID == game.GameID);  // Az aktuálisan kiválasztott játék

            // Ha nem találod a játékot, akkor visszatérsz
            if (gameWithDetails == null)
            {
                tbPlatform.Text = "N/A";
                tbReview.Text = "N/A";
                return;
            }

            // Ha sikerült betölteni, beállítod a mezőket
            tbId.Text = gameWithDetails.GameID.ToString();
            tbTitle.Text = gameWithDetails.Title;
            tbReleaseYear.Text = gameWithDetails.ReleaseYear.ToString();
            tbDeveloper.Text = gameWithDetails.Developer?.Name ?? "N/A";
            tbCountry.Text = gameWithDetails.Developer?.Country ?? "N/A";

            // Platformok betöltése
            tbPlatform.Text = gameWithDetails.Platforms.Any() ? string.Join(", ", gameWithDetails.Platforms.Select(p => p.PlatformName)) : "N/A";

            // Vélemények betöltése
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
            GamesListBox.Items.Clear();
            foreach (var item in ctx.Games)
            {
                GamesListBox.Items.Add(item.Title);
            }
        }

        private void GamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var title = (string)GamesListBox.SelectedItem;

            var game = ctx.Games
                .Include(x => x.Developer)
                .Where(x => x.Title == title)
                .FirstOrDefault();

            GameToFields(game);
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var game = FieldsToGames();
            game = ctx.Games.Add(game);
            ctx.SaveChanges();
            if (game != null)
            {
                GameToFields(game);
                RefreshListBox();
            }

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var id = int.Parse(tbId.Text);
            var fields = FieldsToGames();

            var game = ctx.Games.Where(x => x.GameID == id).FirstOrDefault();
            if (game != null)
            {
                if (fields.Title != "")
                    game.Title = fields.Title;
                if (fields.ReleaseYear != 0)
                    game.ReleaseYear = fields.ReleaseYear;
                if (fields.Developer != null)
                    game.Developer = fields.Developer;
                if (!string.IsNullOrEmpty(fields.Developer.Country))
                    game.Developer.Country = fields.Developer.Country;
                if (fields.Platforms != null && fields.Platforms.Any())
                    game.Platforms = fields.Platforms;
                if (fields.Reviews != null && fields.Reviews.Any())
                    game.Reviews = fields.Reviews;
                ctx.SaveChanges();
                RefreshListBox();
            }
            else
            {
                MessageBox.Show("Nincs ilyen azonosítóval játék az adatbázisban.");
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

            GamesListBox.Items.Clear();
            foreach (var item in res)
                GamesListBox.Items.Add(item.Title);
        }
    }
}
