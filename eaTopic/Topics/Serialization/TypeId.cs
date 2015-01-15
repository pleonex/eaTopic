//
//  TypeId.cs
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

namespace EaTopic.Topics.Serialization
{
	internal enum TypeId : byte {
		Default = 0xFF,
		Byte   = 0x01,
		UInt32 = 0x02,
		Int32  = 0x03,
		String = 0x04,

		DateTime = 0x20,

		TopicDataType = 0x40,
		DataFormatter = 0x41,

		Array  = 0x80,
	}
}

