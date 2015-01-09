//
//  UdpMulticastWriter.cs
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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace EaTopic.Transports
{
	/// <summary>
	/// UDP multicast writer.
	/// Example code from: http://www.jarloo.com/c-udp-multicasting-tutorial/
	/// </summary>
	public class UdpMulticastWriter<T> : TransportWrite<T>
		where T : TopicType
	{
		IPAddress address;
		IPEndPoint remotept;
		UdpClient client;

		BinaryFormatter binFormatter;
		MemoryStream buffer;

		public UdpMulticastWriter(string multicastIp, int port)
		{
			address = IPAddress.Parse(multicastIp);
			if (!address.IsIPv6Multicast)
				throw new ArgumentException("[eaTopic]: FATAL ERROR -> IP is not multicast");

			client = new UdpClient();
			client.JoinMulticastGroup(address);

			remotept = new IPEndPoint(address, port);
			buffer   = new MemoryStream();
			binFormatter = new BinaryFormatter();
		}

		public void Close()
		{
			client.Close();
		}

		public void Write(T data)
		{
			// Clean buffer and serialize class
			buffer.SetLength(0);
			binFormatter.Serialize(buffer, data);

			// Send it
			byte[] binData = buffer.ToArray();
			client.Send(binData, binData.Length, remotept);
		}
	}
}

