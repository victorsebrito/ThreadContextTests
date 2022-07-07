#if NETFRAMEWORK
using System.Runtime.Remoting.Messaging;
#else
using System.Threading;
#endif

namespace ThreadContextTests
{
    /// <summary>
    /// Uses CallContext for .NET Framework and AsyncLocal/ThreadLocal
    /// for .NET Core
    /// </summary>
    internal static class Context
    {
#if NETFRAMEWORK
        private const string KEY = nameof(Context);
#else
        static readonly AsyncLocal<object> _asyncLocal = new();
        static readonly ThreadLocal<object> _threadLocal = new();
#endif

        public static object GetData()
        {
#if NETFRAMEWORK
            return CallContext.GetData(KEY);
#else
            return _threadLocal.Value;
#endif
        }

        public static void SetData(object data)
        {
#if NETFRAMEWORK
            CallContext.SetData(KEY, data);
#else
            _threadLocal.Value = data;
#endif
        }

        public static object LogicalGetData()
        {
#if NETFRAMEWORK
            return CallContext.LogicalGetData(KEY);
#else
            return _asyncLocal.Value;
#endif
        }

        public static void LogicalSetData(object data)
        {
#if NETFRAMEWORK
            CallContext.LogicalSetData(KEY, data);
#else
            _asyncLocal.Value = data;
#endif
        }
    }
}