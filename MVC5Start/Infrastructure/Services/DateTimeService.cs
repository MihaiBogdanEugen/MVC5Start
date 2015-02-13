using System;

namespace MVC5Start.Infrastructure.Services
{
    public class DateTimeService
    {
        public static DateTime Now
        {
            get { return DateTime.UtcNow; }
        }

        public static int CurrentYear
        {
            get { return DateTime.UtcNow.Year; }
        }
    }
}