using System;
using System.Linq;
using ExchangeRates.Helper;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ExchangeRates.Model;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;
using ExchangeRates.Settings;

namespace ExchangeRates.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrenciesSettingsPage : ContentPage
    {
        bool fitstClickOrSecond = false;
        Currency currencyDrag;
        Currency currencyDrop;
        int indexDrag;
        int indexDrop;
        Currency temp;
   
        public CurrenciesSettingsPage()
        {
            InitializeComponent();           
            NavigationPage.SetHasNavigationBar(this, false);
            CurrencyCollection currencyCollectionWhereTrue = Setting.GetSwitchSetting();
            //Применение данных из настроек, берем те элементы которые сохранены в настройках со значением свойства SwitchOffOrOn true,
            //И ищем в списке который содержит все элементы(CurrencyHelper.currencies) такой же элемент и задаем ему значение из настроек, после чего используем для показа данных
            foreach (var cur in currencyCollectionWhereTrue.Collection)
            {
                var curQ = (from c in CurrencyHelper.currencies where c.Id == cur.Id select c).FirstOrDefault();
                curQ.SwitchOnOrOff = cur.SwitchOnOrOff;
            }
            //var curOrderByBool = from cur in CurrencyHelper.currencies orderby cur.SwitchOnOrOff descending select cur;
            //currencyCollectView.ItemsSource = curOrderByBool.ToList();
            currencyCollectView.ItemsSource = CurrencyHelper.currencies.ToList();
        }

        private void backBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();          
        }     

        private void checkBtn_Clicked(object sender, EventArgs e)
        {
            //Сохранение настроек, просто записываем в настройки список элементов со значение true, путем сериализации его и записи в строку
            //CurrencyCollection curCol = new CurrencyCollection();
            //var curWherTrue = from cur in CurrencyHelper.currencies where cur.SwitchOnOrOff == true select cur;
            //foreach(var curWher in curWherTrue)
            //{
            //    curCol.Collection.Add(curWher);
            //}        
            //string jsonString = JsonSerializer.Serialize(curCol);         
            //App.Current.Properties.Remove("switchSettings");
            //App.Current.Properties.Add("switchSettings", jsonString);
            //DisplayAlert("", "Настройки успешно сохранены!", "OK");
            CurrencyCollection curCol = new CurrencyCollection();
            foreach(var cur in CurrencyHelper.currencies)
            {
                curCol.Collection.Add(cur);
            }
            string jsonString = JsonSerializer.Serialize(curCol);
            App.Current.Properties.Remove("switchSettings");
            App.Current.Properties.Add("switchSettings", jsonString);
            DisplayAlert("", "Настройки успешно сохранены!", "OK");
        }

        private void moveBtn_Clicked(object sender, EventArgs e)
        {
            if(fitstClickOrSecond == false)
            {
                fitstClickOrSecond = true;
                ImageButton imageButton = sender as ImageButton;
                Currency currency = imageButton.BindingContext as Currency;
                currencyDrag = currency;
                temp = currency;
                indexDrag = CurrencyHelper.currencies.IndexOf(currencyDrag);

            }
            else if(fitstClickOrSecond == true)
            {
                fitstClickOrSecond = false;
                ImageButton imageButton = sender as ImageButton;
                Currency currency = imageButton.BindingContext as Currency;
                currencyDrop = currency;
                indexDrop = CurrencyHelper.currencies.IndexOf(currencyDrop);
                CurrencyHelper.currencies[indexDrag] = CurrencyHelper.currencies[indexDrop];
                CurrencyHelper.currencies[indexDrop] = currencyDrag;
                currencyCollectView.ItemsSource = CurrencyHelper.currencies.ToList();

            }
        }
    }
}
