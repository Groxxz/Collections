using System.Collections.ObjectModel;
using System.Collections;
using System.Diagnostics;

namespace Collections;

    public partial class MainPage : ContentPage
    {
        private string collectionFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Collections");
        public ObservableCollection<CollectionPage> Collections = new();

        public MainPage()
        {
            InitializeComponent();

            if (!Directory.Exists(collectionFolderPath))
                Directory.CreateDirectory(collectionFolderPath);

            LoadCollections();
            collections.ItemsSource = Collections;

            Debug.WriteLine(collectionFolderPath);
        }

        private async void AddCollectionButton_Clicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Tworzenie", "Podaj nazwę nowej kolekcji", "Stwórz", "Anuluj");

            if (name != null && name != "Anuluj")
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    CollectionPage newCollection = new()
                    {
                        Name = name
                    };

                    SaveCollectionFile(newCollection);
                    Collections.Add(newCollection);
                    await Navigation.PushAsync(newCollection);
                }
                else
                {
                    await DisplayAlert("Błąd", "Nazwa kolekcji nie może być pusta.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Info", "Tworzenie kolekcji anulowane.", "OK");
            }
    }

    private void DeleteCollectionButton_Clicked(object sender, EventArgs e)
        {
            CollectionPage selectedCollection = collections.SelectedItem as CollectionPage;

            if (selectedCollection != null)
            {
                DeleteCollectionFile(selectedCollection);
                Collections.Remove(selectedCollection);
            }

        }

        private async void GoToCollectionButton_Clicked(object sender, EventArgs e)
        {
            if (collections.SelectedItem != null)
                await Navigation.PushAsync(new CollectionPage() { Name = (collections.SelectedItem as CollectionPage).Name });
        }

        private void SaveCollectionFile(CollectionPage collection)
        {
            string filePath = Path.Combine(collectionFolderPath, $"{collection.Name}.txt");
            try
            {
                File.Create(filePath).Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w zapisywaniu kolekcji do pliku: {ex.Message}");
            }
        }

        private void DeleteCollectionFile(CollectionPage collection)
        {
            string filePath = Path.Combine(collectionFolderPath, $"{collection.Name}.txt");
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd w usuwaniu kolekcji: {ex.Message}");
                }
            }
        }

        private void LoadCollections()
        {
            var files = Directory.GetFiles(collectionFolderPath);

            foreach (string file in files)
            {
                var fileName = Path.GetFileName(file).Split(".")[0];
                CollectionPage collection = new() { Name = fileName };

                Collections.Add(collection);
            };
        }
    }

