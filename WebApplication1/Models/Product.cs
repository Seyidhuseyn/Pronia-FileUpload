﻿namespace WebApplication1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public float Price { get; set; }
        public int CaegoryId { get; set; }
        public Category Category { get; set; }
    }
}