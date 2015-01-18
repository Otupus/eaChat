//
//  ChatBoxView.Designer.cs
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
	public partial class ChatBoxView : HBox
	{
		MarkdownView view;
		TextEntry textEntry;
		Button sendBtn;

		ListView userList;
		TextEntry filterText;
		ToggleButton filterBtn;
		RadioButton filterTextBtn;
		RadioButton filterUserBtn;

		void CreateComponents()
		{
			var chatBox = new VBox();
			PackStart(chatBox, true, true);

			view = new MarkdownView();
			view.Margin = 5;
			view.MarginBottom = 0;
			view.CanGetFocus = false;
			var scrolled = new ScrollView (view) { MinHeight = 200 };
			chatBox.PackStart(scrolled, true, true);

			var chatMsgBox = new HBox();
			chatMsgBox.Margin = 5;
			chatBox.PackStart(chatMsgBox);

			textEntry = new TextEntry();
			chatMsgBox.PackStart(textEntry, true);
		
			sendBtn = new Button("Send!");
			chatMsgBox.PackStart(sendBtn, vpos: WidgetPlacement.Center);

			var extraBox = new VBox();
			extraBox.WidthRequest = 150;
			PackStart(extraBox);

			userList = new ListView();
			userList.HeadersVisible = false;
			extraBox.PackStart(userList, true, true);
			extraBox.PackStart(new Label("Message filter"));

			filterTextBtn = new RadioButton("Text") { Active = true };
			filterUserBtn = new RadioButton("User");
			filterUserBtn.Group = filterTextBtn.Group;
			var filterTypeBox = new HBox();
			filterTypeBox.PackStart(filterTextBtn);
			filterTypeBox.PackStart(filterUserBtn);
			extraBox.PackStart(filterTypeBox);

			filterText = new TextEntry();
			extraBox.PackStart(filterText, hpos: WidgetPlacement.Fill);

			filterBtn = new ToggleButton("Set");
			extraBox.PackStart(filterBtn, hpos: WidgetPlacement.Fill);
		}
	}
}

