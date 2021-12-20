// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Suity.Synchonizing.Core
{
    public class SyncPathReportItem
    {
        public readonly object Owner;

        public readonly SyncPath Path;

        public readonly object Info;

        public readonly string Message;


        public SyncPathReportItem(object owner, SyncPath path)
        {
            Owner = owner;
            Path = path ?? SyncPath.Empty;
        }
        public SyncPathReportItem(object owner, SyncPath path, object info, string message)
        {
            Owner = owner;
            Path = path ?? SyncPath.Empty;
            Info = info;
            Message = message;
        }

        public override string ToString()
        {
            if (Path != null)
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    //return Message;
                    return $"{Message}\r\n{Owner}|{Path}";
                }
                else
                {
                    return $"{Owner}|{Path}";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    //return Message;
                    return $"{Message}\r\n{Owner}";
                }
                else
                {
                    return $"{Owner}";
                }
            }
        }
    }
}
