// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Synchonizing;

namespace Suity.Views
{
    public interface IPreviewEdit : IPreviewDisplay
    {
        bool CanEditPreviewText { get; }
        void SetPreviewText(string text, ISyncContext setup);
    }
}
