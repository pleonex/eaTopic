//
//  DataEncoder.cs
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
	internal abstract class DataEncoder
	{
		public void Encode(dynamic value, Stream stream)
		{
			TypeId type = GetTypeId(value.GetType());
			Write(type, stream);
			Write(value, stream);
		}

		protected TypeId GetTypeId(Type type)
		{
			TypeId id;

			if (type.IsArray)
				id = TypeId.Array;
			else
				id = (TypeId)Enum.Parse(typeof(TypeId), type.Name);

			return id;
		}

		public abstract void Write(TypeId value, Stream stream);

		public abstract void Write(byte value, Stream stream);

		public abstract void Write(uint value, Stream stream);

		public abstract void Write(int value, Stream stream);

		public abstract void Write(string value, Stream stream);

		public abstract void Write(Array value, Stream stream);

		public abstract void Write(DateTime value, Stream stream);

		public abstract void Write(TopicDataType value, Stream stream);

		public abstract void Write(DataFormatter value, Stream stream);
	}
}

