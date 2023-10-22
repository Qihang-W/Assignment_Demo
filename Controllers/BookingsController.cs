using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Assignment_V2.Models;
using Microsoft.AspNet.Identity;

namespace Assignment_V2.Controllers
{
    public class BookingsController : Controller
    {
        private Entities db = new Entities();
        private String userId;

        [Authorize]
        public ActionResult Index()
        {
            userId = User.Identity.GetUserId();
            var bookings = db.BookingSet.Where(s => s.PatientID == userId).ToList();
            if (User.IsInRole("admin"))
            {
                bookings = db.BookingSet.ToList();
            }
            return View(bookings);
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.BookingSet.Find(id);
            userId = User.Identity.GetUserId();
            if (booking == null)
            {
                return HttpNotFound();
            }
            if (!User.IsInRole("admin") && userId != booking.AspNetUsersId)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            var list = from c in db.AspNetUserRoles
                        join o in db.AspNetUsers
                        on c.UserId equals o.Id
            select new
            {
                UserId = c.UserId,
                Email = o.Email,
                RoleId = c.RoleId
            };

            //ViewBag.AspNetUsersId = new SelectList(db.AspNetUserRoles.Where(s => s.RoleId == "2").ToList(), "UserId", "UserId");
            ViewBag.AspNetUsersId = new SelectList(list.Where(s => s.RoleId == "2").ToList(), "UserId", "Email");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingId,Description,BookingDate,AspNetUsersId")] Booking booking)
        {
            //booking.PatientID = User.Identity.GetUserId();
            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        db.BookingSet.Add(booking);
            //        db.SaveChanges();
            //        return RedirectToAction("Index");
            //    }
            //    catch (Exception ex)
            //    {
            //        // Handle any specific database or other exceptions here.
            //        ModelState.AddModelError("", "An error occurred while saving the booking.");
            //    }
            //}
                booking.PatientID = User.Identity.GetUserId();
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(booking.Description))
                    {
                        ModelState.AddModelError("Description", "Description is required.");
                    }

                    if (booking.BookingDate == null)
                    {
                        ModelState.AddModelError("BookingDate", "Booking date is required.");
                    }

                    if (ModelState.IsValid) // Check ModelState after adding custom errors
                    {
                        try
                        {
                            db.BookingSet.Add(booking);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        catch (Exception ex)
                        {
                            // Handle any specific database or other exceptions here.
                            ModelState.AddModelError("", "An error occurred while saving the booking.");
                        }
                    }
                }

                // If ModelState is not valid, show the form again with validation errors.
                ViewBag.AspNetUsersId = new SelectList(db.AspNetUsers, "Id", "Email", booking.AspNetUsersId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.BookingSet.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.AspNetUsersId = new SelectList(db.AspNetUsers, "Id", "Email", booking.AspNetUsersId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingId,Description,BookingDate,AspNetUsersId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AspNetUsersId = new SelectList(db.AspNetUsers, "Id", "Email", booking.AspNetUsersId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.BookingSet.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.BookingSet.Find(id);
            db.BookingSet.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
