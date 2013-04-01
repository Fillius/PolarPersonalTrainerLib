using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarPersonalTrainerLib
{
    public class PPTConvert
    {
        private static String[] columns = { "Time",
                             "Sport",
                             "Calories",
                             "Duration",
                             "Resting HR",
                             "Average HR",
                             "Maximum HR",
                             "VO2 Max" };

        public static DataRow convertExerciseToDataRow(PPTExercise exercise, DataTable dt)
        {
            addMissingColumns(ref dt);

            DataRow dr = dt.NewRow();

            dr["Time"] = exercise.time;
            dr["Sport"] = exercise.sport;
            dr["Calories"] = exercise.calories;
            dr["Duration"] = exercise.duration;

            HeartRate hr = exercise.heartRate;

            if (hr != null && hr.hasData())
            {
                dr["Resting HR"] = exercise.heartRate.resting;
                dr["Average HR"] = exercise.heartRate.average;
                dr["Maximum HR"] = exercise.heartRate.maximum;
                dr["VO2 Max"] = exercise.heartRate.vo2Max;
            }

            return dr;
        }

        private static void addMissingColumns(ref DataTable dt)
        {
            foreach (String column in columns)
            {
                if (!dt.Columns.Contains(column))
                    dt.Columns.Add(column);
            }
        }
    }
}
