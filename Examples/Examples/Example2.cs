using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using NanoSockets;

namespace Examples
{
    public sealed unsafe class Example2
    {
        public static void StartServer(ushort port, string localIP = "::0")
        {
            UDP.Initialize();

            Socket server = Socket.Create(256 * 1024, 256 * 1024);

            Address.CreateFromIP(localIP, out Address listenAddress);
            listenAddress.Port = port;

            if (server.Bind(listenAddress) == 0)
                Console.WriteLine("Socket bound!");

            if (!server.SetDontFragment())
                Console.WriteLine("Don't fragment option error!");

            if (!server.SetNonBlocking(true))
                Console.WriteLine("Non-blocking option error!");

            Address address = new Address();
            Span<byte> buffer = stackalloc byte[1024];

            while (!Console.KeyAvailable)
            {
                if (server.Poll(15) > 0)
                {
                    int dataLength;

                    while ((dataLength = server.Receive(ref address, buffer)) > 0)
                    {
                        string data = Encoding.UTF8.GetString(buffer.Slice(0, dataLength));

                        Console.WriteLine($"Message received from {address.ToString()}: " + data);
                    }
                }
            }

            server.Dispose();

            UDP.Deinitialize();
        }

        public static void StartClient(string serverIP, ushort port)
        {
            UDP.Initialize();

            Socket client = Socket.Create(256 * 1024, 256 * 1024);
            Address.CreateFromIP(serverIP, out Address connectionAddress);

            connectionAddress.Port = port;

            if (client.Connect(connectionAddress) == 0)
                Console.WriteLine("Socket connected!");

            if (!client.SetDontFragment())
                Console.WriteLine("Don't fragment option error!");

            if (!client.SetNonBlocking(true))
                Console.WriteLine("Non-blocking option error!");

            Span<byte> buffer = stackalloc byte[1024];

            ref Address remoteAddress = ref Unsafe.AsRef<Address>(null);

            int bytes = Encoding.UTF8.GetBytes("hello server.", buffer);

            client.Send(remoteAddress, buffer.Slice(0, bytes));

            byte i = 0;

            while (!Console.KeyAvailable)
            {
                bytes = Encoding.UTF8.GetBytes($"test send {i++}.", buffer);

                client.Send(remoteAddress, buffer.Slice(0, bytes));

                Thread.Sleep(1000);
            }

            client.Dispose();

            UDP.Deinitialize();
        }
    }
}