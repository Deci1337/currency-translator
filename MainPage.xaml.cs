using System.Net.Http;
using System.Text.Json;

namespace ChinaConverter
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            FromPicker.SelectedIndex = 0; // по умолчанию CNY валюта
            ToPicker.SelectedIndex = 1;   // по умолчанию RUB
        }

        private async void OnConvertClicked(object sender, EventArgs e)
        {
            if (!double.TryParse(AmountEntry.Text, out double amount))
            {
                ResultLabel.Text = "Введите правильное число!!!!!";
                return;
            }

            string fromCurrency = FromPicker.SelectedItem?.ToString().Contains("CNY") == true ? "CNY" : "RUB";
            string toCurrency = ToPicker.SelectedItem?.ToString().Contains("CNY") == true ? "CNY" : "RUB";

            try
            {
                double rate = await GetConversionRateAsync(fromCurrency, toCurrency);

                double result = amount * rate;

                ResultLabel.Text = $">>: {result:F2} {toCurrency}";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = $"Ошибка: {ex.Message}";
            }
        }

        // метод конвертации валют
        private async Task<double> GetConversionRateAsync(string fromCurrency, string toCurrency)
        {
            if (fromCurrency == toCurrency)
                return 1.0;

            string url = $"https://api.exchangerate-api.com/v4/latest/{fromCurrency}";

            using HttpClient client = new HttpClient();

            string response = await client.GetStringAsync(url);

            using JsonDocument json = JsonDocument.Parse(response);

            JsonElement rates = json.RootElement.GetProperty("rates");

            double rate = rates.GetProperty(toCurrency).GetDouble();

            return rate;
        }
    }
}
