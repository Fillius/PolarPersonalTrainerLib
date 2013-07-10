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
        public const string Duration = "Duration";
        public const string Calories = "Calories";
        public const string Sport = "Sport";
        public const string AverageHR = "Average HR";
        public const string MaximumHR = "Maximum HR";
        public const string RestingHR = "Resting HR";
        public const string VO2Max = "VO2 Max";
    }

    public class PPTConvert
    {
        static String[] columnNames =
                { PPTColumns.Time,
                  PPTColumns.Duration,
                  PPTColumns.Calories,
                  PPTColumns.Sport,
                  PPTColumns.AverageHR,
                  PPTColumns.MaximumHR,
                  PPTColumns.RestingHR,
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

        public static PPTExercise convertDataRowToExercise(DataRow dr)
        {
            PPTExercise exercise = new PPTExercise();

            exercise.time = Convert.ToDateTime(dr[PPTColumns.Time]);
            exercise.sport = dr[PPTColumns.Sport].ToString();
            exercise.calories = Convert.ToInt32(dr[PPTColumns.Calories]);
            exercise.duration = TimeSpan.Parse(dr[PPTColumns.Duration].ToString());

            exercise.heartRate = new HeartRate();

            exercise.heartRate.resting = Convert.ToInt32(dr[PPTColumns.RestingHR]);
            exercise.heartRate.average = Convert.ToInt32(dr[PPTColumns.AverageHR]);
            exercise.heartRate.maximum = Convert.ToInt32(dr[PPTColumns.MaximumHR]);
            exercise.heartRate.vo2Max = Convert.ToInt32(dr[PPTColumns.VO2Max]);

            return exercise;
        }

        private static void addTypedColumn(ref DataTable dt, String columnName, Type columnType)
        {
            if (!dt.Columns.Contains(columnName))
                dt.Columns.Add(columnName, columnType);
        }

        private static void addMissingColumns(ref DataTable dt)
        {
            addTypedColumn(ref dt, PPTColumns.Time, typeof(DateTime));
            addTypedColumn(ref dt, PPTColumns.Duration, typeof(TimeSpan));
            addTypedColumn(ref dt, PPTColumns.Calories, typeof(int));
            addTypedColumn(ref dt, PPTColumns.Sport, typeof(string));
            addTypedColumn(ref dt, PPTColumns.AverageHR, typeof(int));
            addTypedColumn(ref dt, PPTColumns.MaximumHR, typeof(int));
            addTypedColumn(ref dt, PPTColumns.RestingHR, typeof(int));
            addTypedColumn(ref dt, PPTColumns.VO2Max, typeof(int));
        }
    }
}
