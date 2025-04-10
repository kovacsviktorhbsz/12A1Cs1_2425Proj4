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
                    GamesListBox.Items.Add(item.Title);
                }
            }
        }
        private void MovieToFields(Movie movie)
        {
            if (movie == null)
                return;
            tbId.Text = movie.Id.ToString();
            tbTitle.Text = movie.Title;
            tbYear.Text = movie.Year.ToString();
            tbDirector.Text = movie.Director;
            tbCountry.Text = movie.Country.Name;
            tbLanguage.Text = movie.Language;
            tbRuntime.Text = movie.Runtime.ToString();
        }

        private Movie FieldsToMovie()
        {
            int year = 0;
            int runtime = 0;
            if (tbYear.Text != "")
                year = int.Parse(tbYear.Text);
            if (tbRuntime.Text != "")
                runtime = int.Parse(tbRuntime.Text);

            var country = ctx.Countries.Where(x => x.Name == tbCountry.Text)
                                        .FirstOrDefault();

            var movie = new Movie
            {
                Title = tbTitle.Text,
                Year = year,
                Director = tbDirector.Text,
                CountryId = country.Id,
                Country = country,
                Language = tbLanguage.Text,
                Runtime = runtime
            };
            return movie;
        }

        private void RefreshListBox()
        {
            lbMovieList.Items.Clear();
            foreach (var item in ctx.Movies)
            {
                lbMovieList.Items.Add(item.Title);
            }
        }

        private void lbMovieList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var title = (string)lbMovieList.SelectedItem;
            var movie = ctx.Movies
                .Include(x => x.Country)
                .Where(x => x.Title == title).FirstOrDefault();
            MovieToFields(movie);
        }

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            //RefreshListBox();
            //var id = int.Parse(tbId.Text);
            //var fields = FieldsToMovie();
            /*var movie = ctx.Movies
                           .Include(x => x.Country)
                           .Where(x => x.Id == id).FirstOrDefault();*/
            //MovieToFields(movie);

            var title = tbTitle.Text;
            var director = tbDirector.Text;
            var language = tbLanguage.Text;
            var country = tbCountry.Text;

            var res = ctx.Movies.Include(x => x.Country)
                                .Where(x => x.Title.Contains(title) &&
                                            x.Director.Contains(director) &&
                                            x.Language.Contains(language) &&
                                            x.Country.Name.Contains(country)).ToList();

            if (tbYear.Text != "")
            {
                var year = int.Parse(tbYear.Text);
                res = res.Where(x => x.Year == year).ToList();
            }

            if (tbMin.Text != "" && tbMax.Text != "")
            {
                var min = int.Parse(tbMin.Text);
                var max = int.Parse(tbMax.Text);
                res = res.Where(x => x.Runtime > min && x.Runtime < max).ToList();
            }

            lbMovieList.Items.Clear();
            foreach (var item in res)
                lbMovieList.Items.Add(item.Title);

        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var movie = FieldsToMovie();
            movie = ctx.Movies.Add(movie);
            ctx.SaveChanges();
            if (movie != null)
            {
                MovieToFields(movie);
                RefreshListBox();
            }

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var id = int.Parse(tbId.Text);
            var fields = FieldsToMovie();

            var movie = ctx.Movies.Where(x => x.Id == id).FirstOrDefault();
            if (movie != null)
            {
                if (fields.Title != "")
                    movie.Title = fields.Title;
                if (fields.Year != 0)
                    movie.Year = fields.Year;
                if (fields.Director != "")
                    movie.Director = fields.Director;
                if (fields.Country != null)
                    movie.Country = fields.Country;
                if (fields.Language != "")
                    movie.Language = fields.Language;
                if (fields.Runtime != 0)
                    movie.Runtime = fields.Runtime;
                ctx.SaveChanges();
                RefreshListBox();
            }
            else
            {
                MessageBox.Show("Nincs ilyen azonosítóval film az adatbázisban.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var id = int.Parse(tbId.Text);
            var movie = ctx.Movies.Where(x => x.Id == id).FirstOrDefault();

            if (movie != null)
            {
                ctx.Movies.Remove(movie);
                ctx.SaveChanges();
                RefreshListBox();
            }
            else
            {
                MessageBox.Show("Nincs ilyen azonosítóval film eltárolva!");
            }
        }

        private void tbTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            var title = tbTitle.Text;
            var res = ctx.Movies.Where(x => x.Title.Contains(title)).ToList();

            lbMovieList.Items.Clear();
            foreach (var item in res)
                lbMovieList.Items.Add(item.Title);
        }
    }
}
