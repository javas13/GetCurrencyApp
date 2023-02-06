using System.Collections.Generic;
using System.Xml.Serialization;

namespace ExchangeRates.Model
{
    //Классы для десериализации
    [XmlRoot("DailyExRates")]
    public class DailyExRate
    {
        [XmlAttribute("Date")]
        public string Date { get; set; }
        [XmlElement("Currency")]
        //Список валют
        public List<Currency> Currencies { get; set; }
    }
    public class Currency
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
        public float RateTomorrow { get; set; }
        [XmlElement("SwitchOnOrOff")]
        public bool SwitchOnOrOff { get; set; } = false;
    }
    public class CurrencyCollection
    {
        [XmlArray("Collection"), XmlArrayItem("Item")]
        public List<Currency> Collection { get; set; } = new List<Currency>();
    }
}
