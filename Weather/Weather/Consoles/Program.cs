using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Weather.Models;
using Weather.Services;

namespace Weather.Consoles
{
    //Your can move your Console application Main here. Rename Main to myMain and make it NOT static and async
    class Program
    {
        #region used by the Console
        Views.ConsolePage theConsole;
        StringBuilder theConsoleString;
        public Program(Views.ConsolePage myConsole)
        {
            //used for the Console
            theConsole = myConsole;
            theConsoleString = new StringBuilder();
        }
        #endregion

        #region Console Demo program
        //This is the method you replace with your async method renamed and NON static Main
        public async Task myMain()
        {
            //Register the event
            OpenWeatherService service = new OpenWeatherService();
            service.WeatherForecastAvailable += ReportWeatherDataAvailable;

            Task<Forecast> t1 = null, t2 = null, t3 = null, t4 = null;
            Exception exception = null;
            try
            {
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;

                //Create the two tasks and wait for comletion
                await service.GetForecastAsync(latitude, longitude);
                await service.GetForecastAsync("Miami");

                t1 = service.GetForecastAsync(latitude, longitude);
                t2 = service.GetForecastAsync("Miami");

                Task.WaitAll(t1, t2);

                t3 = service.GetForecastAsync(latitude, longitude);
                t4 = service.GetForecastAsync("Miami");

                //Wait and confirm we get an event showing cahced data avaialable
                Task.WaitAll(t3, t4);
            }
            catch (Exception ex)
            {
                //if exception write the message later
                exception = ex;
            }

            theConsole.WriteLine("-----------------");
            if (t1?.Status == TaskStatus.RanToCompletion)
            {
                Forecast forecast = t1.Result;
                theConsole.WriteLine($"Weather forecast for {forecast.City}");
                var GroupedList = forecast.Items.GroupBy(item => item.DateTime.Date);
                foreach (var group in GroupedList)
                {
                    theConsoleString.AppendLine(group.Key.Date.ToShortDateString());
                    foreach (var item in group)
                    {
                        theConsoleString.AppendLine($"   - {item.DateTime.ToShortTimeString()}: {item.Description}, teperature: {item.Temperature} degC, wind: {item.WindSpeed} m/s");
                    }
                }
                theConsole.WriteLine(theConsoleString.ToString());
            }
            else
            {
                theConsole.WriteLine($"Geolocation weather service error.");
                theConsole.WriteLine($"Error: {exception.Message}");
            }
            theConsoleString.Clear();

            theConsole.WriteLine("-----------------");
            if (t2.Status == TaskStatus.RanToCompletion)
            {
                Forecast forecast = t2.Result;
                theConsole.WriteLine($"Weather forecast for {forecast.City}");
                var GroupedList = forecast.Items.GroupBy(item => item.DateTime.Date);
                foreach (var group in GroupedList)
                {
                    theConsoleString.AppendLine(group.Key.Date.ToShortDateString());
                    foreach (var item in group)
                    {
                        theConsoleString.AppendLine($"   - {item.DateTime.ToShortTimeString()}: {item.Description}, teperature: {item.Temperature} degC, wind: {item.WindSpeed} m/s");
                    }
                }
                theConsole.WriteLine(theConsoleString.ToString());
            }
            else
            {
                theConsole.WriteLine($"City weather service error");
                theConsole.WriteLine($"Error: {exception.Message}");
            }
        }

        //If you have any event handlers, they could be placed here
        void ReportWeatherDataAvailable(object sender, string message)
        {
            theConsole.WriteLine($"Event message from weather service: {message}"); //theConsole is a Captured Variable, don't use myConsoleString here
        }
        #endregion
    }
}
