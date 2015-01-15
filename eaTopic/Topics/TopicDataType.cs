//
//  TopicType.cs
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

namespace EaTopic.Topics
{
	public struct TopicDataType
	{
		public TopicDataType(params Type[] types)
			: this()
		{
			Fields = types;
		}

		public Type[] Fields { get; private set; }

		public static bool operator ==(TopicDataType obj1, TopicDataType obj2)
		{
			return obj1.Equals(obj2);
		}

		public static bool operator !=(TopicDataType obj1, TopicDataType obj2)
		{
			return !obj1.Equals(obj2);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(TopicDataType))
				return false;
			TopicDataType other = (TopicDataType)obj;
			return Fields.SequenceEqual(other.Fields);
		}

		public override int GetHashCode()
		{
			unchecked {
				return (Fields != null ? Fields.GetHashCode() : 0);
			}
		}

		public static TopicDataType FromGeneric<T1>()
		{
			return new TopicDataType(typeof(T1));
		}

		public static TopicDataType FromGeneric<T1, T2>()
		{
			return new TopicDataType(typeof(T1), typeof(T2));
		}

		public static TopicDataType FromGeneric<T1, T2, T3>()
		{
			return new TopicDataType(typeof(T1), typeof(T2), typeof(T3));
		}

		public static TopicDataType FromGeneric<T1, T2, T3, T4>()
		{
			return new TopicDataType(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
		}

		public static TopicDataType FromGeneric<T1, T2, T3, T4, T5>()
		{
			return new TopicDataType(
				typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
		}

		public static TopicDataType FromGeneric<T1, T2, T3, T4, T5, T6>()
		{
			return new TopicDataType(
				typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
		}

		public static TopicDataType FromGeneric<T1, T2, T3, T4, T5, T6, T7>()
		{
			return new TopicDataType(
				typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
				typeof(T7));
		}

		public static TopicDataType FromGeneric<T1, T2, T3, T4, T5, T6, T7, T8>()
		{
			return new TopicDataType(
				typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
				typeof(T7), typeof(T8));
		}

		public static TopicDataType FromGeneric<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
		{
			return new TopicDataType(
				typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6),
				typeof(T7), typeof(T8), typeof(T9));
		}
	}
}

