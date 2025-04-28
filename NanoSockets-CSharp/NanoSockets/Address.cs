using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
#if NET7_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif

#pragma warning disable CS1591
#pragma warning disable CS8632

// ReSharper disable ALL

namespace NanoSockets
{
    [StructLayout(LayoutKind.Explicit, Size = 20)]
    public struct Address : IEquatable<Address>
    {
        public static ref Address NullRef => ref MemoryMarshal.GetReference(Span<Address>.Empty);

        public Span<byte> IPv6
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref Unsafe.As<Address, byte>(ref Unsafe.AsRef(in this)), 16);
        }

        public Span<byte> IPv4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref Unsafe.Add(ref Unsafe.As<Address, byte>(ref Unsafe.AsRef(in this)), 12), 4);
        }

        [FieldOffset(16)] public ushort Port;

        public bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref var reference = ref Unsafe.As<Address, long>(ref Unsafe.AsRef(in this));
                return reference != 0 || Unsafe.Add(ref reference, 1) != 0;
            }
        }

        public bool IsIPv4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref var reference = ref Unsafe.As<Address, ulong>(ref Unsafe.AsRef(in this));
                return reference == 0 && Unsafe.Add(ref reference, 1) >> 32 == 0xFFFF;
            }
        }

        public bool IsIPv6
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !IsIPv4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CreateFromIP(ReadOnlySpan<char> ip, out Address address)
        {
            address = new Address();
            return UDP.SetIP(ref address, ip) == Status.OK;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CreateFromHostName(ReadOnlySpan<char> name, out Address address)
        {
            address = new Address();
            return UDP.SetHostName(ref address, name) == Status.OK;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIP(int bufferSize, out string? ip)
        {
            Span<byte> buffer = stackalloc byte[bufferSize];
            var status = UDP.GetIP(ref Unsafe.AsRef(in this), ref MemoryMarshal.GetReference(buffer), bufferSize);
            if (status == Status.OK)
            {
                ip = Encoding.ASCII.GetString(buffer[..buffer.IndexOf((byte)'\0')]);
                return true;
            }

            ip = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIP(int bufferSize, Span<byte> ip, out int byteCount)
        {
            Span<byte> buffer = stackalloc byte[bufferSize];
            var status = UDP.GetIP(ref Unsafe.AsRef(in this), ref MemoryMarshal.GetReference(buffer), bufferSize);
            if (status == Status.OK)
            {
                byteCount = buffer.IndexOf((byte)'\0');
                if (ip.Length < byteCount)
                    return false;
                Unsafe.CopyBlockUnaligned(ref MemoryMarshal.GetReference(ip), ref MemoryMarshal.GetReference(buffer), (uint)byteCount);
                return true;
            }

            byteCount = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetHostName(int bufferSize, out string? name)
        {
            Span<byte> buffer = stackalloc byte[bufferSize];
            var status = UDP.GetHostName(ref Unsafe.AsRef(in this), ref MemoryMarshal.GetReference(buffer), bufferSize);
            if (status == Status.OK)
            {
                name = Encoding.ASCII.GetString(buffer[..buffer.IndexOf((byte)'\0')]);
                return true;
            }

            name = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetHostName(int bufferSize, Span<byte> name, out int byteCount)
        {
            Span<byte> buffer = stackalloc byte[bufferSize];
            var status = UDP.GetHostName(ref Unsafe.AsRef(in this), ref MemoryMarshal.GetReference(buffer), bufferSize);
            if (status == Status.OK)
            {
                byteCount = buffer.IndexOf((byte)'\0');
                if (name.Length < byteCount)
                    return false;
                Unsafe.CopyBlockUnaligned(ref MemoryMarshal.GetReference(name), ref MemoryMarshal.GetReference(buffer), (uint)byteCount);
                return true;
            }

            byteCount = 0;
            return false;
        }

        public bool Equals(Address other)
        {
#if NET7_0_OR_GREATER
            if (Vector128.IsHardwareAccelerated)
                return Vector128.LoadUnsafe(ref Unsafe.As<Address, byte>(ref Unsafe.AsRef(in this))) == Vector128.LoadUnsafe(ref Unsafe.As<Address, byte>(ref other)) && Port == other.Port;
#endif
            ref var left = ref Unsafe.As<Address, int>(ref Unsafe.AsRef(in this));
            ref var right = ref Unsafe.As<Address, int>(ref other);
            return left == right && Unsafe.Add(ref left, 1) == Unsafe.Add(ref right, 1) && Unsafe.Add(ref left, 2) == Unsafe.Add(ref right, 2) && Unsafe.Add(ref left, 3) == Unsafe.Add(ref right, 3) && Unsafe.Add(ref left, 4) == Unsafe.Add(ref right, 4);
        }

        public override bool Equals(object? obj) => obj is Address socketAddress && Equals(socketAddress);
        public override int GetHashCode() => XxHash32.Hash(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<Address, byte>(ref Unsafe.AsRef(in this)), 16)) ^ Port;

        public override string ToString()
        {
            Span<byte> buffer = stackalloc byte[64];
            return UDP.GetIP(ref Unsafe.AsRef(in this), ref MemoryMarshal.GetReference(buffer), 64) == Status.OK ? Encoding.ASCII.GetString(buffer[..buffer.IndexOf((byte)'\0')]) + ":" + Port : "ERROR";
        }

        public string ToString(int bufferSize)
        {
            Span<byte> buffer = stackalloc byte[bufferSize];
            return UDP.GetIP(ref Unsafe.AsRef(in this), ref MemoryMarshal.GetReference(buffer), bufferSize) == Status.OK ? Encoding.ASCII.GetString(buffer[..buffer.IndexOf((byte)'\0')]) + ":" + Port : "ERROR";
        }

        public static bool operator ==(Address left, Address right) => left.Equals(right);
        public static bool operator !=(Address left, Address right) => !(left == right);

        public Span<byte> AsSpan() => MemoryMarshal.CreateSpan(ref Unsafe.As<Address, byte>(ref Unsafe.AsRef(in this)), 20);
        public ReadOnlySpan<byte> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<Address, byte>(ref Unsafe.AsRef(in this)), 20);
    }
}