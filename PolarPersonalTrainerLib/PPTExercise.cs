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

        public List<HeartBeat> HeartBeats { get; set; }

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
        /// <summary>
        /// Gets or sets the distance for the excercise
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Name of the exercise
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates that the excercise has gps data available for export from PPT.com
        /// </summary>
        public bool HasGPSData { get; set; }
    }

    public class HeartBeat
    {
        public int HeartRate { get; set; }
        public DateTime Time { get; set; }
    }
}
