﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using TomP2P.Extensions.Workaround;

namespace TomP2P.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Counts the leading zeros of this integer.
        /// </summary>
        /// <param name="x">The integer.</param>
        /// <returns>The amount of leading zeros in this integer.</returns>
        public static int LeadingZeros(this int x)
        {
            // taken from http://stackoverflow.com/questions/10439242/count-leading-zeroes-in-an-int32
            // see also http://aggregate.org/MAGIC/
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);
            return (sizeof(int) * 8 - Ones(x));
        }

        private static int Ones(int x)
        {
            x -= ((x >> 1) & 0x55555555);
            x = (((x >> 2) & 0x33333333) + (x & 0x33333333));
            x = (((x >> 4) + x) & 0x0f0f0f0f);
            x += (x >> 8);
            x += (x >> 16);
            return (x & 0x0000003f);
        }

        public static sbyte[] ComputeHash(this string x)
        {
            HashAlgorithm algorithm = SHA1.Create();
            return (sbyte[])(Array)algorithm.ComputeHash(Encoding.UTF8.GetBytes(x)); // TODO test double cast
        }

        /// <summary>
        /// Equivalent to Java's Queue.peek() that returns null if empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static T Peek2<T>(this Queue<T> q) where T : class
        {
            try
            {
                return q.Peek();
            }
            catch (InvalidOperationException ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Convert a BitArray to a byte. (Only takes first 8 bits.)
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static sbyte ToByte(this BitArray ba)
        {
            sbyte b = 0;
            for (int i = 0; i < 8; i++)
            {
                if (ba.Get(i))
                {
                    b |= (sbyte)(1 << i); // TODO test
                }
            }
            return b;
        }

        /// <summary>
        /// Equivalent to Java's BitSet.toByteArray().
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static sbyte[] ToByteArray(this BitArray ba)
        {
            // TODO check if same result as with Java
            var bytes = new sbyte[(ba.Length - 1) / 8 + 1];
            ba.CopyTo(bytes, 0);
            return bytes;
        }

        /// <summary>
        /// Equivalent to Java's BitSet.flip(int, int).
        /// Sets each bit from the specified fromIndex (inclusive) to the specified toIndex (exclusive) to the complement of its current value.
        /// </summary>
        /// <param name="ba"></param>
        /// <param name="fromIndex"></param>
        /// <param name="toIndex"></param>
        public static void Flip(this BitArray ba, int fromIndex, int toIndex)
        {
            for (int i = fromIndex; i < toIndex; i++) // TODO check if works correctly
            {
                ba[i] = !ba[i];
            }
        }

        /// <summary>
        /// Equivalent to Java's BitSet.equals() implementation.
        /// </summary>
        /// <param name="ba"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool Equals2(this BitArray ba, BitArray other)
        {
            if (ba.Length != other.Length)
            {
                return false;
            }
            for (int i = 0; i < ba.Length; i++)
            {
                if (ba[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsIPv4(this IPAddress ip)
        {
            return ip.AddressFamily == AddressFamily.InterNetwork;
        }

        public static bool IsIPv6(this IPAddress ip)
        {
            return ip.AddressFamily == AddressFamily.InterNetworkV6;
        }

        #region Java ByteBuffer

        /// <summary>
        /// Equivalent of Java's ByteBuffer.remaining().
        /// Returns the number of elements between the current position and the limit.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long Remaining(this Stream s)
        {
            return s.Length - s.Position;
        }

        /// <summary>
        /// Equivalent of Java's ByteBuffer.get(byte[], int, int).
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="dst"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public static void Get(this MemoryStream ms, byte[] dst, long offset, long length)
        {
            Array.Copy(ms.ToArray(), offset, dst, 0, length);
        }

        #endregion

        #region Java Netty

        /// <summary>
        /// Equivalent to Java Netty's ByteBuf.writeBytes(byte[] src).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="bytes"></param>
        public static void WriteBytes(this Stream s, sbyte[] bytes)
        {
            s.Write(bytes.ToByteArray(), 0, bytes.Length); // TODO test
        }

        /// <summary>
        /// Equivalent to Java Netty's ByteBuf.readableBytes().
        /// (writerPosition - readerPosition = Length - Position).
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long ReadableBytes(this Stream s)
        {
            // TODO correct when implementing a .NET ByteBuf
            return s.Remaining();
        }

        /// <summary>
        /// Equivalent to Java Netty's ByteBuf.duplicate().
        /// NOTE: Changes to the respective Stream instances are not mirrored as in Java Netty's ByteBuf.duplicate().
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static MemoryStream Duplicate(this Stream s)
        {
            var copy = new MemoryStream();
            s.CopyTo(copy); // TODO make async
            return copy;
        }

        /// <summary>
        /// Equivalent to Java Netty's ByteBuf.slice().
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static MemoryStream Slice(this MemoryStream ms)
        {
            byte[] data = ms.ToArray().Skip((int)ms.Position).ToArray(); // TODO test
            return new MemoryStream(data);
        }

        /// <summary>
        /// Equivalent to Java Netty's ByteBuf.readerIndex(int).
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="index"></param>
        public static void SetReaderIndex(this JavaBinaryReader reader, long index)
        {
            reader.BaseStream.Position = index;
        }

        public static long WriterIndex(this JavaBinaryWriter writer)
        {
            return writer.BaseStream.Position;
        }

        public static long ReaderIndex(this JavaBinaryReader reader)
        {
            return reader.BaseStream.Position;
        }

        public static void SkipBytes(this Stream s, int length)
        {
            for (int i = 0; i < length; i++)
            {
                s.ReadByte();
            }
        }

        public static long ReadableBytes(this JavaBinaryReader reader)
        {
            return reader.BaseStream.ReadableBytes();
        }

        /// <summary>
        /// Equivalent to Java Netty's ByteBuf.isReadable().
        /// Returns true if and only if (this.writerIndex - this.readerIndex) is greater than 0.
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static bool IsReadable(this JavaBinaryWriter writer)
        {
            // TODO implement
            return false;
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Converts a sbyte[] to byte[].
        /// </summary>
        /// <param name="signed">The sbyte[] to be converted.</param>
        /// <returns>The converted byte[].</returns>
        public static byte[] ToByteArray(this sbyte[] signed)
        {
            byte[] unsigned = new byte[signed.Length];
            Buffer.BlockCopy(signed, 0, unsigned, 0, signed.Length);

            return unsigned;
        }

        /// <summary>
        /// Converts a byte[] to sbyte[].
        /// </summary>
        /// <param name="unsigned">The byte[] to be converted.</param>
        /// <returns>The converted sbyte[].</returns>
        public static sbyte[] ToSByteArray(this byte[] unsigned)
        {
            // TODO test
            sbyte[] signed = new sbyte[unsigned.Length];
            Buffer.BlockCopy(unsigned, 0, signed, 0, unsigned.Length);

            return signed;
        }

        #endregion
    }
}