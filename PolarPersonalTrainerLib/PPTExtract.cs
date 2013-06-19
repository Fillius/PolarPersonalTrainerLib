using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PolarPersonalTrainerLib
{
    public class PPTExtract
    {
        public static List<PPTExercise> convertXmlToExercises(XmlDocument xml, bool requireSport = false)
        {
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xml.NameTable);
            XmlNodeList xmlNodes = xml.GetElementsByTagName("exercise");
            
            if (xmlNodes == null)
                throw new InvalidDataException("No Polar exercises found");

            List<PPTExercise> exercises = new List<PPTExercise>();

            foreach (XmlElement exerciseNode in xmlNodes)
            {
                PPTExercise exercise = new PPTExercise();

                XmlNode timeNode = exerciseNode["time"];
                XmlNode sportNode = exerciseNode["sport"];
                XmlElement resultNode = (XmlElement)exerciseNode["result"];

                if (timeNode == null  || resultNode == null)
                    continue;

                if (requireSport && sportNode == null)
                    continue;
                else
                    exercise.sport = sportNode.InnerText;

                XmlNode caloriesNode = resultNode["calories"];
                XmlNode durationNode = resultNode["duration"];
                XmlElement hrNode = (XmlElement)resultNode["heart-rate"];
                XmlElement userNode = (XmlElement)resultNode["user-settings"];
                XmlElement hrUserNode = (XmlElement)userNode["heart-rate"];
                XmlNode vo2MaxNode = userNode["vo2max"];

                if (caloriesNode == null || durationNode == null)
                    continue;

                exercise.time = DateTime.Parse(timeNode.InnerText);
                exercise.calories = Convert.ToInt32(caloriesNode.InnerText);
                exercise.duration = TimeSpan.Parse(durationNode.InnerText);

                HeartRate hr = new HeartRate();

                if (hrNode != null)
                {
                    XmlNode averageNode = hrNode["average"];
                    XmlNode maximumNode = hrNode["maximum"];

                    if (maximumNode != null)
                        hr.maximum = Convert.ToInt32(maximumNode.InnerText);

                    if (averageNode != null)
                        hr.average = Convert.ToInt32(averageNode.InnerText);
                }

                if (hrUserNode != null)
                {
                    XmlNode restingNode = hrUserNode["resting"];

                    if (restingNode != null)
                        hr.resting = Convert.ToInt32(restingNode.InnerText);
                }

                if (vo2MaxNode != null)
                    hr.vo2Max = Convert.ToInt32(vo2MaxNode.InnerText);

                exercise.heartRate = hr;

                exercises.Add(exercise);
            }

            return exercises;
        }
    }
}
