//
//  Topic.cs
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
using System.Collections.Generic;
using System.Reflection;
using EaTopic.Publishers;
using EaTopic.Transports;

namespace EaTopic.Topics
{
	public class Topic<T> : Entity
		where T : DataFormatter
	{
		readonly List<Publisher<T>> publishers;

		internal Topic(string name, bool isBuiltin)
		{
			Name = name;
			IsBuiltin  = isBuiltin;
			publishers = new List<Publisher<T>>();
		}

		public string Name {
			get;
			private set;
		}

		internal bool IsBuiltin {
			get;
			private set;
		}

		public void Dispose()
		{
			foreach (var pub in publishers)
				pub.Dispose();

			publishers.Clear();
		}

		public Publisher<T> CreatePublisher()
		{
			var pub = new Publisher<T>(this);
			publishers.Add(pub);
			return pub;
		}
	}
}

