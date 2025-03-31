using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;

#pragma warning disable CS1591
#pragma warning disable SYSLIB1054
#pragma warning disable CA1401
#pragma warning disable CA2101

// ReSharper disable ALL

namespace NanoSockets
{
    [SuppressUnmanagedCodeSecurity]
    public static class UDP
    {
#if __IOS__ || (UNITY_IOS && !UNITY_EDITOR)
        private const string NATIVE_LIBRARY = "__Internal";
#else
        private const string NATIVE_LIBRARY = "nanosockets";
#endif

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_initialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status Initialize();

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_deinitialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Deinitialize();

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_create", CallingConvention = CallingConvention.Cdecl)]
        public static extern Socket Create(int sendBufferSize, int receiveBufferSize);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Destroy(ref Socket socket);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_bind", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Bind(Socket socket, ref Address address);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_connect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Connect(Socket socket, ref Address address);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_set_option", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status SetOption(Socket socket, SocketOptionLevel level, SocketOptionName optionName, ref int optionValue, int optionLength);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_get_option", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status GetOption(Socket socket, SocketOptionLevel level, SocketOptionName optionName, ref int optionValue, ref int optionLength);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_set_nonblocking", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status SetNonBlocking(Socket socket, byte state);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_set_dontfragment", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status SetDontFragment(Socket socket);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_poll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Poll(Socket socket, long milliseconds);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_receive", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Receive(Socket socket, ref Address address, ref byte buffer, int bufferLength);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_send", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Send(Socket socket, ref Address address, ref byte buffer, int bufferLength);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_address_get", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status GetAddress(Socket socket, ref Address address);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_address_set_ip", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status SetIP(ref Address address, ref byte ip);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_address_get_ip", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status GetIP(ref Address address, ref byte ip, int ipLength);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_address_set_hostname", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status SetHostName(ref Address address, ref byte name);

        [DllImport(NATIVE_LIBRARY, EntryPoint = "nanosockets_address_get_hostname", CallingConvention = CallingConvention.Cdecl)]
        public static extern Status GetHostName(ref Address address, ref byte name, int nameLength);
    }
}