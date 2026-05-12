using System;
using System.Collections.Generic;

namespace Lab_04_05
{
    public class DrivingService
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FullName { get; set; }
        public string Description { get; set; }
        public List<string> ImagePaths { get; set; } = new();
        public string Category { get; set; }
        public double Rating { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string TransmissionType { get; set; }
        public int DurationHours { get; set; }
        public string Country { get; set; }
        public decimal Discount { get; set; }
        public bool IsNotAvailable { get; set; }
        public List<string> RelatedServiceIds { get; set; } = new();
        public int PurchasedCount { get; set; }
        public string Producer { get; set; }
        public string Color { get; set; }
        public int GroupSize { get; set; }
    }
}