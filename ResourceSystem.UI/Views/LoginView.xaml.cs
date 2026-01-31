using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace ResourceSystem.UI.Views;

public partial class LoginView : UserControl
{
    private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    });

    public LoginView()
    {
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var username = UsernameBox.Text?.Trim() ?? "";
        var password = PasswordBox.SecurePassword;
        if (string.IsNullOrEmpty(username))
        {
            MessageBox.Show("Wprowadź nazwę użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var passwordStr = new System.Net.NetworkCredential("", password).Password;
        if (string.IsNullOrEmpty(passwordStr))
        {
            MessageBox.Show("Wprowadź hasło.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        LoginButton.IsEnabled = false;
        try
        {
            var body = JsonConvert.SerializeObject(new { username, password = passwordStr });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("https://localhost:7168/api/auth/login", content);
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
        catch (HttpRequestException ex)
        {
            MessageBox.Show($"Błąd połączenia: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }
}
