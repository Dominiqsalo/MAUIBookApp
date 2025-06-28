using MauiApp1.Model;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace MauiApp1;

public partial class EditPage : ContentPage
{
    private Book selectedBook;
    private string apiUrl;
    private HttpClient httpClient;
    private const string ApiUrl = "http://localhost:5265/api/Books";
    public EditPage(Book biblo,string apiUrl, HttpClient httpClient)
    {
		InitializeComponent();
        selectedBook = biblo;
        editTitleEntry.Text = selectedBook.Title;
        editAuthorEntry.Text = selectedBook.Author;
        this.apiUrl = apiUrl;
        this.httpClient = httpClient;

    }

    private async void UpdateButton_Clicked(object sender, EventArgs e)
    {
        selectedBook.Title = editTitleEntry.Text;
        selectedBook.Author = editAuthorEntry.Text;

        // Implement the update operation here (e.g., calling UpdateBookAsync)
        bool isUpdated = await UpdateBookAsync(selectedBook);
        if (isUpdated)
        {
            await DisplayAlert("Success", "Book updated successfully", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Failed to update the book. Please try again.", "OK");
       }
    }
    public async Task<bool> UpdateBookAsync(Book updatedBook)
    {
        try
        {
            var json = JsonConvert.SerializeObject(updatedBook);
            var content = new StringContent(json, Encoding.UTF8, "application/json"); // Correct usage of Encoding and media type
            var response = await httpClient.PutAsync($"{apiUrl}/{updatedBook.BookId}", content);

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
        bool confirmed = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this book?", "Yes", "No");
        if (confirmed)
        {
            bool isDeleted = await DeleteBookAsync(selectedBook.BookId);
            if (isDeleted)
            {
                // Assuming you have navigation set up to go back to the list of books
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Could not delete the book. Please try again.", "OK");
            }
        }
    }
    private async Task<bool> DeleteBookAsync(int bookId)
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