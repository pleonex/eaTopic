//
//  DataDecoder.cs
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

namespace EaTopic.Topics.Serialization
{
	internal abstract class DataDecoder
	{
		public dynamic Decode(Stream stream)
		{
			TypeId type = ReadTypeId(stream);
			return ReadByTypeId(type, stream);
		}

		protected Type TypeId2Type(TypeId id)
		{
			switch (id) {
			case TypeId.Byte:   return typeof(Byte);
			case TypeId.UInt32: return typeof(UInt32);
			case TypeId.Int32:  return typeof(Int32);
			case TypeId.String: return typeof(String);
			case TypeId.Array:  return typeof(Array);
			case TypeId.DateTime: return typeof(DateTime);
			case TypeId.TopicDataType: return typeof(TopicDataType);
			case TypeId.DataFormatter: return typeof(DataFormatter);
			}

			return null;
		}

		protected dynamic ReadByTypeId(TypeId type, Stream stream)
		{
			switch (type) {
			case TypeId.Byte:
				return ReadByte(stream);

			case TypeId.UInt32:
				return ReadUInt32(stream);

			case TypeId.Int32:
				return ReadInt32(stream);

			case TypeId.String:
				return ReadString(stream);

			case TypeId.Array:
				return ReadArray(stream);

			case TypeId.DateTime:
				return ReadDateTime(stream);

			case TypeId.TopicDataType:
				return ReadTopicDataType(stream);

			case TypeId.DataFormatter:
				return ReadDataFormatter(stream);
			}

			return null;
		}

		public abstract TypeId ReadTypeId(Stream stream);

		public abstract byte ReadByte(Stream stream);

		public abstract uint ReadUInt32(Stream stream);

		public abstract int ReadInt32(Stream stream);

		public abstract string ReadString(Stream stream);

		public abstract dynamic ReadArray(Stream stream);

		public abstract DateTime ReadDateTime(Stream stream);

		public abstract TopicDataType ReadTopicDataType(Stream stream);

		public abstract DataFormatter ReadDataFormatter(Stream stream);
	}
}

