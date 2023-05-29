using Newtonsoft.Json.Linq;
using Rebus.Config;
using Rebus.Serialization;
using Rebus.Serialization.Json;

namespace EasyDesk.RebusCompanions.Core.Json;

public static class JsonExtensions
{
    public static void AlwaysDeserializeAsJObject(this StandardConfigurer<ISerializer> serialization)
    {
        serialization.UseNewtonsoftJson(JsonInteroperabilityMode.PureJson);
        serialization
            .OtherService<IMessageTypeNameConvention>()
            .Decorate(_ => new JObjectMessageTypeNameConvention());
    }

    private class JObjectMessageTypeNameConvention : IMessageTypeNameConvention
    {
        public string GetTypeName(Type type) => typeof(JObject).Name;

        public Type GetType(string name) => typeof(JObject);
    }
}
