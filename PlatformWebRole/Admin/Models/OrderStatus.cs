﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin {
    partial class OrderStatus {
        public List<OrderStatus> GetAll() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<OrderStatus> statuses = db.OrderStatus.OrderBy(x => x.ID).ToList();
            return statuses;
        }
    }

    public enum OrderStatuses {
        PaymentPending = 1,
        PaymentComplete = 2,
        PaymentDeclined = 3,
        Processed = 4,
        Refunded = 5,
        BackOrder = 6,
        CheckNotes = 7,
        MessageSent = 8,
        AwaitingTracking = 9,
        Shipped = 10,
        AwaitingCancellation = 11,
        Cancelled = 12,
        Fraudulent = 13,
        Void = 14,
        Complete = 15
    }
}