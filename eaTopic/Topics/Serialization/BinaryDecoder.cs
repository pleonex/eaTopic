//
//  BinaryDecoder.cs
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
	internal class BinaryDecoder : DataDecoder
	{
		public override TypeId ReadTypeId(Stream stream)
		{
			return (TypeId)ReadByte(stream);
		}

		public override byte ReadByte(Stream stream)
		{
			return (byte)stream.ReadByte();
		}

		byte[] ReadData(Stream stream, int size)
		{
			byte[] data = new byte[size];
			stream.Read(data, 0, size);
			return data;
		}

		byte[] ReadDataAndLength(Stream stream)
		{
			byte size = ReadByte(stream);
			return ReadData(stream, size);
		}

		public override uint ReadUInt32(Stream stream)
		{
			return BitConverter.ToUInt32(ReadData(stream, 4), 0);
		}

		public override int ReadInt32(Stream stream)
		{
			return BitConverter.ToInt32(ReadData(stream, 4), 0);
		}

		public override string ReadString(Stream stream)
		{
			byte[] data = ReadDataAndLength(stream);
			return Encoding.UTF8.GetString(data);
		}

		public override dynamic ReadArray(Stream stream)
		{
			TypeId id = ReadTypeId(stream);
			int size = ReadByte(stream);

			Type type = TypeId2Type(id);
			Array array = Array.CreateInstance(type, size);
			for (int i = 0; i < size; i++)
				array.SetValue(ReadByTypeId(id, stream) , i);

			return array;
		}

		public override DateTime ReadDateTime(Stream stream)
		{
			uint data = ReadUInt32(stream);
			return new DateTime(1970, 1, 1).AddSeconds(data);
		}

		public override TopicDataType ReadTopicDataType(Stream stream)
		{
			var types = new Type[ReadByte(stream)];
			for (int i = 0; i < types.Length; i++)
				types[i] = TypeId2Type(ReadTypeId(stream));

			return new TopicDataType(types);
		}

		public override DataFormatter ReadDataFormatter(Stream stream)
		{
			TopicDataType type = ReadTopicDataType(stream);

			var formatter = new DataFormatter(type);
			formatter.Read(ReadDataAndLength(stream));

			return formatter;
		}
	}
}

