using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker
{
    public class Habit
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int habitQuantity { get; set; }


        public Habit(int ID, DateTime Date, int habitQuantity)
        {
            this.ID = ID;
            this.Date = Date;
            this.habitQuantity = habitQuantity;
        }

        public int GetID()
        {
            return this.ID;
        }
        public DateTime GetDateTime()
        {
            return this.Date;
        }

        public int GetQuantity()
        {
            return this.habitQuantity;
        }
    }
}
