//
//  BinDeserializerTests.cs
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
using NUnit.Framework;
using EaTopic.Topics;
using System.Text;
using System.Net;

namespace eaTopic.Tests
{
	[TestFixture]
	public class BinDeserializerTests
	{
		T TestType<T>(byte[] data)
		{
			DataFormatter formatter = new DataFormatter(TopicDataType.FromGeneric<T>());
			formatter.Read(data);

			return formatter[0];
		}

		[Test]
		public void DeserializeByte()
		{
			byte expected = 0xBE;
			byte[] data = { 0x01, expected };

			byte actual = TestType<byte>(data);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DeserializeUInt32()
		{
			byte[] data = { 0x02, 0xE0, 0xDD, 0xFF, 0xFF };
			uint expected = 4294958560;
			uint actual = TestType<uint>(data);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DeserializeInt32()
		{
			byte[] data = { 0x03, 0xE0, 0xDD, 0xFF, 0xFF };
			int expected = -8736;
			int actual = TestType<int>(data);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DeserializeString()
		{
			Encoding enc = Encoding.UTF8;
			String expected = "Hëço Gûeños";

			byte[] data = new byte[enc.GetByteCount(expected) + 2];
			data[0] = 0x4;
			data[1] = (byte)(data.Length - 2);
			enc.GetBytes(expected, 0, expected.Length, data, 2);

			String actual = TestType<string>(data);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DeserializeArray()
		{
			byte[] data = { 0x80, 0x1, 0x4, 0xCA, 0xFE, 0xBE, 0xBE };
			byte[] expected = { 0xCA, 0xFE, 0xBE, 0xBE };

			byte[] actual = TestType<byte[]>(data);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void NOTDeserializeJaggedByteArray()
		{
			byte[] data = { 0x80, 0x80, 0x02, 0x1, 0x2, 0xCA, 0xFE, 0x1, 0x2, 0xBE, 0xBE };
			//var expected = new byte[2][] { new byte[]{ 0xCA, 0xFE }, new byte[]{ 0xBE, 0xBE } };

			Assert.Throws<ProtocolViolationException>(() => TestType<byte[][]>(data));
			//Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DeserializeDateTime()
		{
			byte[] data = { 0x20, 0xF7, 0xD1, 0xB7, 0x54 };
			var expected = new DateTime(2015, 1, 15, 14, 43, 3);
			var actual = TestType<DateTime>(data);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DeserializeTopicDataTime()
		{
			byte[] data = { 0x40, 0x2, 0x1, 0x4 };
			var expected = TopicDataType.FromGeneric<byte, string>();
			var actual = TestType<TopicDataType>(data);
			Assert.AreEqual(expected.Fields, actual.Fields);
		}

		[Test]
		public void DeserializeDataFormatter()
		{
			var expected = new DataFormatter(TopicDataType.FromGeneric<byte, string>());
			expected[0] = (byte)0x32;
			expected[1] = "a02";

			byte[] data = { 0x41, 0x02, 0x01, 0x04, 0x07, 0x1, 0x32, 0x4, 0x3, 0x61, 0x30, 0x32 };
			var actual = TestType<DataFormatter>(data);

			Assert.AreEqual(expected.Type.Fields, actual.Type.Fields);
			Assert.AreEqual(expected[0], actual[0]);
			Assert.AreEqual(expected[1], actual[1]);
		}
	}
}

