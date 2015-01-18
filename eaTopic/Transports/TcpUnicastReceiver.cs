//
//  TcpUnicastReceiver.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
//  Copyright (c) 2015 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using EaTopic.Topics;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace EaTopic.Transports
{
	public class TcpUnicastReceiver : TransportReceiver
	{
		TcpListener receiver;
		AcceptState acceptState;

		public TcpUnicastReceiver(string ip)
		{
			receiver = new TcpListener(IPAddress.Parse(ip), 0);
			ConfigureSharePort();

			acceptState = new AcceptState(receiver);
		}

		void ConfigureSharePort()
		{
			receiver.ExclusiveAddressUse = false;
			receiver.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			receiver.ExclusiveAddressUse = false;
		}

		public int Port {
			get { return ((IPEndPoint)receiver.LocalEndpoint).Port; }
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Close()
		{
			receiver.Stop();
			foreach (var client in acceptState.ClientList) {
				if (client.Key.Connected) {
					client.Key.GetStream().Close();
					client.Key.Close();
				}
			}

			acceptState.ClientList.Clear();
		}

		public event ReceivedDataEventHandler ReceivedData;

		public void StartReceive(DataFormatter formatter)
		{
			acceptState.Formatter = formatter;

			receiver.Start();
			receiver.BeginAcceptTcpClient(OnAcceptClient, acceptState);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		void OnAcceptClient(IAsyncResult ar)
		{
			var acceptState = (AcceptState)ar.AsyncState;

			TcpListener receiver = acceptState.Receiver;
			TcpClient client;
			try {
				client = receiver.EndAcceptTcpClient(ar);
				if (client == null)
					return;

				acceptState.ClientList.TryAdd(client, client);
				receiver.BeginAcceptTcpClient(OnAcceptClient, acceptState);
			} catch (ObjectDisposedException) {
				return;
			}

			var readState = new ReadState(acceptState.ClientList, client, acceptState.Formatter);
			client.GetStream().BeginRead(
				readState.GetBuffer(), 0, readState.BufferSize, OnReceiveData, readState);

		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		void OnReceiveData(IAsyncResult ar)
		{
			var readState = (ReadState)ar.AsyncState;
			if (!readState.Client.Connected)
				return;

			int readSize = readState.Client.GetStream().EndRead(ar);
			if (readSize == 0) {
				TcpClient dummy;
				readState.ClientList.TryRemove(readState.Client, out dummy);
				return;
			}
				
			readState.Resize(readSize);
			readState.Formatter.Read(readState.GetBuffer());

			readState.Resize(readState.BufferSize);
			readState.Client.GetStream().BeginRead(
				readState.GetBuffer(), 0, readState.BufferSize, OnReceiveData, readState);

			if (ReceivedData != null)
				ReceivedData(readState.Formatter);
		}

		class AcceptState
		{
			public AcceptState(TcpListener receiver)
			{
				Receiver = receiver;
				ClientList = new ConcurrentDictionary<TcpClient, TcpClient>();
			}

			public TcpListener Receiver { get; private set; }
			public ConcurrentDictionary<TcpClient, TcpClient> ClientList { get; private set; }

			public DataFormatter Formatter { get; set; }
		}

		class ReadState
		{
			byte[] buffer;

			public ReadState(ConcurrentDictionary<TcpClient, TcpClient> clientList, 
				TcpClient client, DataFormatter formatter)
			{
				ClientList = clientList;
				Client = client;
				Formatter = formatter;
				buffer = new byte[BufferSize];
			}

			public ConcurrentDictionary<TcpClient, TcpClient> ClientList { get; private set; }

			public TcpClient Client { get; private set; }

			public DataFormatter Formatter { get; private set; }

			public int BufferSize { get { return 1024; } }

			public byte[] GetBuffer()
			{
				return buffer;
			}

			public void Resize(int size)
			{
				Array.Resize(ref buffer, size);
			}
		}
	}
}

