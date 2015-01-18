//
//  Filter.cs
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
using EaTopic.Topics;
using System.Collections.Generic;

namespace EaTopic.Subscribers
{
	public class Filter
	{
		List<FilterExpression> conditions;

		public Filter(TopicDataType dataType)
		{
			DataType = dataType;
			conditions = new List<FilterExpression>();
		}

		public TopicDataType DataType {
			get;
			private set;
		}

		public void AddCondition(int entry, FilterCondition cond, dynamic op2)
		{
			conditions.Add(new FilterExpression(entry, cond, op2));
		}

		public bool IsValid(TopicData instance) {
			var formatter = instance.CreateFormatter();
			instance.SerializeData(formatter);

			foreach (var expr in conditions) {
				dynamic op1 = formatter.Get(expr.EntryIndex);
				bool result = FilterConditions.Apply(expr.Condition, op1, expr.Operand2);
				if (!result)
					return false;
			}

			return true;
		}

		struct FilterExpression
		{
			public FilterExpression(int entryIndex, FilterCondition condition, dynamic op2)
				: this()
			{
				this.EntryIndex = entryIndex;
				this.Condition = condition;
				this.Operand2 = op2;
			}

			public int EntryIndex { get; private set; }
			public FilterCondition Condition { get; private set; }
			public dynamic Operand2 { get; private set; }
		}
	}
}

