//
//  BinaryEncoder.cs
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
using System.IO;
using System.Text;

namespace EaTopic.Topics.Serialization
{
	internal class BinaryEncoder : DataEncoder
	{
		void WriteData(byte[] data, Stream stream)
		{
			stream.Write(data, 0, data.Length);
		}

		void WriteDataAndLength(byte[] data, Stream stream)
		{
			stream.WriteByte((byte)data.Length);
			WriteData(data, stream);
		}

		public override void Write(TypeId value, Stream stream)
		{
			Write((byte)value, stream);
		}

		public override void Write(byte value, Stream stream)
		{
			stream.WriteByte(value);
		}

		public override void Write(uint value, Stream stream)
		{
			WriteData(BitConverter.GetBytes(value), stream);
		}

		public override void Write(int value, Stream stream)
		{
			WriteData(BitConverter.GetBytes(value), stream);
		}

		public override void Write(string value, Stream stream)
		{
			WriteDataAndLength(Encoding.UTF8.GetBytes(value), stream);
		}

		public override void Write(Array value, Stream stream)
		{
			Write(GetTypeId(value.GetType().GetElementType()), stream);
			Write((byte)value.Length, stream);

			foreach (dynamic v in value)
				Write(v, stream);
		}

		public override void Write(DateTime value, Stream stream)
		{
			// The End of World will be in 2038 :D
			uint unixTime = (uint)(value.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			WriteData(BitConverter.GetBytes(unixTime), stream);
		}

		public override void Write(TopicDataType value, Stream stream)
		{
			Write((byte)value.Fields.Length, stream);
			foreach (var v in value.Fields)
				Write(GetTypeId(v), stream);
		}

		public override void Write(DataFormatter value, Stream stream)
		{
			Write(value.Type, stream);
			WriteDataAndLength(value.Write(), stream);
		}
	}
}

