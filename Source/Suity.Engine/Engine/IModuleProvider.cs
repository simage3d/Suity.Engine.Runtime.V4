// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity.Engine
{
    public interface IModuleProvider
    {
        T Bind<T>(string name, ModuleConfig config) where T : class;
    }

    public sealed class EmptyModuleProvider : IModuleProvider
    {
        public static readonly EmptyModuleProvider Empty = new EmptyModuleProvider();

        private EmptyModuleProvider()
        {
        }

        public T Bind<T>(string name, ModuleConfig config) where T : class
        {
            Logs.LogError($"EmptyModuleProvider bind module : {name}");
            return null;
        }
    }
}
