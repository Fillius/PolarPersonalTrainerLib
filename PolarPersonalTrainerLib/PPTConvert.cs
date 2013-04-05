using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarPersonalTrainerLib
{
    public struct PPTColumns
    {
        public const string Time = "Time";
        public const string Sport = "Sport";
        public const string Calories = "Calories";
        public const string Duration = "Duration";
        public const string RestingHR = "Resting HR";
        public const string AverageHR = "Average HR";
        public const string MaximumHR = "Maximum HR";
        public const string VO2Max = "VO2 Max";
    }

    public class PPTConvert
    {
        static String[] columnNames =
                { PPTColumns.Time,
                  PPTColumns.Sport,
                  PPTColumns.Calories,
                  PPTColumns.Duration,
                  PPTColumns.RestingHR,
                  PPTColumns.AverageHR,
                  PPTColumns.MaximumHR,
                  PPTColumns.VO2Max };

        public static DataRow convertExerciseToDataRow(PPTExercise exercise, DataTable dt)
        {
            addMissingColumns(ref dt);

            DataRow dr = dt.NewRow();

            dr[PPTColumns.Time] = exercise.time;
            dr[PPTColumns.Sport] = exercise.sport;
            dr[PPTColumns.Calories] = exercise.calories;
            dr[PPTColumns.Duration] = exercise.duration;

            HeartRate hr = exercise.heartRate;

            if (hr != null && hr.hasData())
            {
                dr[PPTColumns.RestingHR] = exercise.heartRate.resting;
                dr[PPTColumns.AverageHR] = exercise.heartRate.average;
                dr[PPTColumns.MaximumHR] = exercise.heartRate.maximum;
                dr[PPTColumns.VO2Max] = exercise.heartRate.vo2Max;
            }

            return dr;
        }

        private static void addMissingColumns(ref DataTable dt)
        {
            foreach (String columnName in columnNames)
            {
                if (!dt.Columns.Contains(columnName))
                    dt.Columns.Add(columnName);
            }
        }
    }
}
