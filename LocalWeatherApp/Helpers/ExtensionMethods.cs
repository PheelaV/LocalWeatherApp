using System;

namespace LocalWeatherApp.Helpers
{
    public static class ExtensionMethods
    {
        public static T CheckForNull<T>(this T nullable)
        {
            if (nullable == null)
            {
                throw new IoCException("Ioc registration error");
            }

            return nullable;
        }

        public class IoCException : Exception
        {
            public IoCException(string message): base(message)
            {
            
            }
        }
    }
}
