// Copyright 2011-2015 Chris Patterson, Dru Sellers
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Automatonymous.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;


    public class JsonStateSerializer<TStateMachine, TInstance>
        where TStateMachine : StateMachine<TInstance>
        where TInstance : class
    {
        readonly TStateMachine _machine;

        JsonSerializer _deserializer;

        JsonSerializer _serializer;

        public JsonStateSerializer(TStateMachine machine)
        {
            _machine = machine;
        }

        public JsonSerializer Deserializer
        {
            get
            {
                return _deserializer ?? (_deserializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Auto,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    Converters = new List<JsonConverter>(new JsonConverter[]
                    {
                        new StateConverter<TStateMachine>(_machine),
                    })
                }));
            }
        }

        public JsonSerializer Serializer
        {
            get
            {
                return _serializer ?? (_serializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Auto,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    Converters = new List<JsonConverter>(new JsonConverter[]
                    {
                        new StateConverter<TStateMachine>(_machine),
                    }),
                }));
            }
        }

        public string Serialize<T>(T instance)
            where T : TInstance
        {
            using (var ms = new MemoryStream())
            {
                Serialize(ms, instance);

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }


        public void Serialize<T>(Stream output, T instance)
            where T : TInstance
        {
            using (var writer = new StreamWriter(output))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;

                Serializer.Serialize(jsonWriter, instance);

                jsonWriter.Flush();
                writer.Flush();
            }
        }

        public T Deserialize<T>(string body)
            where T : TInstance
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(body)))
            {
                return Deserialize<T>(ms);
            }
        }

        public T Deserialize<T>(Stream input)
            where T : TInstance

        {
            using (var reader = new StreamReader(input))
            using (var jsonReader = new JsonTextReader(reader))
                return Deserializer.Deserialize<T>(jsonReader);
        }
    }
}