// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Synchonizing;
using Suity.Views;


namespace Suity.Engine
{
    public enum StringAllocationMode
    {
        Custom,
        EnvironmentConfig,
        ServiceId,
        GalaxyName,
        GalaxyId,
        ApplicationName,
        ClassTypeInfo,
        None,
    }

    public class StringConfig : IViewObject
    {
        public StringAllocationMode Mode { get; set; } = StringAllocationMode.Custom;
        public string Value { get; set; }
        public string ConfigKey { get; set; }

        AssetRef<ClassTypeInfo> _classTypeInfoRef = new AssetRef<ClassTypeInfo>();

        public StringConfig()
        {
        }
        public StringConfig(string value)
        {
            Mode = StringAllocationMode.Custom;
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
                    case StringAllocationMode.EnvironmentConfig:
                        setup.InspectorField(ConfigKey, new ViewProperty("ConfigKey"));
                        break;
                    case StringAllocationMode.Custom:
                        setup.InspectorField(Value, new ViewProperty("Value"));
                        break;
                    case StringAllocationMode.ClassTypeInfo:
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
            Mode = sync.Sync("Mode", Mode, SyncFlag.AffectsOthers, StringAllocationMode.Custom);
            Value = sync.Sync("Value", Value, SyncFlag.NotNull);
            ConfigKey = sync.Sync("ConfigKey", ConfigKey);
            if (Mode == StringAllocationMode.ClassTypeInfo)
            {
                _classTypeInfoRef = sync.Sync("TypeInfo", _classTypeInfoRef, SyncFlag.NotNull);
            }

            if (NodeApplication.Current != null)
            {
                sync.Sync("LiveValue", GetValue(), SyncFlag.ReadOnly);
            }
        } 
        #endregion

        public string GetValue()
        {
            var app = NodeApplication.Current;

            if (app == null)
            {
                return null;
            }

            switch (Mode)
            {
                case StringAllocationMode.ServiceId:
                    return app.ServiceId;
                case StringAllocationMode.GalaxyName:
                    return app.GalaxyName;
                case StringAllocationMode.GalaxyId:
                    return app.GalaxyId;
                case StringAllocationMode.ApplicationName:
                    return app.ApplicationName;
                case StringAllocationMode.EnvironmentConfig:
                    return Environment.GetConfig(ConfigKey);
                case StringAllocationMode.ClassTypeInfo:
                    return _classTypeInfoRef.Key;
                case StringAllocationMode.None:
                    return null;
                case StringAllocationMode.Custom:
                default:
                    return Value;
            }
        }

        public override string ToString()
        {
            if (NodeApplication.Current == null)
            {
                switch (Mode)
                {
                    case StringAllocationMode.ServiceId:
                        return "[ServiceId]";
                    case StringAllocationMode.GalaxyName:
                        return "[GalaxyName]";
                    case StringAllocationMode.GalaxyId:
                        return "[GalaxyId]";
                    case StringAllocationMode.ApplicationName:
                        return "[ApplicationName]";
                    case StringAllocationMode.EnvironmentConfig:
                        return "Config:" + ConfigKey;
                    case StringAllocationMode.ClassTypeInfo:
                        return "TypeInfo:" + _classTypeInfoRef.ToString();
                    case StringAllocationMode.None:
                        return string.Empty;
                    case StringAllocationMode.Custom:
                    default:
                        return Value;
                }
            }
            else
            {
                return GetValue();
            }
        }
    }
}
