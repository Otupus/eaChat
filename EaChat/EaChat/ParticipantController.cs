//
//  ParticipantController.cs
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
using EaTopic.Participants;
using EaTopic.Participants.Builtin;
using EaTopic.Publishers;
using EaTopic.Subscribers;
using EaTopic.Topics;

namespace EaChat
{
	public class ParticipantController
	{
		Participant participant;
		Dictionary<string, TopicEntities> topics;

		public ParticipantController(string userName, MainWindow window)
		{
			participant = new Participant(0);
			participant.BuiltinTopic.PublisherDiscovered  += HandlePublisherDiscovered;
			participant.BuiltinTopic.SubscriberDiscovered += HandleSubscriberDiscovered;
			topics = new Dictionary<string, TopicEntities>();

			// Start temp code to ui testing
			var t = participant.CreateTopic<ChatMessage>("test");
			t.CreatePublisher();
			t.CreateSubscriber();
			// End temp code

			UserName   = userName;
			MainWindow = window;
		}

		public string UserName {
			get;
			set;
		}

		public MainWindow MainWindow {
			get;
			private set;
		}

		public void Send(string text, string topicName)
		{
			if (string.IsNullOrEmpty(topicName) || !topics.ContainsKey(topicName))
				return;

			var message = new ChatMessage(UserName, text, DateTime.Now);
			topics[topicName].Publisher.Write(message);
		}

		void HandlePublisherDiscovered(PublisherInfo pubInfo, BuiltinEventArgs e)
		{
			string topicName = pubInfo.TopicName;
			if (!topics.ContainsKey(topicName))
				topics.Add(topicName, new TopicEntities());

			if (e.Change == BuiltinEntityChange.Added)
				topics[topicName].NumPublishers++;
			else if (e.Change == BuiltinEntityChange.Removed)
				topics[topicName].NumPublishers--;

			UpdateWindowChatInfo(topicName);
		}

		void HandleSubscriberDiscovered (SubscriberInfo subInfo, BuiltinEventArgs e)
		{
			string topicName = subInfo.TopicName;
			if (!topics.ContainsKey(topicName))
				topics.Add(topicName, new TopicEntities());

			if (e.Change == BuiltinEntityChange.Added)
				topics[topicName].NumSubscribers++;
			else if (e.Change == BuiltinEntityChange.Removed)
				topics[topicName].NumSubscribers--;

			UpdateWindowChatInfo(topicName);
		}

		void UpdateWindowChatInfo(string topicName)
		{
			MainWindow.UpdateChatInfo(
				topicName,
				topics[topicName].NumPublishers,
				topics[topicName].NumSubscribers
			);
		}

		class TopicEntities
		{
			public Topic<ChatMessage> Topic { get; set; }
			public Publisher<ChatMessage> Publisher { get; set; }
			public Subscriber<ChatMessage> Subscriber { get; set; }
			
			public int NumPublishers { get; set; }
			public int NumSubscribers { get; set; }
		}
	}
}

