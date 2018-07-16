namespace AutomobileMaintenanceTracker.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class MaintainceTask
    {
        public enum TaskType { OilChange, TireRotation, TimingBelt };

        public int Id { get; set; }
        [Required]
        public int CarId { get; set; }
        
        [Required]
        public string Type
        {
            get { return TaskTypeEnum.ToString(); }
            set
            {
                TaskType newValue;
                if (Enum.TryParse(value, out newValue))
                {
                    TaskTypeEnum = newValue;
                }
            }

        }
        [NotMapped]
        private TaskType TaskTypeEnum { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime LastTime { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime NextTime { get; set; }

        public virtual Car Car { get; set; }
    }
}
