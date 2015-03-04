﻿using System.Net;
using System.Net.Sockets;

namespace TomP2P.Connection.Windows.Netty
{
    public delegate void ChannelEventHandler(IChannel channel);
    
    /// <summary>
    /// Interface to expose Java Netty's Channel API that is needed for this project.
    /// </summary>
    public interface IChannel
    {
        event ChannelEventHandler Closed;
        event ChannelEventHandler WriteCompleted;

        void Close();

        /// <summary>
        /// The underlying socket that is used.
        /// </summary>
        Socket Socket { get; }

        IPEndPoint LocalEndPoint { get; }

        IPEndPoint RemoteEndPoint { get; }

        Pipeline Pipeline { get; }

        bool IsUdp { get; }

        bool IsTcp { get; }

        /// <summary>
        /// Indicates whether this channel is open and may send or receive data.
        /// </summary>
        bool IsOpen { get; }
    }

    /// <summary>
    /// Marker interface for all TCP channels.
    /// </summary>
    public interface ITcpChannel : IChannel
    { }

    /// <summary>
    /// Marker interface for all UDP channels.
    /// </summary>
    public interface IUdpChannel : IChannel
    { }
}
