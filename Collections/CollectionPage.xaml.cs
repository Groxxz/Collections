using System.Collections.ObjectModel;

namespace Collections;

public partial class CollectionPage : ContentPage
{
    public string Name { get; set; }

    public ObservableCollection<Element> Elements = new();


    public CollectionPage()
    {
        InitializeComponent();

        elements.ItemsSource = Elements;
        LoadElementsFromFile();
    }

    private async void AddElementButton_Clicked(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("Nowy element", "Podaj nazwê elementu", "Dodaj", "Anuluj");
        if (name != null && name != "Anuluj")
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Element element = new() { Name = name };
                Elements.Add(element);
                SaveElementsToFile();
            }
            else
            {
                await DisplayAlert("B³¹d", "Nazwa elementu nie mo¿e byæ pusta.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Info", "Operacja anulowana.", "OK");
        }
    }

    private void DeleteElementButton_Clicked(object sender, EventArgs e)
    {
        if (elements.SelectedItem is Element selectedElement)
        {
            Elements.Remove(selectedElement);
            SaveElementsToFile();
        }
    }

    private async void EditElementButton_Clicked(object sender, EventArgs e)
    {
        if (elements.SelectedItem is Element selectedElement)
        {
            string newName = await DisplayPromptAsync("Edycja elementu", "Nowa nazwa elementu", initialValue: selectedElement.Name, accept: "Edytuj", cancel: "Anuluj");
            if (newName != null && newName != "Anuluj")
            {
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    selectedElement.Name = newName;
                    SaveElementsToFile();
                }
                else
                {
                    await DisplayAlert("B³¹d", "Nowa nazwa elementu nie mo¿e byæ pusta.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Info", "Edycja anulowana.", "OK");
            }
        }
    }

    private void LoadElementsFromFile()
    {
        string filePath = GetFilePath();
        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                Elements.Add(new Element { Name = line });
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadElementsFromFile();
    }

    private void SaveElementsToFile()
    {
        string filePath = GetFilePath();
        File.WriteAllLines(filePath, Elements.Select(e => e.Name));
    }

    private string GetFilePath()
    {
        return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Collections"), $"{Name}.txt");
    }
}