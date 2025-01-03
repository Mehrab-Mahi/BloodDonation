﻿using System;

namespace BloodDonation.Domain.Entities
{
    public class Campaign : Entity
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Address { get; set; }
        public string BannerUrl { get; set; }
    }
}