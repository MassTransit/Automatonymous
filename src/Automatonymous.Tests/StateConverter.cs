// Copyright 2011-2013 Chris Patterson, Dru Sellers
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
    using System;
    using System.Globalization;
    using Newtonsoft.Json;


    public class StateConverter<T> :
        JsonConverter
        where T : StateMachine
    {
        readonly T _machine;

        public StateConverter(T machine)
        {
            _machine = machine;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var state = (State)value;
            string text = state.Name;
            if (string.IsNullOrEmpty(text))
                text = "";

            writer.WriteValue(text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default(State);

            if (reader.TokenType == JsonToken.String)
            {
                var text = (string)reader.Value;
                if (string.IsNullOrWhiteSpace(text))
                    return default(State);

                return _machine.GetState((string)reader.Value);
            }

            throw new JsonReaderException(string.Format(CultureInfo.InvariantCulture,
                "Error reading State. Expected a string but got {0}.", new object[] {reader.TokenType}));
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(State).IsAssignableFrom(objectType);
        }
    }
}