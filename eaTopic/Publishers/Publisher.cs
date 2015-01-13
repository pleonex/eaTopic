//
//  Publisher.cs
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
using EaTopic.Transports;
using EaTopic.Topics;

namespace EaTopic.Publishers
{
	public class Publisher<T>
		where T: DataFormatter
	{
		readonly TransportSender<T> sender;

		internal Publisher(TransportSender<T> sender)
		{
			this.sender = sender;
		}

		/// <summary>
		/// Gets or sets the metadata associated with this publisher
		/// </summary>
		/// <value>Metadata value.</value>
		public string Metadata {
			get;
			set;
		}

		/// <summary>
		/// Write / Publish an instance of data.
		/// </summary>
		/// <param name="instance">Instance of data to write.</param>
		public void Write(T instance)
		{
			sender.Write(instance);
		}
	}
}

