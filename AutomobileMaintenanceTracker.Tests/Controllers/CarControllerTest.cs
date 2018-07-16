using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutomobileMaintenanceTracker.Controllers;
using AutomobileMaintenanceTracker.Models;
using AutomobileMaintenanceTracker.Repositories;
using Moq;
using MvcFlash.Core;
using MvcFlash.Core.Messages;
using NUnit.Framework;

namespace AutomobileMaintenanceTracker.Tests.Controllers
{
    [TestFixture]
    public class CarControllerTest
    {
        private Mock<ICarRepository> _repo;
        private Mock<IFlashMessenger> _messenger;
        private MessageBase _message;
        private CarsController _controller;

        [SetUp]
        public void Setup()
        {
            _repo = new Mock<ICarRepository>();
            _messenger = new Mock<IFlashMessenger>();
            _messenger.Setup(m => m.Push(It.IsAny<MessageBase>())).Callback<MessageBase>(r => { _message = r;});
            _controller = new CarsController(_repo.Object, _messenger.Object);
        }

        [Test]
        public void ShouldToIndexWhenSuccess()
        {
            _repo.Setup(r=>r.GetAllCars()).Returns(new List<Car>());

            var actResult = _controller.Index() as ViewResult;
            Assert.AreEqual(actResult.ViewName, "Index");
        }
        [Test]
        public void ShouldToCreateWhenSuccess()
        {
            var actResult = _controller.Create() as ViewResult;
            Assert.AreEqual(actResult.ViewName, "Create");
        }
        [Test]
        public void ShouldToIndexWhenSaveSuccess()
        {
            _repo.Setup(r=> r.Create(It.IsAny<Car>())).Verifiable();
            var actResult = _controller.Create(new Car
            {
                Id = 0,
                Make = "C",
                Model = "D",
                Type = Car.CarType.Electric.ToString(),
                Vin = "B1",
                Milage = 2,
                Year = 2010
            }) as ViewResult;
            _messenger.Verify(m => m.Push(It.IsAny<MessageBase>()));
            Assert.AreEqual(_message.MessageType, "success");
        }
        [Test]
        public void ShouldToCreateWhenSaveFailedWithInvalidInput()
        {
            _repo.Setup(r => r.Create(It.IsAny<Car>())).Verifiable();
            _controller.ModelState.AddModelError("test", "test");
            var actResult = _controller.Create(new Car
            {
                Id = 0,
                Make = "C",
                Type = Car.CarType.Electric.ToString()
            }) as ViewResult;
            _messenger.Verify(m => m.Push(It.IsAny<MessageBase>()));
            Assert.AreEqual(_message.MessageType, "error");
        }

        [Test]
        public void ShouldToEditWhenSaveFailedWithInvalidInput()
        {
            _controller.ModelState.AddModelError("test", "test");
            var actResult = _controller.Edit(new Car
            {
                Id = 0,
                Make = "C",
                Type = Car.CarType.Electric.ToString()
            }) as ViewResult;
            Assert.AreEqual(actResult.ViewName, "Edit");
        }

        [Test]
        public void ShouldToIndexWhenEditSuccess()
        {
            _repo.Setup(r =>r.DeleteMaintainceTasksFromCar(It.IsAny<Car>())).Verifiable();
            _repo.Setup(r => r.AddMaintainceTasksToCar(It.IsAny<List<MaintainceTask>>())).Verifiable();
            _repo.Setup(r => r.Update(It.IsAny<Car>())).Verifiable();
            var actResult = _controller.Edit(new Car
            {
                Id = 0,
                Make = "C",
                Type = Car.CarType.Electric.ToString()
            }) as ViewResult;
            Assert.AreEqual(_message.MessageType, "success");
        }
        [Test]
        public void ShouldToEditWhenSaveFailedWithDbError()
        {
            _repo.Setup(r => r.DeleteMaintainceTasksFromCar(It.IsAny<Car>())).Throws(new SystemException("test"));
            var actResult = _controller.Edit(new Car
            {
                Id = 0,
                Make = "C",
                Type = Car.CarType.Electric.ToString()
            }) as ViewResult;

            Assert.AreEqual(_message.MessageType, "error");
            Assert.AreEqual(_message.Title, "test");
        }

        [Test]
        public void ShouldToEditWhenFirstLoadSuccess()
        {
            _repo.Setup(r => r.FindCarById(It.IsAny<int>())).Returns(new Car
            {
                Id = 0,
                Make = "C",
                Type = Car.CarType.Electric.ToString()
            });
            var actResult = _controller.Edit(1) as ViewResult; ;
            Assert.AreEqual(actResult.ViewName, "Edit");
        }
        [Test]
        public void ShouldErrorOutWhenEditNotFoundCar()
        {

            _repo.Setup(r => r.FindCarById(It.IsAny<int>())).Returns((Car)null);
            var actResult = _controller.Edit(1) as ViewResult; ;
            Assert.AreEqual(_message.MessageType, "error");
        }
        [Test]
        public void ShouldToHttpNotFoundWhenDeleteNotFoundCar()
        {
            _repo.Setup(r => r.FindCarById(It.IsAny<int>())).Returns((Car)null);
            var actResult = _controller.Delete(1) ;
            Assert.IsInstanceOf<HttpNotFoundResult>(actResult);
        }

        [Test]
        public void ShouldToDeleteConfirmWhenSuccess()
        {
            _repo.Setup(r => r.FindCarById(It.IsAny<int>())).Returns(new Car
            {
                Id = 0,
                Make = "C",
                Type = Car.CarType.Electric.ToString()
            });
            var actResult = _controller.Delete(1) as ViewResult; 
            Assert.AreEqual(actResult.ViewName, "Delete");
        }

        [Test]
        public void ShouldChangeTypeWhenSuccess()
        {
            _repo.Setup(r => r.FindCarById(It.IsAny<int>())).Returns(new Car
            {
                Id = 1,
                Make = "C",
                Type = Car.CarType.Electric.ToString()
            });
            _repo.Setup(r => r.ChangeCarType(It.IsAny<Car>(), It.IsAny<Car.CarType>())).Verifiable();
            var actResult = _controller.ChangeCarType(1, Car.CarType.Diesel) ; 
            Assert.IsInstanceOf<RedirectToRouteResult>(actResult);
            Assert.AreEqual(_message.MessageType, "success");
        }

        [Test]
        public void ShouldDeleteConfirmedWhenSuccess()
        {
            _repo.Setup(r => r.FindCarById(It.IsAny<int>())).Returns(new Car
            {
                Id = 1,
                Make = "C",
                Type = Car.CarType.Electric.ToString()
            });
            _repo.Setup(r => r.Delete(It.IsAny<Car>())).Verifiable();
            _repo.Setup(r => r.DeleteMaintainceTasksFromCar(It.IsAny<Car>())).Verifiable();
            var actResult = _controller.DeleteConfirmed(1);
            Assert.AreEqual(_message.MessageType, "success");
            Assert.IsInstanceOf<RedirectToRouteResult>(actResult);
        }

        [Test]
        public void ShouldToIndexWhenDeleteConfirmedNotFoundCar()
        {
            _repo.Setup(r => r.FindCarById(It.IsAny<int>())).Returns((Car)null);
            var actResult = _controller.DeleteConfirmed(1);
            Assert.AreEqual(_message.MessageType, "error");
        }
    }
}
