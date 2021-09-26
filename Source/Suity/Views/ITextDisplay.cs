// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Views
{
    public interface ITextDisplay
    {
        string Text { get; }
        object Icon { get; }
        TextStatus TextStatus { get; }
    }
}
