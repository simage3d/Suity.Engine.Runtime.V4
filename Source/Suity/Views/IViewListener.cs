// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Views
{
    public interface IViewListener
    {
        void EnterView(int viewId);
        void ExitView(int viewId);
        void ViewEdited(int viewId);
    }
}
