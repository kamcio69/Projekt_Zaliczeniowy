using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace ResourceSystem.UI.Views;

public partial class LoginView : UserControl
{
    private static readonly HttpClient HttpClient = new HttpClient
    {
        BaseAddress = new System.Uri("http://localhost:5079")
    };

    public LoginView()
    {
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var username = UsernameBox.Text?.Trim() ?? "";
        var password = new System.Net.NetworkCredential("", PasswordBox.SecurePassword).Password;
        if (string.IsNullOrEmpty(username))
        {
            MessageBox.Show("Wprowadź nazwę użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Wprowadź hasło.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        LoginButton.IsEnabled = false;
        try
        {
            var body = JsonSerializer.Serialize(new { username, password });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("/api/auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Zalogowano pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                MessageBox.Show(string.IsNullOrWhiteSpace(errorBody) ? $"Błąd: {response.StatusCode}" : errorBody,
                    "Błąd logowania", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Błąd: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }
}
