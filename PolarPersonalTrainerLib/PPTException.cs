using System;
using System.Net;

namespace PolarPersonalTrainerLib
{
    /// <summary>
    /// Custom exception class to be used to gather response data when there's an error
    /// </summary>
    public class PPTException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public PPTException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            this.HttpStatusCode = statusCode;
        }

        public PPTException(string message)
            : base(message)
        {
        }
    }
}
