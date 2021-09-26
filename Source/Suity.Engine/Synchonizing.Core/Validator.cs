// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Suity.Synchonizing.Core
{
    public static class Validator
    {
        public static IEnumerable<SyncPathReportItem> Find(object owner, object target, string findStr, FindOption findOption, VisitFlag flag = VisitFlag.None)
        {
            List<SyncPathReportItem> reports = new List<SyncPathReportItem>();
            ValidationContext context = new ValidationContext();

            Visitor.Visit<IValidate>(
                target,
                (validate, pathContext) => {
                    validate.Find(context, findStr, findOption);
                    foreach (var item in context.Reports)
                    {
                        SyncPathReportItem report = new SyncPathReportItem(owner, pathContext.GetPath(), item.Information, item.Message);
                        reports.Add(report);
                    }
                    context.Reports.Clear();
                },
                flag);

            return reports;
        }
        public static IEnumerable<SyncPathReportItem> Validate(object owner, object target, VisitFlag flag = VisitFlag.None)
        {
            List<SyncPathReportItem> reports = new List<SyncPathReportItem>();
            ValidationContext context = new ValidationContext();

            Visitor.Visit<IValidate>(
                target, 
                (validate, pathContext) => {
                    validate.Validate(context);
                    foreach (var item in context.Reports)
                    {
                        SyncPathReportItem report = new SyncPathReportItem(owner, pathContext.GetPath(), item.Information, item.Message);
                        reports.Add(report);
                    }
                    context.Reports.Clear();
                },
                flag);

            return reports;
        }


        public static bool Compare(string source, string find, FindOption option)
        {
            if ((option & FindOption.CaseSensetive) == 0)
            {
                source = source.ToLowerInvariant();
                find = find.ToLowerInvariant();
            }

            if ((option & FindOption.FullCompare) != 0)
            {
                //return source == find;
                string pattern = String.Format(@"\b{0}\b", find);

                try
                {
                    var match = Regex.Match(source, pattern);
                    return match.Success;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return source.Contains(find);
            }
        }
    }
}
