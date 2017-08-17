using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UISleuth.Tests.SurfaceManager
{
    public class TestPerson
    {
        public string Name { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Location { get; private set; }

        public TestPerson(string name, DateTime dob, string location)
        {
            Name = name;
            DateOfBirth = dob;
            Location = location;
        }
    }

    public class TestPersonDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ValidTemplate { get; set; }
        public DataTemplate InvalidTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((TestPerson)item).DateOfBirth.Year >= 1980 ? ValidTemplate : InvalidTemplate;
        }
    }

    public class TestDataSelectorPage : ContentPage
    {
        DataTemplate validTemplate;
        DataTemplate invalidTemplate;

        public ListView ListView { get; set; }

        public TestDataSelectorPage()
        {
            var people = new List<TestPerson> {
                new TestPerson ("Kath", new DateTime (1985, 11, 20), "France"),
                new TestPerson ("Steve", new DateTime (1975, 1, 15), "USA"),
                new TestPerson ("Lucas", new DateTime (1988, 2, 5), "Germany"),
                new TestPerson ("John", new DateTime (1976, 2, 20), "USA"),
                new TestPerson ("Tariq", new DateTime (1987, 1, 10), "UK"),
                new TestPerson ("Jane", new DateTime (1982, 8, 30), "USA"),
                new TestPerson ("Tom", new DateTime (1977, 3, 10), "UK")
            };

            SetupDataTemplates();

            ListView = new ListView
            {
                ItemsSource = people,
                ItemTemplate = new TestPersonDataTemplateSelector
                {
                    ValidTemplate = validTemplate,
                    InvalidTemplate = invalidTemplate
                }
            };

            Content = new StackLayout
            {
                Children = {
                    new Label {
                        Text = "ListView with a DataTemplateSelector",
                    },
                    ListView
                }
            };
        }

        void SetupDataTemplates()
        {
            validTemplate = new DataTemplate(() => {
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.4, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.3, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.3, GridUnitType.Star) });

                var nameLabel = new Label {};
                var dobLabel = new Label();
                var locationLabel = new Label();

                nameLabel.SetBinding(Label.TextProperty, "Name");
                dobLabel.SetBinding(Label.TextProperty, "DateOfBirth", stringFormat: "{0:d}");
                locationLabel.SetBinding(Label.TextProperty, "Location");
                nameLabel.TextColor = Color.Green;
                dobLabel.TextColor = Color.Green;
                locationLabel.TextColor = Color.Green;

                grid.Children.Add(nameLabel);
                grid.Children.Add(dobLabel, 1, 0);
                grid.Children.Add(locationLabel, 2, 0);

                return new ViewCell
                {
                    View = grid
                };
            });

            invalidTemplate = new DataTemplate(() => {
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.4, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.3, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.3, GridUnitType.Star) });

                var nameLabel = new Label { };
                var dobLabel = new Label();
                var locationLabel = new Label();

                nameLabel.SetBinding(Label.TextProperty, "Name");
                dobLabel.SetBinding(Label.TextProperty, "DateOfBirth", stringFormat: "{0:d}");
                locationLabel.SetBinding(Label.TextProperty, "Location");
                nameLabel.TextColor = Color.Red;
                dobLabel.TextColor = Color.Red;
                locationLabel.TextColor = Color.Red;

                grid.Children.Add(nameLabel);
                grid.Children.Add(dobLabel, 1, 0);
                grid.Children.Add(locationLabel, 2, 0);

                return new ViewCell
                {
                    View = grid
                };
            });
        }
    }

}
