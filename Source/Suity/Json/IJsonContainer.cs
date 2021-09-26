namespace ComputerBeacon.Json
{
    interface IJsonContainer {
        bool IsArray { get; }
        void InternalAdd(string key, object value);
    }
}
