//
//  TopicType.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
//  Copyright (c) 2015 Otupus
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
using System.Collections.Generic;
using System.IO;

namespace EaTopic.Topics
{
	/// <summary>
	/// Topic data formatter.
	/// We are not using built-in .NET serialization since we aim to make it compatible
	/// with other languages.
	/// </summary>
	public class DataFormatter
	{
		List<dynamic> entries;
		TopicDataType type;

		public DataFormatter(TopicDataType type)
		{
			this.type = type;
		}

		public byte[] Write()
		{
			var stream = new MemoryStream();
			Write(stream);

			byte[] data = stream.ToArray();
			stream.Dispose();
			return data;
		}

		public void Write(Stream stream)
		{
			throw new NotImplementedException();
		}

		public void Read(byte[] data)
		{
			MemoryStream stream = new MemoryStream(data);
			Read(stream);
			stream.Dispose();
		}

		public void Read(Stream stream)
		{
			throw new NotImplementedException();
		}
	}
}

