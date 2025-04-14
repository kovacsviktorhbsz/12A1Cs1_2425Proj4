using _4.projektmunka.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Videogames.Database;

namespace Videogames
{
    public partial class MainWindow : Window
    {
        private VideoGamesContext ctx = new VideoGamesContext();
        public ObservableCollection<Game> Games { get; set; }
        private CollectionView view;

        public MainWindow()
        {
            InitializeComponent();

            Games = new ObservableCollection<Game>(ctx.Games
                .Include(g => g.Platforms)
                .Include(g => g.Reviews)
                .Include(g => g.Developer)
                .ToList());
            GamesListBox.ItemsSource = Games;
            GamesListBox.DisplayMemberPath = "Title";

            view = (CollectionView)CollectionViewSource.GetDefaultView(GamesListBox.ItemsSource);
            view.Filter = GameFilter;
        }

        private bool GameFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(tbTitle.Text))
                return true;

            var game = item as Game;
            return game.Title.ToLower().Contains(tbTitle.Text.ToLower());
        }

        private void tbTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (view != null)
                view.Refresh();
        }

        private void GameToFields(Game game)
        {
            if (game == null)
                return;

            tbId.Text = game.GameID.ToString();
            tbTitle.Text = game.Title;
            tbReleaseYear.Text = game.ReleaseYear.ToString();
            tbDeveloper.Text = game.Developer?.Name ?? "N/A";
            tbCountry.Text = game.Developer?.Country ?? "N/A";
            tbPlatform.Text = game.Platforms.Any()
                ? string.Join(", ", game.Platforms.Select(p => p.PlatformName))
                : "N/A";
            tbReview.Text = game.Reviews.Any()
                ? string.Join("\n", game.Reviews.Select(r => r.Comment))
                : "N/A";
        }

        private Game FieldsToGames()
        {
            int year = 0;
            if (tbReleaseYear.Text != "")
                year = int.Parse(tbReleaseYear.Text);

            var developer = ctx.Developers.FirstOrDefault(x => x.Country == tbCountry.Text);
            if (developer == null && !string.IsNullOrWhiteSpace(tbDeveloper.Text) && !string.IsNullOrWhiteSpace(tbCountry.Text))
            {
                developer = new Developer { Name = tbDeveloper.Text, Country = tbCountry.Text };
                ctx.Developers.Add(developer);
                ctx.SaveChanges();
            }

            var platformNames = tbPlatform.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            var platforms = ctx.Platforms.Where(p => platformNames.Contains(p.PlatformName)).ToList();

            foreach (var platformName in platformNames)
            {
                if (!platforms.Any(p => p.PlatformName == platformName))
                {
                    var newPlatform = new Platform { PlatformName = platformName };
                    ctx.Platforms.Add(newPlatform);
                    platforms.Add(newPlatform);
                }
            }
            ctx.SaveChanges(); 

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
            Games.Clear();
            foreach (var game in ctx.Games
                .Include(g => g.Platforms)
                .Include(g => g.Reviews)
                .Include(g => g.Developer)
                .ToList())
            {
                Games.Add(game);
            }
            view?.Refresh();
        }

        private void GamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGame = (Game)GamesListBox.SelectedItem;
            if (selectedGame != null)
            {
                GameToFields(selectedGame);
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var game = FieldsToGames();
            ctx.Games.Add(game);
            ctx.SaveChanges();
            RefreshListBox(); 
            GamesListBox.SelectedItem = game; 
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(tbId.Text, out int id)) return;

            var fields = FieldsToGames();
            var gameToUpdate = ctx.Games
                .Include(g => g.Platforms)
                .Include(g => g.Reviews)
                .Include(g => g.Developer)
                .FirstOrDefault(x => x.GameID == id);

            if (gameToUpdate != null)
            {
                gameToUpdate.Title = fields.Title;
                gameToUpdate.ReleaseYear = fields.ReleaseYear;

                if (fields.Developer != null)
                {
                    gameToUpdate.Developer = fields.Developer;
                    gameToUpdate.DeveloperID = fields.Developer.DeveloperID;
                }

                gameToUpdate.Platforms.Clear();
                foreach (var platform in fields.Platforms)
                {
                    gameToUpdate.Platforms.Add(platform);
                }

                if (fields.Reviews.Any())
                {
                    if (gameToUpdate.Reviews.Any())
                    {
                        gameToUpdate.Reviews.First().Comment = fields.Reviews.First().Comment;
                        gameToUpdate.Reviews.First().Rating = fields.Reviews.First().Rating;
                        gameToUpdate.Reviews.First().UserName = fields.Reviews.First().UserName;
                    }
                    else
                    {
                        gameToUpdate.Reviews.Add(fields.Reviews.First());
                    }
                }

                ctx.SaveChanges();
                RefreshListBox();
                GamesListBox.SelectedItem = Games.FirstOrDefault(g => g.GameID == id);
            }
            else
            {
                MessageBox.Show("Nincs ilyen azonosítóval játék!");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(tbId.Text, out int id)) return;

            var gameToDelete = ctx.Games.FirstOrDefault(x => x.GameID == id);
            if (gameToDelete != null)
            {
                ctx.Games.Remove(gameToDelete);
                ctx.SaveChanges();
                Games.Remove(gameToDelete);
                view.Refresh();
                GameToFields(null);
            }
            else
            {
                MessageBox.Show("Nincs ilyen azonosítóval játék!");
            }
        }
    }
}