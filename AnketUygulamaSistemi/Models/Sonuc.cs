﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnketUygulamaSistemi.Models
{
    public class Sonuc
    {
        public String soruMetni { get; set; }
        public List<Secenek> secenekler { get; set; }
        public string[] cevap { get; set; }
        public int[] cevapSayisi { get; set; }
        public int tip { get; set; }
        public int count { get; set; }
        public int soruId { get; set; }
    }
}