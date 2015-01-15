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

		public void Close()
		{
			acceptState.Stop = true;
			receiver.Stop();

			foreach (var client in acceptState.ClientList)
				client.Close();

			receiver.Server.Close();
		}

		public event ReceivedDataEventHandler ReceivedData;

		public void StartReceive(DataFormatter formatter)
		{
			acceptState.Formatter = formatter;

			receiver.Start();
			receiver.BeginAcceptTcpClient(OnAcceptClient, acceptState);
		}

		void OnAcceptClient(IAsyncResult ar)
		{
			var acceptState = (AcceptState)ar.AsyncState;
			var receiver = acceptState.Receiver;

			var client = receiver.EndAcceptTcpClient(ar);
			acceptState.ClientList.Add(client);

			var readState = new ReadState(client.GetStream(), acceptState.Formatter);
			client.GetStream().BeginRead(
				readState.GetBuffer(), 0, readState.BufferSize, OnReceiveData, readState);

			if (!acceptState.Stop)
				receiver.BeginAcceptTcpClient(OnAcceptClient, acceptState);
		}

		void OnReceiveData(IAsyncResult ar)
		{
			var readState = (ReadState)ar.AsyncState;

			int readSize = readState.Stream.EndRead(ar);
			readState.Resize(readSize);

			if (readSize > 0) {
				readState.Formatter.Read(readState.GetBuffer());

				if (ReceivedData != null)
					ReceivedData(readState.Formatter);

				readState.Resize(readState.BufferSize);
				readState.Stream.BeginRead(
					readState.GetBuffer(), 0, readState.BufferSize, OnReceiveData, readState);
			}
		}

		class AcceptState
		{
			public AcceptState(TcpListener receiver)
			{
				Receiver = receiver;
				ClientList = new List<TcpClient>();
				Stop = false;
			}

			public TcpListener Receiver { get; private set; }
			public List<TcpClient> ClientList { get; private set; }

			public DataFormatter Formatter { get; set; }
			public bool Stop { get; set; }
		}

		class ReadState
		{
			byte[] buffer;

			public ReadState(NetworkStream stream, DataFormatter formatter)
			{
				Formatter = formatter;
				Stream = stream;
				buffer = new byte[BufferSize];
			}

			public DataFormatter Formatter { get; private set; }

			public int BufferSize { get { return 1024; } }

			public byte[] GetBuffer()
			{
				return buffer;
			}

			public NetworkStream Stream { get; private set; }

			public void Resize(int size)
			{
				Array.Resize(ref buffer, size);
			}
		}
	}
}

