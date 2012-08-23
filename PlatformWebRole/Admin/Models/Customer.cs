﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Admin.Models;
using System.Text;

namespace Admin {
    partial class Customer {
        public string plainpassword { get; set; }

        internal List<Customer> GetAll() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            List<Customer> custs = db.Customers.OrderBy(x => x.dateAdded).ToList<Customer>();
            if (custs == null) {
                custs = new List<Customer>();
            }
            return custs;
        }

        internal void Get() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Customer cust = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            if (cust == null) {
                throw new Exception("Customer not found.");
            }
            this.email = cust.email;
            this.password = cust.password;
            this.fname = cust.fname;
            this.lname = cust.lname;
            this.phone = cust.phone;
            this.dateAdded = cust.dateAdded;
            this.isSuspended = cust.isSuspended;
            this.receiveNewsletter = cust.receiveNewsletter;
            this.receiveOffers = cust.receiveOffers;
            this.billingID = cust.billingID;
            this.shippingID = cust.shippingID;
            this.isValidated = cust.isValidated;
            this.validator = cust.validator;
            this.Address = cust.Address;
            this.Address1 = cust.Address1;
            this.Carts = cust.Carts;
        }

        internal void ToggleSuspended() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            Customer tmp = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            if (tmp.isSuspended == 0) {
                tmp.isSuspended = 1;
            } else {
                tmp.isSuspended = 0;
            }
            db.SubmitChanges();
        }

        internal void Save() {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            PasswordGenerator pw = new PasswordGenerator();
                string new_pass = pw.Generate();

            // Make sure we don't have an account with this e-mail address
            Customer cust = this.GetCustomerByEmail();
            if (cust != null && cust.ID > 0) {
                throw new Exception("An account using the E-Mail address you provided already exists.");
            }

            // We are going to make an attempt at saving the Customer record

            Customer new_customer = new Customer {
                email = this.email,
                fname = this.fname,
                lname = this.lname,
                phone = this.phone,
                dateAdded = this.dateAdded,
                isSuspended = this.isSuspended,
                isValidated = this.isValidated,
                validator = this.validator,
                password = this.password,
                receiveNewsletter = this.receiveNewsletter,
                receiveOffers = this.receiveOffers,
            };

            db.Customers.InsertOnSubmit(new_customer);
            db.SubmitChanges();
            this.ID = new_customer.ID;

            SendNotification();
        }

        internal void SaveAddresses(Address billing, Address shipping) {

            this.billingID = billing.ID;
            this.shippingID = shipping.ID;

            Customer tmp = new Customer();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            tmp = db.Customers.Where(x => x.ID.Equals(this.ID)).FirstOrDefault<Customer>();
            tmp.billingID = this.billingID;
            tmp.shippingID = this.shippingID;
            db.SubmitChanges();
        }

        internal List<Address> GetAddresses() {
            List<Address> addresses = new List<Address>();
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            addresses = db.Addresses.Where(x => x.cust_id.Equals(this.ID) && x.active).ToList<Address>();
            return addresses;
        }

        internal Customer GetCustomerByEmail(string email = null) {
            EcommercePlatformDataContext db = new EcommercePlatformDataContext();
            if (email == null) {
                return db.Customers.Where(x => x.email.Equals(this.email)).FirstOrDefault<Customer>();
            } else {
                return db.Customers.Where(x => x.email.Equals(email)).FirstOrDefault<Customer>();
            }
        }

        internal void SendNotification() {
            string[] tos = { this.email };
            StringBuilder sb = new StringBuilder();
            Settings settings = new Settings();

            sb.Append("<p>A new account with this e-mail address was created from <a href='" + settings.Get("SiteURL") + "' title='" + settings.Get("SiteName") + "'>" + settings.Get("SiteName") + "</a>.</p>");
            sb.Append("<hr />");
            sb.AppendFormat("<p>Your password has been automatically generated and is listed below.</p><hr />");
            sb.AppendFormat("<p><strong>Password:</strong> {0}</p>", this.plainpassword);
            sb.Append("<hr /><br />");
            sb.Append("<p style='font-size:11px'>If you feel this was a mistake please disregard this e-mail.</p>");

            UDF.SendEmail(tos, "New Account for " + settings.Get("SiteName"), true, sb.ToString());
            this.plainpassword = "";
        }

        internal void GeneratePassword() {
            try {
                PasswordGenerator pw = new PasswordGenerator();
                string new_pass = pw.Generate();
                this.plainpassword = new_pass;
                this.password = UDF.EncryptString(new_pass);
            } catch (Exception) {}
        }

    }
}