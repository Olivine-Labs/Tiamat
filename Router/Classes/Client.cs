using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Alchemy.Server.Classes;
using System.Collections.Concurrent;
using Constants;
using Newtonsoft.Json;
using System.IO;

namespace Router.Classes
{
    class Client
    {
        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        public ContentType OutputType   = ContentType.JSON;
        public ContentType InputType    = ContentType.JSON;

        public Guid Id = Guid.Empty;
        public ConcurrentDictionary<Guid, Request> Requests = new ConcurrentDictionary<Guid, Request>();
        public UserContext Context = null;

        public void Error(Request request, Message message)
        {
            Response response = new Response();
            response.Error = message;
            Respond(request, null, response);
        }

        public void Respond(Request request, object data, Response response = null)
        {
            if(response == null)
                response = new Response();
            ContentType outputType = OutputType;
            if (request != null)
            {
                response.Path = request.Path;
                if (request.OutputType != null)
                    outputType = (ContentType)request.OutputType;
            }
            response.Data = data;
            Context.Send(serializeResponse(response, outputType));
        }

        private string serializeResponse(Response response, ContentType? outputType = null)
        {
            if (outputType == null)
                outputType = OutputType;
            //TODO handle multiple input/output types
            response.Time = convertToUnixTimestamp(DateTime.Now);
            String responseString = string.Empty;
            switch (outputType)
            {
                case ContentType.JSON:
                    responseString = JsonConvert.SerializeObject(response, Formatting.None, _serializerSettings);
                    break;
                case ContentType.XML:
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(response.GetType());
                    using (StringWriter writer = new StringWriter())
                    {
                        x.Serialize(writer, response);
                        responseString = writer.ToString();
                    }
                    break;
            }
            return responseString;
        }

        public Request[] GetRequests()
        {
            //TODO handle multiple input/output types
            Request[] requests = null;
            try
            {
                switch (InputType)
                {
                    case ContentType.JSON:
                        requests = JsonConvert.DeserializeObject<Request[]>(Context.DataFrame.ToString());
                        break;
                    case ContentType.XML:
                        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requests.GetType());
                        using (StringReader reader = new StringReader(Context.DataFrame.ToString()))
                        {
                            requests = x.Deserialize(reader) as Request[];
                        }
                        break;
                    default:
                        throw new Exception("Invalid Output Type");
                }
                
            }
            catch (Exception)
            {
                //Ignore, we don't care why the request is malformed.
            }
            if (requests == null)
                requests = new Request[0];
            return requests;
        }

        private static double convertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return diff.TotalSeconds;
        }
    }
}
