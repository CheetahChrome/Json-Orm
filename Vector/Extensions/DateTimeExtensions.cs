using System;

namespace JSON.ORM.Vector.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Check whether time is in between datetime.Min and datetime.max to determine validity. 
        /// </summary>
        /// <param name="time">Time to check</param>
        /// <returns>False if it is on the extremes; meaning it has not been set.</returns>
        public static bool IsDateTimeValid(this DateTime time)
            => time > DateTime.MinValue && time < DateTime.MaxValue;
    }
}
