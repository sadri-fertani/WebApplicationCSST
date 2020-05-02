using System;

namespace WebApplicationCSST.API.Hubs.Models
{
    public class MessageModel
    {
        public int IdObject { get; set; }
        public string TypeObject { get; set; }
        public DateTime Date { get; set; }
    }
}