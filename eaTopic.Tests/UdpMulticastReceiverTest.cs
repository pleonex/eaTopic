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
		static readonly byte[] Expected = { 0xCA, 0xFE };

		DataFormatter formatter;
		UdpMulticastSender sender;
		UdpMulticastReceiver receiver;
		int calledTimes;

		[SetUp]
		public void Setup()
		{
			formatter = new DataFormatter(TopicDataType.FromGeneric<byte, byte>());
			sender    = new UdpMulticastSender(MulticastIp, Port);
			receiver  = new UdpMulticastReceiver(MulticastIp, Port);
			calledTimes = 0;
		}

		[Test]
		public void ReceiveData()
		{
			ReceiveXData(1);
		}

		[Test]
		public void ReceiveTwiceData()
		{
			ReceiveXData(2);
		}

		[Test]
		public void ReceiveZeroData()
		{
			ReceiveXData(0);
		}

		public void ReceiveXData(int times)
		{
			formatter[0] = Expected[0];
			formatter[1] = Expected[1];

			receiver.ReceivedData += HandleReceivedData;
			receiver.StartReceive(formatter);

			for (int i = 0; i < times; i++)
				sender.Write(formatter);

			System.Threading.Thread.Sleep(100);
			receiver.Close();

			Assert.AreEqual(times, calledTimes);
		}

		void HandleReceivedData (DataFormatter data)
		{
			calledTimes++;
			Assert.AreEqual(Expected[0], formatter[0]);
			Assert.AreEqual(Expected[1], formatter[1]);
		}
	}
}

