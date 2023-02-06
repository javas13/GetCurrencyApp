using System.Collections.Generic;
using System.Xml.Serialization;

namespace ExchangeRates.Model
{
    //Этот класс также используется для вчерашней даты
    [XmlRoot("DailyExRates")]
    public class DailyExRateTomorrow
    {
        [XmlAttribute("Date")]
        public string Date { get; set; }
        [XmlElement("Currency")]
        //Список валют
        public List<Currency> Currencies { get; set; }
    }
    public class CurrencyTomorrow
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }
        [XmlElement("NumCode")]
        public int NumCode { get; set; }
        [XmlElement("CharCode")]
        public string CharCode { get; set; }
        [XmlElement("Scale")]
        public int Scale { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Rate")]
        public float Rate { get; set; }
       
    }
}
   
