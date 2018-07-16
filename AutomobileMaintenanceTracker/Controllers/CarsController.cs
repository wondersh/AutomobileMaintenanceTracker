using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutomobileMaintenanceTracker.Models;
using AutomobileMaintenanceTracker.Repositories;
using MvcFlash.Core;
using MvcFlash.Core.Extensions;

namespace AutomobileMaintenanceTracker.Controllers
{
    public class CarsController : Controller
    {
        private readonly ICarRepository _carRepo;
        private readonly IFlashMessenger _flashMessenger;

        public CarsController(ICarRepository repo, IFlashMessenger flashMessenger)
        {
            _carRepo = repo;
            _flashMessenger = flashMessenger;
        }

        public CarsController()
        {
             _carRepo = new CarRepository(new AMTDbContext());   //TODO add DI later
             _flashMessenger = Flash.Instance;
        }

        public ActionResult Index()
        {
            return View("Index", _carRepo.GetAllCars());
        }
        
        public ActionResult Create()
        {
            ViewBag.Type = ViewBag.Type = GetCarTypeList();
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,Vin,Model,Make,Year,Milage")] Car car)
        {
            if (ModelState.IsValid)
            {
                _carRepo.Create(car);
                _flashMessenger.Success("Car Created");
                return RedirectToAction("Index");
            }
            else
            {
                _flashMessenger.Error("Invalid Car Data");
                return View("Create",car);
            }

        }

        public ActionResult Edit(int id)
        {
            var car = _carRepo.FindCarById(id);
            if (car != null)
            {
                car.MaintainceTasks = car.GetMaintainceTasksByType();
                ViewBag.Type = GetCarTypeList();
                return View("Edit",car);
            }
            else
            {
                _flashMessenger.Error("Car Not found");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeCarType(int id, Car.CarType type)
        {
            var result = _carRepo.FindCarById(id);
            _carRepo.ChangeCarType(result, type);
            _flashMessenger.Success("Type Updated");
            return RedirectToAction("Edit", new {id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Car car)
        {
            ViewBag.Type = ViewBag.Type = GetCarTypeList();
            if (ModelState.IsValid)
            {
                try
                {
                    _carRepo.DeleteMaintainceTasksFromCar(car);
                    _carRepo.AddMaintainceTasksToCar(car.MaintainceTasks);
                    car.MaintainceTasks = null;
                    _carRepo.Update(car);
                    _flashMessenger.Success("Car information saved");
                }
                catch (SystemException e)
                {
                    _flashMessenger.Error(e.Message);
                }
                
                return RedirectToAction("Index");
            }
            return View("Edit",car);
        }

        public ActionResult Delete(int id)
        {
            var car = _carRepo.FindCarById(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View("Delete",car);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var car = _carRepo.FindCarById(id);
            if (car != null)
            {
                _carRepo.DeleteMaintainceTasksFromCar(car);
                _carRepo.Delete(car);
                _flashMessenger.Success("Car deleted");
            }
            else
            {
                _flashMessenger.Error("Car Not found");
            }

            return RedirectToAction("Index");
        }

        private IEnumerable<SelectListItem> GetCarTypeList()
        {
            return Enum.GetValues(typeof(Car.CarType)).Cast<Car.CarType>().Select(m => new SelectListItem
            {
                Value = m.ToString(),
                Text = m.ToString()
            });
        }
    }
}
