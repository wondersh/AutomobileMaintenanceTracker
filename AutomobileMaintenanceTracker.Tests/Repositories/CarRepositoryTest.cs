using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using AutomobileMaintenanceTracker.Models;
using AutomobileMaintenanceTracker.Repositories;
using Moq;
using NUnit.Framework;

namespace AutomobileMaintenanceTracker.Tests.Repositories
{
    [TestFixture]
    public class CarRepositoryTest
    {
        private List<Car> _carData;
        private List<MaintainceTask> _taskData;
        private CarRepository _repo;

        [SetUp]
        public void Setup()
        {
            _carData = new List<Car>
            {
                new Car
                {
                    Id = 1,
                    Make = "A",
                    Model = "B",
                    Type = Car.CarType.Gas.ToString(),
                    Vin = "A1",
                    Milage = 1,
                    Year = 2000,
                    MaintainceTasks = new Collection<MaintainceTask>
                    {
                        new MaintainceTask
                        {
                            Id = 1,
                            LastTime = DateTime.Now,
                            NextTime = DateTime.Now,
                            Type = MaintainceTask.TaskType.OilChange.ToString(),
                            CarId = 1
                        },
                        new MaintainceTask
                        {
                            Id = 2,
                            LastTime = DateTime.Now,
                            NextTime = DateTime.Now,
                            Type = MaintainceTask.TaskType.TireRotation.ToString(),
                            CarId = 1
                        }
                    }
                },
                new Car
                {
                    Id = 2,
                    Make = "C",
                    Model = "D",
                    Type = Car.CarType.Electric.ToString(),
                    Vin = "B1",
                    Milage = 2,
                    Year = 2010,
                    MaintainceTasks = new Collection<MaintainceTask>
                    {
                        new MaintainceTask
                        {
                            Id = 3,
                            LastTime = DateTime.Now,
                            NextTime = DateTime.Now,
                            Type = MaintainceTask.TaskType.TireRotation.ToString(),
                            CarId = 2
                        }
                    }
                }
            };
            _taskData = new List<MaintainceTask>();
            _carData.ForEach(e =>
            {
                _taskData.AddRange(e.MaintainceTasks.ToList());
            });
            var carSet = new Mock<DbSet<Car>>()
                .SetupData(_carData);
            var taskSet = new Mock<DbSet<MaintainceTask>>()
                .SetupData(_taskData);
            var context = new Mock<AMTDbContext>();
            context.Setup(c => c.Cars).Returns(carSet.Object);
            context.Setup(c => c.MaintainceTasks).Returns(taskSet.Object);
            _repo = new CarRepository(context.Object);
        }

        [Test]
        public void ShouldReturnAllCarsWhenSuccess()
        {


            var result = _repo.GetAllCars();
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void ShouldReturnCarWhenFound()
        {
            var result = _repo.FindCarById(2);
            Assert.NotNull(result);
            Assert.AreEqual(result.Model,"D");
        }

        [Test]
        public void ShouldCreateCarWhenSuccess()
        {
            _repo.Create(new Car
            {
                Id = 3,
                Make = "E",
                Model = "F",
                Type = Car.CarType.Diesel.ToString(),
                Vin = "C1",
                Milage = 1,
                Year = 2000
            });
            var result = _repo.FindCarById(3);
            Assert.NotNull(result);
        }
        [Test]
        public void ShouldDeleteCarWhenSuccess()
        {
            var car = _repo.FindCarById(2);
            _repo.Delete(car);
            Assert.AreEqual(_carData.Count,1);
        }
        [Test]
        public void ShouldDeleteAllTasksWhenSuccess()
        {
            _repo.DeleteMaintainceTasksFromCar(new Car
            {
                Id = 2
            });
            var car = _repo.FindCarById(2);
            Assert.AreEqual(car.MaintainceTasks.Count,1);
        }
        [Test]
        public void ShouldAddTasksWhenSuccess()
        {
            var tasksToBeAdded = new List<MaintainceTask>
            {
                new MaintainceTask
                {
                    Id = 4,
                    LastTime = DateTime.Now,
                    NextTime = DateTime.Now,
                    Type = MaintainceTask.TaskType.TimingBelt.ToString(),
                    CarId = 2
                },
                new MaintainceTask
                {
                    Id = 5,
                    LastTime = DateTime.Now,
                    NextTime = DateTime.Now,
                    Type = MaintainceTask.TaskType.TireRotation.ToString(),
                    CarId = 2
                }
            };
            _repo.AddMaintainceTasksToCar(tasksToBeAdded);
            Assert.AreEqual(_taskData.Count, 5);
        }
    }
}
