using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NanoSockets;

namespace Examples
{
    // unsafe
    public sealed unsafe class Example1
    {
        public static void StartServer(ushort port, string localIP = "::0")
        {
            UDP.Initialize();

            Socket server = UDP.Create(256 * 1024, 256 * 1024);

            Address listenAddress = new Address();
            listenAddress.Port = port;

            int ipByteCount = Encoding.ASCII.GetByteCount(localIP);
            byte* addressIPBuffer = stackalloc byte[ipByteCount];
            Encoding.ASCII.GetBytes(localIP, MemoryMarshal.CreateSpan(ref *addressIPBuffer, ipByteCount));

            if (UDP.SetIP(ref listenAddress, ref *addressIPBuffer) == Status.OK)
                Console.WriteLine("Address set!");

            if (UDP.Bind(server, ref listenAddress) == 0)
                Console.WriteLine("Socket bound!");

            if (UDP.SetNonBlocking(server, 1) != Status.OK)
                Console.WriteLine("Non-blocking option error!");

            Address address = new Address();
            byte* buffer = stackalloc byte[1024];

            while (!Console.KeyAvailable)
            {
                if (UDP.Poll(server, 15) > 0)
                {
                    int dataLength;

                    while ((dataLength = UDP.Receive(server, ref address, ref *buffer, 1024)) > 0)
                    {
                        string data = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpan(ref *buffer, dataLength));

                        Console.WriteLine($"Message received from {address.ToString()}: " + data);
                    }
                }
            }

            UDP.Destroy(ref server);

            UDP.Deinitialize();
        }

        public static void StartClient(string serverIP, ushort port)
        {
            UDP.Initialize();

            Socket client = UDP.Create(256 * 1024, 256 * 1024);
            Address connectionAddress = new Address();

            connectionAddress.Port = port;

            int ipByteCount = Encoding.ASCII.GetByteCount(serverIP);
            byte* addressIPBuffer = stackalloc byte[ipByteCount];
            Encoding.ASCII.GetBytes(serverIP, MemoryMarshal.CreateSpan(ref *addressIPBuffer, ipByteCount));

            if (UDP.SetIP(ref connectionAddress, ref *addressIPBuffer) == Status.OK)
                Console.WriteLine("Address set!");

            if (UDP.Connect(client, ref connectionAddress) == 0)
                Console.WriteLine("Socket connected!");

            if (UDP.SetNonBlocking(client, 1) != Status.OK)
                Console.WriteLine("Non-blocking option error!");

            byte* buffer = stackalloc byte[1024];

            int bytes = Encoding.UTF8.GetBytes("hello server.", MemoryMarshal.CreateSpan(ref *buffer, 1024));

            UDP.Send(client, ref *buffer, bytes);

            byte i = 0;

            while (!Console.KeyAvailable)
            {
                bytes = Encoding.UTF8.GetBytes($"test send {i++}.", MemoryMarshal.CreateSpan(ref *buffer, 1024));

                UDP.Send(client, ref *buffer, bytes);

                Thread.Sleep(1000);
            }

            UDP.Destroy(ref client);

            UDP.Deinitialize();
        }
    }
}