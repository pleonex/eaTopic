//
//  UdpMulticastReceiver.cs
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

namespace EaTopic.Transports
{
	public class UdpMulticastReceiver : TransportReceiver
	{
		IPAddress address;
		UdpClient client;
		IPEndPoint localEp;
		UdpState state;

		public UdpMulticastReceiver(string multicastIp, int port)
		{
			address = IPAddress.Parse(multicastIp);
			if (address.GetAddressBytes()[0] < 224 || address.GetAddressBytes()[1] > 239)
				throw new ArgumentException("[eaTopic]: FATAL ERROR -> IP is not multicast");

			client = new UdpClient();
			ConfigureSharePort(port);

			client.Client.Bind(localEp);
			client.JoinMulticastGroup(address);
		}

		void ConfigureSharePort(int port)
		{
			client.ExclusiveAddressUse = false;
			localEp = new IPEndPoint(IPAddress.Any, port);
			client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			client.ExclusiveAddressUse = false;
		}

		public void Close()
		{
			client.Close();
		}

		public event ReceivedDataEventHandler ReceivedData;

		public void OnReceived(IAsyncResult ar)
		{
			var state = (UdpState)ar.AsyncState;
			IPEndPoint endPoint = state.EndPoint;
			byte[] binData;

			try {
				binData = state.Client.EndReceive(ar, ref endPoint);
			} catch (ObjectDisposedException) {
				return;
			}

			if (binData.Length > 0) {
				state.Formatter.Read(binData);

				if (ReceivedData != null)
					ReceivedData(state.Formatter);
			}

			if (!state.Stop)
				state.LastAsyncResult = state.Client.BeginReceive(OnReceived, state);
		}

		public void StartReceive(DataFormatter formatter)
		{
			state = new UdpState(localEp, client, formatter);
			state.LastAsyncResult = client.BeginReceive(OnReceived, state);
		}

		class UdpState
		{
			public UdpState(IPEndPoint ep, UdpClient client, DataFormatter formatter)
			{
				EndPoint  = ep;
				Client    = client;
				Formatter = formatter;
				Stop = false;
			}

			public IPEndPoint    EndPoint  { get; private set; }
			public UdpClient     Client    { get; private set; }
			public DataFormatter Formatter { get; private set; }

			public bool Stop { get; set; }
			public IAsyncResult LastAsyncResult { get; set; }
		}
	}
}

