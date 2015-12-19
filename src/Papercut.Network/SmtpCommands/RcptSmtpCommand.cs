﻿// Papercut
// 
// Copyright © 2008 - 2012 Ken Robertson
// Copyright © 2013 - 2015 Jaben Cargman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License. 

namespace Papercut.Network.SmtpCommands
{
    using System.Collections.Generic;

    using Papercut.Core.Helper;
    using Papercut.Core.Network;

    public class RcptSmtpCommand : BaseSmtpCommand
    {
        protected override IEnumerable<string> GetMatchCommands()
        {
            return new[] { "RCPT" };
        }

        protected override void Run(string command, string[] args)
        {
            string line = string.Join(" ", args);

            // Check for the ":"
            if (!line.ToUpper().StartsWith("TO") || !line.Contains(":"))
            {
                Connection.SendLine("504 Command parameter not implemented");
                return;
            }

            // Check request order
            if (Session.Sender == null || Session.MailFrom == null)
            {
                Connection.SendLine("503 Bad sequence of commands");
                return;
            }

            string address =
                line.Substring(line.IndexOf(":") + 1)
                    .Replace("<", string.Empty)
                    .Replace(">", string.Empty)
                    .Trim();

            if (!Session.Recipients.Contains(address)) Session.Recipients.Add(address);

            Connection.SendLine("250 <{0}> OK", address);
        }
    }
}