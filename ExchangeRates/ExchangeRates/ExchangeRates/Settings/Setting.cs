using System;
using System.Collections.Generic;
using System.Text;
using ExchangeRates.Model;
using System.Text.Json;
using System.Linq;

namespace ExchangeRates.Settings
{
    public class Setting
    {
        //Получение настроек в виде списка элементов, которые необходимо скрыть
        public static CurrencyCollection GetSwitchSetting()
        {
            string settingsToString = App.Current.Properties["switchSettings"].ToString();
            CurrencyCollection currencyCollectionWhereTrue = JsonSerializer.Deserialize<CurrencyCollection>(settingsToString);
            return currencyCollectionWhereTrue;
        }
        //Если это первый вход пользователя, то в настройки добавляются долларв, евро и рубль.
        //При следующем заходе настройка с ключом "switchSettings" будет уже существовать и это условние не будет выполняться
        public static void FirstEntryCheck (List<Currency> currencies){
            if (App.Current.Properties.ContainsKey("switchSettings") != true)
            {
                App.Current.Properties.Add("firstJoinOrNot", "firstJoinExist");
                CurrencyCollection curCol = new CurrencyCollection();
                var curDefault = from cur in currencies where cur.CharCode == "USD" || cur.CharCode == "EUR" || cur.CharCode == "RUB" select cur;
                foreach (var curWher in curDefault)
                {
                    curWher.SwitchOnOrOff = true;
                    curCol.Collection.Add(curWher);
                }
                string jsonString = JsonSerializer.Serialize(curCol);
                App.Current.Properties.Add("switchSettings", jsonString);              
            }
        }
       
    }
}
