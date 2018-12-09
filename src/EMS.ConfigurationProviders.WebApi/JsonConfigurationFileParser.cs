using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EMS.ConfigurationProviders.WebApi
{
    internal class JsonConfigurationFileParser
    {
        private readonly IDictionary<string, string> _data =
            new SortedDictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

        private readonly Stack<string> _context = new Stack<string>();
        private string _currentPath;
        private JsonTextReader _reader;

        private JsonConfigurationFileParser()
        {
        }

        public static IDictionary<string, string> Parse(Stream input)
        {
            return new JsonConfigurationFileParser().ParseStream(input);
        }

        private IDictionary<string, string> ParseStream(Stream input)
        {
            _data.Clear();
            _reader = new JsonTextReader(new StreamReader(input));
            _reader.DateParseHandling = DateParseHandling.None;
            VisitJObject(JObject.Load(_reader));
            return _data;
        }

        private void VisitJObject(JObject jObject)
        {
            foreach (JProperty property in jObject.Properties())
            {
                EnterContext(property.Name);
                VisitProperty(property);
                ExitContext();
            }
        }

        private void VisitProperty(JProperty property)
        {
            VisitToken(property.Value);
        }

        private void VisitToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    VisitJObject(token.Value<JObject>());
                    break;
                case JTokenType.Array:
                    VisitArray(token.Value<JArray>());
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                    VisitPrimitive(token.Value<JValue>());
                    break;
                default:
                    throw new FormatException();
            }
        }

        private void VisitArray(JArray array)
        {
            for (int index = 0; index < array.Count; ++index)
            {
                EnterContext(index.ToString());
                VisitToken(array[index]);
                ExitContext();
            }
        }

        private void VisitPrimitive(JValue data)
        {
            string currentPath = _currentPath;
            if (_data.ContainsKey(currentPath))
                throw new FormatException();
        }

        private void EnterContext(string context)
        {
            _context.Push(context);
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }
    }
}