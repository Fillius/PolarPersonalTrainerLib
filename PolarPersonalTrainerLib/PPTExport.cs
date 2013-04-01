using HtmlAgilityPack;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace PolarPersonalTrainerLib
{
    public class PPTExport
    {
        private XmlDocument xml;
        private String username;
        private String password;

        // Persistent cookie container for all requests
        private CookieContainer cookieJar;

        public PPTExport(String username, String password)
        {
            this.password = password;
            this.username = username;
            cookieJar = new CookieContainer();
        }

        public XmlDocument getXml()
        {
            return this.xml;
        }

        private HttpWebRequest newHttpWebRequest(String url, String requestMethod)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.CookieContainer = cookieJar;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.22 (KHTML, like Gecko) Chrome/25.0.1364.172 Safari/537.2";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = 5000;
            request.Method = requestMethod;

            return request;
        }

        private String postRequest(String url, String strPost)
        {
            HttpWebRequest request = newHttpWebRequest(url, "POST");

            // Turn string into a byte stream
            byte[] postBytes = Encoding.ASCII.GetBytes(strPost);

            request.ContentLength = postBytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response == null)
                    throw new InvalidOperationException(String.Format("POST request to {1} did not get a reponse", url));

                using (var reader = new StreamReader(response.GetResponseStream()))
                    return reader.ReadToEnd();
            }
        }

        private String getRequest(String url)
        {
            HttpWebRequest request = newHttpWebRequest(url, "GET");

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response == null)
                    throw new InvalidOperationException(String.Format("GET request from {1} did not get a reponse", url));

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private String getTrainingSessions(HtmlNode calItems)
        {
            int itemCount = 0;
            NameValueCollection keyValues = new NameValueCollection();

            keyValues.Add(".action", "export");
            keyValues.Add(".filename", "export.xml");

            foreach (HtmlNode row in calItems.SelectNodes("./tr") ?? Enumerable.Empty<HtmlNode>())
            {
                if (row.GetAttributeValue("class", "").Equals("listHeadRow"))
                    continue;

                foreach (HtmlNode cell in row.SelectNodes("./td") ?? Enumerable.Empty<HtmlNode>())
                {
                    // Check if the training Type is OptimizedExcercise (training data which has a sport assigned)
                    HtmlNode itemType = cell.SelectSingleNode("./input[@name='calendarItemTypes']");

                    if (itemType == null)
                        continue;

                    if (!itemType.GetAttributeValue("value", "").Equals("OptimizedExercise"))
                        continue;

                    HtmlNode itemValue = cell.SelectSingleNode("./input[@name='calendarItem']");

                    if (itemValue == null)
                        continue;

                    keyValues.Add("items." + itemCount + ".item", itemValue.GetAttributeValue("value", ""));
                    keyValues.Add("items." + itemCount++ + ".itemType", "OptimizedExercise");
                }
            }

            if (keyValues.Count <= 2)
                return null;

            var strPost = "";

            foreach (string key in keyValues)
            {
                strPost += key + "=" + WebUtility.UrlEncode(keyValues[key]) + "&";
            }

            strPost.Remove(strPost.Length - 1, 1); // remove the last '&'

            return strPost;
        }

        public XmlDocument downloadSessions(DateTime startDate, DateTime endDate)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 1600;

            // Attempt login
            var url = "https://www.polarpersonaltrainer.com/index.ftl";
            var strPost = "email=" + username + "&password=" + password + "&.action=login&tz=0";

            postRequest(url, strPost);

            url = "https://www.polarpersonaltrainer.com/user/calendar/inc/listview.ftl?" +
                "startDate=" + startDate.ToShortDateString() + "&endDate=" + endDate.ToShortDateString();

            // Attempt to get the list of training sessions for the requested dates
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(getRequest(url));

            var calItems = doc.GetElementbyId("calItems");

            if (calItems == null)
            {
                throw new InvalidDataException("Could not find the HTML table 'calItems' containing training results");
            }
            
            url = "https://www.polarpersonaltrainer.com/user/calendar/index.jxml";

            strPost = getTrainingSessions(calItems);

            if (strPost == null)
            {
                throw new InvalidDataException("No training sessions found");
            }

            // Attempt to export the XML file for the excercises found above
            xml = new XmlDocument();
            xml.LoadXml(postRequest(url, strPost));

            return xml;
        }
    }
}
