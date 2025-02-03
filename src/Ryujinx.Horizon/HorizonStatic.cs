using MsgPack;
using Ryujinx.Horizon.Common;
using Ryujinx.Memory;
using System;
using System.Threading;

namespace Ryujinx.Horizon
{
    public static class HorizonStatic
    {
        internal static void HandlePlayReport(MessagePackObject report) => 
            new Thread(() => PlayReport?.Invoke(report))
            {
                Name = "HLE.PlayReportEvent", 
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            }.Start();
        
        public static event Action<MessagePackObject> PlayReport;
        
        [ThreadStatic]
        private static HorizonOptions _options;

        [ThreadStatic]
        private static ISyscallApi _syscall;

        [ThreadStatic]
        private static IVirtualMemoryManager _addressSpace;

        [ThreadStatic]
        private static IThreadContext _threadContext;

        [ThreadStatic]
        private static int _threadHandle;

        public static HorizonOptions Options => _options;
        public static ISyscallApi Syscall => _syscall;
        public static IVirtualMemoryManager AddressSpace => _addressSpace;
        public static IThreadContext ThreadContext => _threadContext;
        public static int CurrentThreadHandle => _threadHandle;

        public static void Register(
            HorizonOptions options,
            ISyscallApi syscallApi,
            IVirtualMemoryManager addressSpace,
            IThreadContext threadContext,
            int threadHandle)
        {
            _options = options;
            _syscall = syscallApi;
            _addressSpace = addressSpace;
            _threadContext = threadContext;
            _threadHandle = threadHandle;
        }
    }
}
