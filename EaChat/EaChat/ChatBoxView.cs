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
using System.Linq;
using EaTopic.Participants.Builtin;
using EaTopic.Subscribers;
using Xwt;
using Xwt.Drawing;

namespace EaChat
{
	public partial class ChatBoxView
	{
		ParticipantController controller;
		ListStore userStore;
		DataField<Image> userImgCol;
		DataField<string> usernameCol;
		DataField<byte[]> userUuidCol;

		public ChatBoxView(ParticipantController controller, string chatName)
		{
			CreateComponents();
			ChatName = chatName;

			usernameCol = new DataField<string>();
			userUuidCol = new DataField<byte[]>();
			userImgCol  = new DataField<Image>();
			userStore = new ListStore(usernameCol, userImgCol, userUuidCol);
			userList.DataSource = userStore;
			userList.Columns.Add("Users", userImgCol, usernameCol);

			this.controller = controller;
			this.controller.CreateTopic(chatName, ShowMessage);

			controller.SubscriberDiscovered += UpdateUserList;
			foreach (var subInfo in controller.GetSubscribers(chatName))
				UpdateUserList(subInfo, new BuiltinEventArgs(null, BuiltinEntityChange.Added));

			textEntry.KeyPressed += HandleKeySendPressed;
			sendBtn.Clicked += HandleSend;
		}

		public string ChatName {
			get;
			private set;
		}

		public void ShowMessage(ChatMessage instance)
		{
			string chatText = string.Format("[*{0}*] __{1}__: {2}  \n", 
				instance.Date.ToShortTimeString(), instance.UserName, instance.Message);
			view.Markdown += chatText;
		}
			
		void UpdateUserList(SubscriberInfo subInfo, BuiltinEventArgs e)
		{
			if (subInfo.TopicName != ChatName)
				return;

			if (e.Change == BuiltinEntityChange.Added)
				AddUserToList(subInfo);
			else if (e.Change == BuiltinEntityChange.Removed)
				RemoveUserFromList(subInfo);
		}

		void AddUserToList(SubscriberInfo info)
		{
			userStore.SetValues(
				userStore.AddRow(),
				usernameCol,
				info.Metadadata,
				userImgCol,
				GetUserImage(info),
				userUuidCol,
				info.Uuid
			);
		}

		Image GetUserImage(SubscriberInfo info)
		{
			if (info.Metadadata == controller.UserName)
				return Image.FromResource("EaChat.res.user_go.png");
			else if (IsUserPublisher(info.TopicName, info.Metadadata))
				return Image.FromResource("EaChat.res.user.png");
			else
				return Image.FromResource("EaChat.res_gray.png");
		}

		bool IsUserPublisher(string topicName, string username)
		{
			return controller.GetPublishers(topicName).Any(p => p.Metadata == username);
		}

		void RemoveUserFromList(SubscriberInfo info)
		{
			int row = FindUser(info.Uuid);
			if (row != -1)
				userStore.RemoveRow(row);
		}

		int FindUser(byte[] uuid)
		{
			for (int i = 0; i < userStore.RowCount; i++)
				if (userStore.GetValue(i, userUuidCol).SequenceEqual(uuid))
					return i;

			return -1;
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

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			controller.CloseTopic(ChatName);
		}
	}
}

