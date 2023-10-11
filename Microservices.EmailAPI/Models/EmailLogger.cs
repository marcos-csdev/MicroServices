﻿namespace Microservices.EmailAPI.Models
{
    public class EmailLogger
    {
        public int Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? DateSent { get; set; }
    }
}
