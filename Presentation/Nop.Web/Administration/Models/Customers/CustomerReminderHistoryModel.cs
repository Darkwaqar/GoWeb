﻿using System;

namespace Nop.Admin.Models.Customers
{
    public class CustomerReminderHistoryModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime SendDate { get; set; }
        public int Level { get; set; }
        public bool OrderId { get; set; }
    }
}