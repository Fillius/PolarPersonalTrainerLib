using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarPersonalTrainerLib
{
    public class HeartRate
    {
        public int resting = 0;
        public int average = 0;
        public int maximum = 0;
        public int vo2Max = 0;

        public Boolean hasData()
        {
            if (resting <= 0 && average <= 0 && maximum <= 0 && vo2Max <= 0)
                return false;

            return true;
        }
    }

    public class PPTExercise
    {
        public DateTime time { get; set; }
        public String sport { get; set; }
        public int calories { get; set; }
        public TimeSpan duration { get; set; }
        public HeartRate heartRate { get; set; }
    }
}
