﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class Gallery
    {
        private static int _idCounter = 1;

        public int Id { get; private set; } = _idCounter++;
        public string Name { get; set; }
        public List<ImageData> Images { get; set; } = new List<ImageData>();
    }



}
