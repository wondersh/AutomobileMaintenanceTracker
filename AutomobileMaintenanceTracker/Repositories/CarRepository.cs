using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutomobileMaintenanceTracker.Models;

namespace AutomobileMaintenanceTracker.Repositories
{
    public interface ICarRepository
    {
        void Create(Car car);
        void Update(Car car);
        void Delete(Car car);
        Car FindCarById(int id);
        void ChangeCarType(Car car, Car.CarType type);
        List<Car> GetAllCars();
        void DeleteMaintainceTasksFromCar(Car car);
        void AddMaintainceTasksToCar(ICollection<MaintainceTask> tasks);
    }

    public class CarRepository : ICarRepository
    {
        private readonly AMTDbContext _db;  //TODO add DI later

        public CarRepository(AMTDbContext db)
        {
            _db = db;
        }

        public void Create(Car car)
        {
            _db.Cars.Add(car);
            _db.SaveChanges();
        }

        public void Update(Car car)
        {
            _db.Entry(car).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void Delete(Car car)
        {
            _db.Cars.Remove(car);
            _db.SaveChanges();
        }

        public Car FindCarById(int id)
        {
            return _db.Cars.FirstOrDefault(c => c.Id == id);
        }

        public void ChangeCarType(Car car, Car.CarType type)
        {
            car.Type = type.ToString();
            _db.Entry(car).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public List<Car> GetAllCars()
        {
            return _db.Cars.ToList();
        }

        public void DeleteMaintainceTasksFromCar(Car car)
        {
            var tasks = _db.MaintainceTasks.Where(e => e.CarId == car.Id).ToList();
            _db.MaintainceTasks.RemoveRange(tasks);
            _db.SaveChanges();
        }

        public void AddMaintainceTasksToCar(ICollection<MaintainceTask> tasks)
        {
            foreach (var m in tasks)
            {
                _db.MaintainceTasks.Add(m);
                _db.SaveChanges();
            }
        }
    }
}
