using System.Linq;

namespace AutomobileMaintenanceTracker.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Car
    {
        private readonly Dictionary<CarType, List<MaintainceTask.TaskType>> _taskMapping =
            new Dictionary<CarType, List<MaintainceTask.TaskType>>
            {
                {CarType.Diesel, new List<MaintainceTask.TaskType>{MaintainceTask.TaskType.OilChange,MaintainceTask.TaskType.TireRotation,MaintainceTask.TaskType.TimingBelt}},
                {CarType.Gas, new List<MaintainceTask.TaskType>{MaintainceTask.TaskType.OilChange,MaintainceTask.TaskType.TireRotation}},
                {CarType.Electric, new List<MaintainceTask.TaskType>{MaintainceTask.TaskType.TireRotation}}
            };
        public enum CarType { Electric, Gas, Diesel };

        public Car()
        {
            MaintainceTasks = new HashSet<MaintainceTask>();
        }

        [Key]
        public int Id { get; set; }
        
        [NotMapped]
        private CarType CarTypeEnum { get; set; }

        [Required]
        public string Type {
            get { return CarTypeEnum.ToString(); }
            set
            {
                CarType newValue;
                if (Enum.TryParse(value, out newValue))
                {
                    CarTypeEnum = newValue;
                }
            }

        }

        [Required]
        public string Vin { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string Make { get; set; }

        public int Year { get; set; }

        [Required]
        public int Milage { get; set; }

        public virtual ICollection<MaintainceTask> MaintainceTasks { get; set; }
        
        public List<MaintainceTask> GetMaintainceTasksByType()
        {
            var result = new List<MaintainceTask>();
            foreach (var type in _taskMapping[CarTypeEnum])
            {
                var task = MaintainceTasks.ToList().FirstOrDefault(m => m.Type == type.ToString());
                if (task != null)
                {
                    result.Add(task);
                }
                else
                {
                    result.Add(new MaintainceTask
                    {
                        CarId = Id,
                        Type = type.ToString(),
                        LastTime = DateTime.Now,
                        NextTime = DateTime.Now
                    });
                }
            }
            return result;
        }
    }
}
