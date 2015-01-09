//
//  UdpMulticastSenderTest.cs
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
using System.Net;
using System.Net.Sockets;
using EaTopic.Topics;
using EaTopic.Transports;
using Moq;
using NUnit.Framework;

namespace EaTopic.Tests
{
	[TestFixture]
	public class UdpMulticastSenderTest
	{
		const string MulticastIp = "239.0.0.222";
		const int Port = 2222;

		Mock<DataFormatter> formatter;
		UdpMulticastWriter<DataFormatter> sender;

		UdpClient client;
		IPEndPoint localEp;

		[SetUp]
		public void Setup()
		{
			formatter = new Mock<DataFormatter>();
			sender    = new UdpMulticastWriter<DataFormatter>(MulticastIp, Port);

			client = new UdpClient();

			client.ExclusiveAddressUse = false;
			localEp = new IPEndPoint(IPAddress.Any, Port);
			client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			client.ExclusiveAddressUse = false;

			client.Client.Bind(localEp);
			client.JoinMulticastGroup(IPAddress.Parse(MulticastIp));
		}

		[Test]
		public void WritesData()
		{
			byte[] expected = { 0xCA, 0xFE };

			formatter.Setup(f => f.SerializeData()).Returns(new byte[] { 0xCA, 0xFE});
			sender.Write(formatter.Object);

			byte[] actual = client.Receive(ref localEp);

			Assert.AreEqual(expected, actual);
		}
	}
}

