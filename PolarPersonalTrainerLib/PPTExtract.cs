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
            // Need a prefix for the default namespace so it can be used in Xpath queries later on
            namespaceManager.AddNamespace("x", "http://www.polarpersonaltrainer.com");
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

                if (sportNode == null && requireSport)
                    continue;

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
                // Added support for distance and name
                exercise.Distance = double.Parse(resultNode["distance"].Value ?? "0");
                exercise.Name = exerciseNode["name"].Value ?? string.Empty;
                // If there exist a running-index gps data has to exist
                exercise.HasGPSData = resultNode["running-index"] != null;
                
                if (sportNode != null)
                    exercise.sport = sportNode.InnerText;

                HeartRate hr = new HeartRate();

                // Added support for heart beats
                try
                {
          
                    int recordRate = int.Parse(resultNode["recording-rate"].InnerText);
                    var heartbeats = resultNode.SelectSingleNode("//x:sample[x:type/text() = 'HEARTRATE']/x:values", namespaceManager).InnerText.Split(new char[] { ',' });
                    hr.HeartBeats = heartbeats.Select((v, i) => new HeartBeat() { HeartRate = int.Parse(v), Time = exercise.time.AddSeconds(i * recordRate) }).ToList();
                }
                catch { }

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
