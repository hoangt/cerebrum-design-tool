/******************************************************************** 
 * Cerebrum Embedded System Design Automation Framework
 * Copyright (C) 2010  The Pennsylvania State University
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ********************************************************************/
/******************************************************************** 
 * MessageEventController.cs
 * Name: Matthew Cotter
 * Date: 24 Sep 2010 
 * Description: Class used a single point-of-interface for multiple objects to send message events.
 * History: 
 * >> ( 1 Oct 2010) Matthew Cotter: Added Status message type to Message Types and events.
 * >> (24 Sep 2010) Matthew Cotter: Added basic method invocations and events to facilitate propagation of messaging events.
 * >> (24 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CerebrumSharedClasses
{
    /// <summary>
    /// Enumerated type indicating the different types of messaging events.
    /// </summary>
    public enum MessageEventType
    {
        /// <summary>
        /// All previous messages of all types should be cleared
        /// </summary>
        Clear,
        /// <summary>
        /// A new 'information' message is generated
        /// </summary>
        Info,
        /// <summary>
        /// A new 'warning' message is generated
        /// </summary>
        Warning,
        /// <summary>
        /// A new 'error' message is generated
        /// </summary>
        Error,
        /// <summary>
        /// A new 'console' message is generated
        /// </summary>
        Console,
        /// <summary>
        /// A new 'status' message is generated
        /// </summary>
        Status
    }
    /// <summary>
    /// Class to provide a single point-of-interface to pass and propagate messages to a single display-output
    /// </summary>
    public class MessageEventController
    {
        /// <summary>
        /// Delegate to handle generated message events
        /// </summary>
        /// <param name="MsgID">A identifier indicating the message generated</param>
        /// <param name="Message">The message generated.</param>
        /// <param name="MsgSource">A string indicating the source of the message</param>
        public delegate void MessageHandler(string MsgID, string Message, string MsgSource);
        /// <summary>
        /// Event fired when messages are to be cleared
        /// </summary>
        public event MessageHandler OnClearMessages;
        /// <summary>
        /// Event fired when an 'information' message is generated
        /// </summary>
        public event MessageHandler OnInfoMessage;
        /// <summary>
        /// Event fired when a 'warning' message is generated
        /// </summary>
        public event MessageHandler OnWarningMessage;
        /// <summary>
        /// Event fired when a 'error' message is generated
        /// </summary>
        public event MessageHandler OnErrorMessage;
        /// <summary>
        /// Event fired when a 'console' message is generated
        /// </summary>
        public event MessageHandler OnConsoleMessage;
        /// <summary>
        /// Event fired when a 'status' message is generated
        /// </summary>
        public event MessageHandler OnStatusMessage;

        /// <summary>
        /// Common method used to invoke events of the desired type based on the type parameter.
        /// </summary>
        /// <param name="MsgType">The type of the message event to be fired</param>
        /// <param name="MsgID">A identifier indicating the message generated</param>
        /// <param name="Message">The message generated.</param>
        /// <param name="MsgSource">A string indicating the source of the message</param>
        public void RaiseMessageEvent(MessageEventType MsgType, string MsgID, string Message, string MsgSource)
        {
            MessageHandler eh = null;
            switch (MsgType)
            {
                case MessageEventType.Clear:
                    eh = OnClearMessages;
                    break;
                case MessageEventType.Info:
                    eh = OnInfoMessage;
                    break;
                case MessageEventType.Warning:
                    eh = OnWarningMessage;
                    break;
                case MessageEventType.Error:
                    eh = OnErrorMessage;
                    break;
                case MessageEventType.Console:
                    eh = OnConsoleMessage;
                    break;
                case MessageEventType.Status:
                    eh = OnStatusMessage;
                    break;
            }
            if (eh != null)
                eh(MsgID, Message, MsgSource);
        }
    }
}
