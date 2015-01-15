//
//  BinSerializerTests.cs
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

namespace EaTopic.Tests
{
	[TestFixture]
	public class BinSerializerTests
	{
		DataFormatter formatter;

		public void TestType<T>(byte[] expected, T value)
		{
			formatter = new DataFormatter(TopicDataType.FromGeneric<T>());
			formatter[0] = value;

			byte[] actual = formatter.Write();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void SerializeByte()
		{
			byte[] expected = { 0x01, 0xBE };
			TestType(expected, expected[1]);
		}

		[Test]
		public void SerializeUInt()
		{
			byte[] expected = { 0x02, 0xE0, 0xDD, 0xFF, 0xFF };
			TestType(expected, (uint)4294958560);
		}

		[Test]
		public void SerializeInt()
		{
			byte[] expected = { 0x03, 0xE0, 0xDD, 0xFF, 0xFF };
			TestType(expected, -8736);
		}

		[Test]
		public void SerializeString()
		{
			Encoding enc = Encoding.UTF8;
			String value = "Hëço Gûeños";

			byte[] expected = new byte[enc.GetByteCount(value) + 2];
			expected[0] = 0x4;
			expected[1] = (byte)(expected.Length - 2);
			enc.GetBytes(value, 0, value.Length, expected, 2);

			TestType(expected, value);
		}

		[Test]
		public void SerializeByteArray()
		{
			byte[] expected = { 0x80, 0x1, 0x4, 0xCA, 0xFE, 0xBE, 0xBE };
			TestType(expected, new byte[4] { 0xCA, 0xFE, 0xBE, 0xBE});
		}

		[Test]
		public void SerializeJaggedByteArray()
		{
			byte[] expected = { 0x80, 0x80, 0x02, 0x1, 0x2, 0xCA, 0xFE, 0x1, 0x2, 0xBE, 0xBE };
			byte[][] value = new byte[2][] { new byte[]{ 0xCA, 0xFE }, new byte[]{ 0xBE, 0xBE } };
			TestType(expected, value);
		}

		[Test]
		public void SerializeDateTime()
		{
			byte[] expected = { 0x20, 0xF7, 0xD1, 0xB7, 0x54 };
			var time = new DateTime(2015, 1, 15, 14, 43, 3);
			TestType(expected, time);
		}

		[Test]
		public void SerializeTopicDataType()
		{
			byte[] expected = { 0x40, 0x2, 0x1, 0x4 };
			TestType(expected, TopicDataType.FromGeneric<byte, string>());
		}

		[Test]
		public void SerializeDataFormatter()
		{
			var formatter = new DataFormatter(TopicDataType.FromGeneric<byte, string>());
			formatter[0] = (byte)0x32;
			formatter[1] = "a02";
			byte[] expected = { 0x41, 0x02, 0x01, 0x04, 0x07, 0x1, 0x32, 0x4, 0x3, 0x61, 0x30, 0x32 };
			TestType(expected, formatter);
		}
	}
}

