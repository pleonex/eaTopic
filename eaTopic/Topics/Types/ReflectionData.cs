//
//  ReflectionData.cs
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
using System.Reflection;

namespace EaTopic.Topics.Types
{
	public abstract class ReflectionData : TopicData
	{
		PropertyInfo[] fields;

		protected ReflectionData()
		{
			fields = this.GetType().GetProperties().Where(p => p.CanWrite && p.CanRead).ToArray();
		}

		public override TopicDataType Type {
			get {
				return new TopicDataType(fields.Select(p => p.PropertyType).ToArray());
			}
		}

		public override void SerializeData(DataFormatter formatter)
		{
			for (int i = 0; i < fields.Length; i++)
				formatter[i] = fields[i].GetValue(this);
		}

		public override void DeserializeData(DataFormatter formatter)
		{
			for (int i = 0; i < fields.Length; i++)
				fields[i].SetValue(this, formatter[i]);
		}
	}
}

