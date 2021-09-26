// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Synchonizing;
using Suity.Views;


namespace Suity.Engine
{
    public enum VersionConfigMode
    {
        Auto,
        Custom,
        EnvironmentConfig,
        ClassTypeInfo,
    }

    public class VersionConfig : IViewObject
    {
        public VersionConfigMode Mode { get; set; } = VersionConfigMode.Auto;
        public string Value { get; set; }
        public string ConfigKey { get; set; }
        AssetRef<ClassTypeInfo> _classTypeInfoRef = new AssetRef<ClassTypeInfo>();

        public AssetRef<ClassTypeInfo> ClassTypeInfoRef => _classTypeInfoRef;

        public VersionConfig()
        {
        }
        public VersionConfig(string value)
        {
            Mode = VersionConfigMode.Custom;
            Value = value;
        }

        #region IViewObject
        void IViewObject.SetupView(IViewObjectSetup setup)
        {
            if (NodeApplication.Current == null)
            {
                setup.InspectorField(Mode, new ViewProperty("Mode"));
                switch (Mode)
                {
                    case VersionConfigMode.EnvironmentConfig:
                        setup.InspectorField(ConfigKey, new ViewProperty("ConfigKey"));
                        break;
                    case VersionConfigMode.Custom:
                        setup.InspectorField(Value, new ViewProperty("Value"));
                        break;
                    case VersionConfigMode.ClassTypeInfo:
                        setup.InspectorField(_classTypeInfoRef, new ViewProperty("TypeInfo"));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                setup.InspectorField(Value, new ViewProperty("LiveValue") { ReadOnly = true });
            }
        }

        void ISyncObject.Sync(IPropertySync sync, ISyncContext context)
        {
            Mode = sync.Sync("Mode", Mode, SyncFlag.AffectsOthers, VersionConfigMode.Auto);
            Value = sync.Sync("Value", Value, SyncFlag.NotNull);
            ConfigKey = sync.Sync("ConfigKey", ConfigKey);
            _classTypeInfoRef = sync.Sync("TypeInfo", _classTypeInfoRef, SyncFlag.NotNull);

            if (NodeApplication.Current != null)
            {
                sync.Sync("LiveValue", GetValue(), SyncFlag.ReadOnly);
            }
        }
        #endregion

        public string GetValue()
        {
            if (NodeApplication.Current == null)
            {
                return null;
            }

            switch (Mode)
            {
                case VersionConfigMode.Custom:
                    return Value;
                case VersionConfigMode.EnvironmentConfig:
                    return Environment.GetConfig(ConfigKey);
                case VersionConfigMode.ClassTypeInfo:
                    return _classTypeInfoRef.Key;
                case VersionConfigMode.Auto:
                default:
                    return string.Empty;
            }
        }

        public override string ToString()
        {
            if (NodeApplication.Current == null)
            {
                switch (Mode)
                {
                    case VersionConfigMode.Custom:
                        return Value;
                    case VersionConfigMode.EnvironmentConfig:
                        return "Config:" + ConfigKey;
                    case VersionConfigMode.ClassTypeInfo:
                        return "TypeInfo:" + _classTypeInfoRef.ToString();
                    case VersionConfigMode.Auto:
                    default:
                        return "[Auto]";
                }
            }
            else
            {
                return GetValue();
            }
        }
    }
}
