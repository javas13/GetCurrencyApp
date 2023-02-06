using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ExchangeRates.Model;
using System.Globalization;
using ExchangeRates.Helper;
using System.Text.Json;
using ExchangeRates.Settings;

namespace ExchangeRates.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListOfCurrenciesPage : ContentPage
    {
        public ListOfCurrenciesPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            try
            {
                List<Currency> currenciesListCompl = new List<Currency>();
                List<Currency> currenciesList = new List<Currency>();
                List<Currency> currenciesListTomorrow = new List<Currency>();
                string dateOfValut = "";
                string dateOfValutForTomorrow = "";
                string dateOfValutForYesterday = "";
                DateTime dateToday = DateTime.Now;
                DateTime dateTomorrow = dateToday.AddDays(1);
                DateTime dateYesterday = dateToday.AddDays(-1);
                string URLString = ($"https://www.nbrb.by/Services/XmlExRates.aspx?ondate={dateToday.Year}-{dateToday.Month}-{dateToday.Day}");
                string URLStringTomorrow = ($"https://www.nbrb.by/Services/XmlExRates.aspx?ondate={dateTomorrow.Year}-{dateTomorrow.Month}-{dateTomorrow.Day}");
                string URLStringYesterday = ($"https://www.nbrb.by/Services/XmlExRates.aspx?ondate={dateYesterday.Year}-{dateYesterday.Month}-{dateYesterday.Day}");
                XmlTextReader reader = new XmlTextReader(URLString);
                XmlSerializer serializer = new XmlSerializer(typeof(DailyExRate));
                while (reader.Read())
                {
                    DailyExRate result = (DailyExRate)serializer.Deserialize(reader);
                    currenciesList = result.Currencies;
                    dateOfValut = result.Date;
                }
                XmlTextReader readerForTomorrow = new XmlTextReader(URLStringTomorrow);
                XmlSerializer serializerForTomorrow = new XmlSerializer(typeof(DailyExRateTomorrow));
                while (readerForTomorrow.Read())
                {
                    DailyExRateTomorrow result = (DailyExRateTomorrow)serializerForTomorrow.Deserialize(readerForTomorrow);
                    currenciesListTomorrow = result.Currencies;
                    dateOfValutForTomorrow = result.Date;
                }
                //Если курсов на завтра, то сегодняшняя дата становится вчерашней,
                //А завтрашняя сегодняшней
                if (currenciesListTomorrow.Count() == 0)
                {
                    XmlTextReader readerForYesterday = new XmlTextReader(URLStringYesterday);
                    XmlSerializer serializerForYesterday = new XmlSerializer(typeof(DailyExRateTomorrow));
                    while (readerForYesterday.Read())
                    {
                        DailyExRateTomorrow result = (DailyExRateTomorrow)serializerForYesterday.Deserialize(readerForYesterday);
                        currenciesListTomorrow = result.Currencies;
                        dateOfValutForYesterday = result.Date;
                    }
                    tomorrowDateLbl.Text = dateToday.ToString("dd.MM.yy");
                    todayDateLbl.Text = dateYesterday.ToString("dd.MM.yy");
                    foreach (var currency in currenciesList)
                    {
                        Currency curTomQ = (from cur in currenciesListTomorrow where cur.Id == currency.Id select cur).FirstOrDefault();
                        currency.RateTomorrow = curTomQ.Rate;
                    }
                    foreach (var el in currenciesList)
                    {
                        currenciesListCompl.Add(el);
                    };
                    foreach (var currency in currenciesListCompl)
                    {
                        Currency curTomQ = (from cur in currenciesList where cur.Id == currency.Id select cur).FirstOrDefault();
                        float rateFl = curTomQ.Rate;
                        float rateTomFl = curTomQ.RateTomorrow;
                        currency.RateTomorrow = rateFl;
                        currency.Rate = rateTomFl;
                    }
                    foreach (var currency in currenciesListCompl)
                    {
                        Currency newCur = new Currency()
                        {
                            Id = currency.Id,
                            CharCode = currency.CharCode,
                            Name = currency.Name,
                            Rate = currency.Rate,
                            NumCode = currency.NumCode,
                            RateTomorrow = currency.RateTomorrow,
                            Scale = currency.Scale,
                            SwitchOnOrOff = currency.SwitchOnOrOff
                        };
                        CurrencyHelper.currencies.Add(newCur);
                    }                   
                    Setting.FirstEntryCheck(currenciesListCompl);
                    CurrencyCollection currencyCollection = Setting.GetSwitchSetting();
                    foreach(var cur in currenciesListCompl)
                    {
                        var curQ = (from c in currencyCollection.Collection where c.Id == cur.Id select c).FirstOrDefault();
                    if (curQ != null)
                    {
                        curQ.RateTomorrow = cur.RateTomorrow;
                        curQ.Rate = cur.Rate;
                    }                   
                    }
                    CurrencyCollection currencyCollectionWhrTrue = new CurrencyCollection();
                    foreach (var cur in currencyCollection.Collection)
                    {
                        if(cur.SwitchOnOrOff == true)
                        {
                            currencyCollectionWhrTrue.Collection.Add(cur);
                        }
                    }
                    currencyCollectView.ItemsSource = currencyCollectionWhrTrue.Collection.ToList();
                }
                else
                {
                    //Если курсы на завтра есть, то
                    //Перевод из одной культуры даты в другую
                    string validformat = "d";
                    CultureInfo provider = new CultureInfo("en-US");
                    DateTime convertDate = DateTime.ParseExact(dateOfValut, validformat, provider);
                    todayDateLbl.Text = convertDate.ToString("dd.MM.yy");

                    //Без перевода культуры, а просто с использование времени на устройстве
                    tomorrowDateLbl.Text = dateTomorrow.ToString("dd.MM.yy");

                    foreach (var currency in currenciesList)
                    {
                        Currency curTomQ = (from cur in currenciesListTomorrow where cur.Id == currency.Id select cur).FirstOrDefault();
                        currency.RateTomorrow = curTomQ.Rate;
                    }
                    //CurrencyHelper.currencies = currenciesList;
                    foreach (var currency in currenciesList)
                    {
                        Currency newCur = new Currency()
                        {
                            Id = currency.Id,
                            CharCode = currency.CharCode,
                            Name = currency.Name,
                            Rate = currency.Rate,
                            NumCode = currency.NumCode,
                            RateTomorrow = currency.RateTomorrow,
                            Scale = currency.Scale,
                            SwitchOnOrOff = currency.SwitchOnOrOff
                        };
                        CurrencyHelper.currencies.Add(newCur);
                    }                 
                    Setting.FirstEntryCheck(currenciesList);
                    CurrencyCollection currencyCollection = Setting.GetSwitchSetting();
                    //Меняем значения курсов на акутальные, т.к. в настройках лежат старые данные
                    foreach (var cur in currenciesList)
                    {
                        var curQ = (from c in currencyCollection.Collection where c.Id == cur.Id select c).FirstOrDefault();
                        if(curQ != null)
                    {
                        curQ.RateTomorrow = cur.RateTomorrow;
                        curQ.Rate = cur.Rate;
                    }                       
                    }
                    CurrencyCollection currencyCollectionWhrTrue = new CurrencyCollection();
                    foreach (var cur in currencyCollection.Collection)
                    {
                        if (cur.SwitchOnOrOff == true)
                        {
                            currencyCollectionWhrTrue.Collection.Add(cur);
                        }
                    }
                    currencyCollectView.ItemsSource = currencyCollectionWhrTrue.Collection.ToList();
                }
            }
            catch
            {
                currencyCollectView.IsVisible = false;
                setBtn.IsVisible = false;
                DisplayAlert("Ошибка", "Не удалось получить курсы валют", "Ок");
            }
        }

        private void setBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CurrenciesSettingsPage());
        }

        private void quitBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}