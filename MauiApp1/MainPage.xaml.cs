using MauiApp1.Model;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    ObservableCollection<Book> T1 = new ObservableCollection<Book>(); // Removed initialization here

    private const string ApiUrl = "http://localhost:5265/api/Books"; 
    private HttpClient httpClient = new HttpClient();

    public MainPage()
    {
        InitializeComponent();
        LoadBooksAsync(); 
       
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadBooksAsync(); // Assumes LoadBooks is an async Task method
    }
    private async void LoadBooksAsync()
    {
        try
        {
            var response = await httpClient.GetAsync(ApiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var books = JsonConvert.DeserializeObject<List<Book>>(content);
                T1.Clear();
                foreach (var book in books)
                {
                    T1.Add(book);
                }
                itemListView.ItemsSource = T1; // This should automatically update the ListView
            }
            else
            {
                Console.WriteLine("Failed to retrieve books.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

   

    // Use async Task to return a Task
    public async Task<List<Book>> GetTasksAsync()
    {
        try
        {
            var response = await httpClient.GetAsync(ApiUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Book>>(content);
            }
            else
            {
                // Handle error
                Console.WriteLine("Failed to retrieve data from the API.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return new List<Book>();
    }

    public async Task<Book> CreateBookAsync(Book newBook)
    {
        try
        {
            var json = JsonConvert.SerializeObject(newBook);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var createdBookJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Book>(createdBookJson);
            }
            else
            {
                Console.WriteLine("Failed to create a new Book.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return null;
    }
    private async void CreateButton_Clicked(object sender, EventArgs e)
    {
        // Create a new Book using input values
        var newBook = new Book { Title = TitleEntry.Text, Author = AuthorEntry.Text };
        var createdBook = await CreateBookAsync(newBook);
        if (createdBook != null)
        {
            T1.Add(createdBook);
            // No need to reset itemListView.ItemsSource because ObservableCollection handles updates
        }
    }


    private async void UpdateButton_Clicked(object sender, EventArgs e)
    {
        // Update an existing Book using input values
        if (itemListView.SelectedItem is Book existingBook)
        {
            existingBook.Title = TitleEntry.Text;
            existingBook.Author = AuthorEntry.Text;
            bool isUpdated = await UpdateBookAsync(existingBook);
            if (isUpdated)
            {
               
            }
        }
    }

    private async void ItemListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

        var selectedBook = (Book)e.SelectedItem;

        // Navigate to the edit page and pass the selected Book for editing
        await Navigation.PushAsync(new EditPage(selectedBook, ApiUrl, httpClient));

        // Deselect the item
        itemListView.SelectedItem = null;
    }

    public async Task<bool> UpdateBookAsync(Book updatedBook)
    {
        try
        {
            var json = JsonConvert.SerializeObject(updatedBook);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"{ApiUrl}/{updatedBook.BookId}", content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Failed to update the Book.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return false;
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (itemListView.SelectedItem is Book selectedBook)
        {
            bool isDeleted = await DeleteBookAsync(selectedBook.BookId);
            if (isDeleted)
            {
                T1.Remove(selectedBook);
                // No need to reset itemListView.ItemsSource because ObservableCollection handles updates
            }
        }
        else
        {
            await DisplayAlert("No Selection", "Please select a book to delete.", "OK");
        }
    }
    public async Task<bool> DeleteBookAsync(int bookId)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"{ApiUrl}/{bookId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
}

