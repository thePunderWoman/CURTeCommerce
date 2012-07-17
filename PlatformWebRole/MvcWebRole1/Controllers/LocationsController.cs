﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommercePlatform.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace EcommercePlatform.Controllers {
    public class LocationsController : BaseController {

        public ActionResult Index() {

            List<PrettyLocation> locs = LocationModel.GetAll();
            ViewBag.locations = locs;
            
            ViewBag.locations_json = JsonConvert.SerializeObject(locs);

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string GetNearest(float lat = 0, float lon = 0) {
            try {
                return JsonConvert.SerializeObject(LocationModel.GetNearest(lat, lon));
            } catch (Exception e) {
                return e.Message;
            }
        }

    }
}
