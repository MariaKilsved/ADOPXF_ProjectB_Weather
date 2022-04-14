using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Weather.Models;
using Weather.Services;

namespace Weather.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ForecastPage : ContentPage
    {
        OpenWeatherService service;
        //GroupedForecast groupedforecast;

        public ForecastPage()
        {
            InitializeComponent();
            
            service = new OpenWeatherService();
            //groupedforecast = new GroupedForecast();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Code here will run right before the screen appears
            //You want to set the Title or set the City

            //This is making the first load of data
            MainThread.BeginInvokeOnMainThread(async () => {await LoadForecast();});

        }

        private async Task LoadForecast()
        {
            //Here you load the forecast 
            Task<Forecast> t1 = null;
            Exception exception = null;

            try
            {
                loading.IsVisible = true;
                errorMsg.IsVisible = false;
                errorMsgEx.Text = "";
                await service.GetForecastAsync(Title);
                t1 = service.GetForecastAsync(Title);
            }
            catch (Exception ex)
            {
                //if exception write the message later
                exception = ex;
            }

            if (t1?.Status == TaskStatus.RanToCompletion)
            {
                Forecast forecast = t1.Result;
                var GroupedList = forecast.Items.GroupBy(item => item.DateTime.Date);
                //groupedforecast.City = Title;
                //groupedforecast.Items = GroupedList;
                GroupedForecastListView.ItemsSource = GroupedList;
                loading.IsVisible = false;
            }
            else
            {
                loading.IsVisible = false;
                errorMsg.IsVisible = true;
                errorMsgEx.Text = exception.Message;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await LoadForecast();
        }
    }
}