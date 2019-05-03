using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnketUygulamaSistemi.Models
{
    public class Soru
    {
        public string soruMetni { get; set; }
        public string soruTipi { get; set; }
        public List<String> cevaplar { get; set; }
    }
}