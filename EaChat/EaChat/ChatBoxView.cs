//
//  ChatBoxView.cs
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
using Xwt;

namespace EaChat
{
	public partial class ChatBoxView
	{
		ParticipantController controller;

		public ChatBoxView(ParticipantController controller, string chatName)
		{
			CreateComponents();

			ChatName = chatName;
			this.controller = controller;
			this.controller.CreateTopic(chatName, ShowMessage);

			textEntry.KeyPressed += HandleKeySendPressed;
			sendBtn.Clicked += HandleSend;
		}

		public string ChatName {
			get;
			private set;
		}

		public void ShowMessage(ChatMessage instance)
		{
			string chatText = string.Format("[{0}] {1}: {2}\n", 
				instance.UserName, instance.Date, instance.Message);
			view.Markdown += chatText;
		}

		void HandleKeySendPressed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
				Send();
		}

		void HandleSend(object sender, EventArgs e)
		{
			Send();
		}

		void Send()
		{
			controller.Send(textEntry.Text, ChatName);
			textEntry.Text = string.Empty;
		}
	}
}

