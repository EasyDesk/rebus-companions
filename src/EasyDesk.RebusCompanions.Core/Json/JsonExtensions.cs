using Rebus.Config;
using Rebus.Serialization;
using Rebus.Serialization.Json;
using System.Text.Json.Nodes;

namespace EasyDesk.RebusCompanions.Core.Json;

public static class JsonExtensions
{
    public static void AlwaysDeserializeAnyObject(this StandardConfigurer<ISerializer> serialization)
    {
        serialization.UseSystemTextJson();
        serialization
            .OtherService<IMessageTypeNameConvention>()
            .Decorate(_ => new AnyObjectMessageTypeNameConvention());
    }

    private class AnyObjectMessageTypeNameConvention : IMessageTypeNameConvention
    {
        public string GetTypeName(Type type) => typeof(JsonObject).Name;

        public Type GetType(string name) => typeof(JsonObject);
    }
}
