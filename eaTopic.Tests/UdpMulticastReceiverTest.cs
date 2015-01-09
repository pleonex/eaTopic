//
//  UdpMulticastReceiverTest.cs
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
using System.Linq;
using NUnit.Framework;
using EaTopic.Transports;
using EaTopic.Topics;
using Moq;

namespace EaTopic.Tests
{
	[TestFixture]
	public class UdpMulticastReceiverTest
	{
		const string MulticastIp = "239.0.0.222";
		const int Port = 2222;

		Mock<DataFormatter> formatter;
		UdpMulticastSender<DataFormatter> sender;
		UdpMulticastReceiver<DataFormatter> receiver;

		[SetUp]
		public void Setup()
		{
			formatter = new Mock<DataFormatter>();
			sender = new UdpMulticastSender<DataFormatter>(MulticastIp, Port);
			receiver = new UdpMulticastReceiver<DataFormatter>(MulticastIp, Port);
		}

		[Test]
		public void ReceiveData()
		{
			byte[] expected = { 0xCA, 0xFE };
			formatter.Setup(f => f.SerializeData()).Returns(expected);
			formatter.Setup(f => f.DeserializeData(
				It.Is<byte[]>(d => d.SequenceEqual(expected))
			));

			sender.Write(formatter.Object);
			receiver.Read(formatter.Object);

			Assert.DoesNotThrow(() => formatter.VerifyAll());
		}
	}
}

