//
//  FilterCondition.cs
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

namespace EaTopic.Subscribers
{
	public enum FilterCondition : byte {
		Equals,
		Bigger,
		BiggerOrEquals,
		Less,
		LessOrEquals,
		Contains,
	}

	public static class FilterConditions
	{
		public static bool Apply(FilterCondition cond, dynamic op1, dynamic op2)
		{
			switch (cond) {
			case FilterCondition.Equals:
				return IsEquals(op1, op2);

			case FilterCondition.Bigger:
				return IsBigger(op1, op2);

			case FilterCondition.BiggerOrEquals:
				return IsBiggerOrEquals(op1, op2);

			case FilterCondition.Less:
				return IsLess(op1, op2);

			case FilterCondition.LessOrEquals:
				return IsLessOrEquals(op1, op2);

			case FilterCondition.Contains:
				return Contains(op1, op2);

			default:
				return false;
			}
		}

		private static bool IsEquals(dynamic op1, dynamic op2)
		{
			return op1.Equals(op2);
		}

		private static bool IsBigger(dynamic op1, dynamic op2)
		{
			return op1 > op2;
		}

		private static bool IsBiggerOrEquals(dynamic op1, dynamic op2)
		{
			return op1 >= op2;
		}

		private static bool IsLess(dynamic op1, dynamic op2)
		{
			return op1 < op2;
		}

		private static bool IsLessOrEquals(dynamic op1, dynamic op2)
		{
			return op1 <= op2;
		}

		private static bool Contains(dynamic op1, dynamic op2)
		{
			return op1.Contains(op2);
		}
	}
}

