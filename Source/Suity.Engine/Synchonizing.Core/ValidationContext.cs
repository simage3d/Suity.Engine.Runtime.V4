// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Suity.Synchonizing.Core
{
    public class ValidationContext
    {
        public class ReportItem
        {
            public string Message { get; set; }
            public TextStatus Status { get; set; }
            public object Information { get; set; }

            public ReportItem()
            {
            }
            public ReportItem(string message, object information)
            {
                Message = message;
                Information = information;
            }
        }

        readonly public List<ReportItem> Reports = new List<ReportItem>();

        public void Report(string message)
        {
            Reports.Add(new ReportItem { Message = message });
        }
        public void Report(string message, TextStatus status)
        {
            Reports.Add(new ReportItem { Message = message, Status = status });
        }
        public void Report(string message, object info)
        {
            Reports.Add(new ReportItem { Message = message, Information = info });
        }
    }
}
