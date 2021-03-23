using System;

namespace FMSC.ORM.Logging
{
    public static class LoggerProvider
    {
        private static ILogger Instance { get; set; } = new DefaultLogger();
        private static Func<ILogger> CreateFunc { get; set; }

        public static ILogger Get()
        {
            var logger = CreateFunc?.Invoke() ?? Instance;
            return logger ?? throw new InvalidOperationException("Logger Provider not initialized properly, Get returned null");
        }

        public static void Register(ILogger logger)
        {
            if (logger is null) { throw new ArgumentNullException(nameof(logger)); }
            Instance = logger;
        }

        public static void Register(Func<ILogger> func)
        {
            if (func is null) { throw new ArgumentNullException(nameof(func)); }
            CreateFunc = func;
        }
    }
}