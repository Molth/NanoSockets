using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS8632

namespace NanoSockets
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct Socket : IDisposable, IEquatable<Socket>
    {
        [FieldOffset(0)] public long Handle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Socket Create(int sendBufferSize, int receiveBufferSize)
        {
            UDP.Initialize();
            return UDP.Create(sendBufferSize, receiveBufferSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            UDP.Destroy(ref this);
            UDP.Deinitialize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Bind(in Address address) => UDP.Bind(this, ref Unsafe.AsRef(in address));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Connect(in Address address) => UDP.Connect(this, ref Unsafe.AsRef(in address));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetOption(SocketOptionLevel level, SocketOptionName optionName, ref int optionValue, int optionLength) => UDP.SetOption(this, level, optionName, ref optionValue, optionLength) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetOption(SocketOptionLevel level, SocketOptionName optionName, ref int optionValue, ref int optionLength) => UDP.GetOption(this, level, optionName, ref optionValue, ref optionLength) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetNonBlocking(bool nonBlocking) => UDP.SetNonBlocking(this, (byte)(nonBlocking ? 1 : 0)) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetDontFragment() => UDP.SetDontFragment(this) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Poll(long milliseconds) => UDP.Poll(this, milliseconds);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Receive(ref Address address, Span<byte> buffer) => UDP.Receive(this, ref address, ref MemoryMarshal.GetReference(buffer), buffer.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Send(in Address address, ReadOnlySpan<byte> buffer) => UDP.Send(this, ref Unsafe.AsRef(in address), ref MemoryMarshal.GetReference(buffer), buffer.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetAddress(ref Address address) => UDP.GetAddress(this, ref address) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetSendBufferSize(out int sendBufferSize)
        {
            sendBufferSize = 0;
            var optionLength = 4;
            return UDP.GetOption(Handle, SocketOptionLevel.Socket, SocketOptionName.SendBuffer, ref sendBufferSize, ref optionLength) == Status.OK;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetSendBufferSize(int sendBufferSize) => UDP.SetOption(Handle, SocketOptionLevel.Socket, SocketOptionName.SendBuffer, ref sendBufferSize, 4) == Status.OK;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetReceiveBufferSize(out int receiveBufferSize)
        {
            receiveBufferSize = 0;
            var optionLength = 4;
            return UDP.GetOption(Handle, SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, ref receiveBufferSize, ref optionLength) == Status.OK;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SetReceiveBufferSize(int receiveBufferSize) => UDP.SetOption(Handle, SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, ref receiveBufferSize, 4) == Status.OK;

        public bool Equals(Socket other) => Handle == other.Handle;
        public override bool Equals(object? obj) => obj is Socket socket && Equals(socket);
        public override int GetHashCode() => Handle.GetHashCode();

        public override string ToString() => $"Socket[{Handle}]";

        public static bool operator ==(Socket left, Socket right) => left.Equals(right);
        public static bool operator !=(Socket left, Socket right) => !(left == right);

        public static implicit operator bool(Socket socket) => socket.Handle > 0;
        public static implicit operator long(Socket socket) => socket.Handle;
        public static implicit operator Socket(long handle) => Unsafe.As<long, Socket>(ref handle);
    }
}