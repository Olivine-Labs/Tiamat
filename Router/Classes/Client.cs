using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Alchemy.Server.Classes;
using System.Collections.Concurrent;
using Common;
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

        public Guid Id = Guid.Empty;
        public ConcurrentDictionary<Guid, Request> Requests = new ConcurrentDictionary<Guid, Request>();
        public UserContext Context = null;

        public ContentType ContentType = ContentType.JSON;

        public void Error(Request request, Message message)
        {
            Response response = new Response();
            response.Error = message;
            Respond(request, null, response);
        }

        public void Respond(Request request, object data, Response response = null)
        {
            try
            {
                if (response == null)
                    response = new Response();

                if (request != null)
                {
                    response.Path = request.Path;
                    if (request.ContentType == null)
                    {
                        String contentType = request.From.Context.Header["Content-Type"];
                        if (Enum.IsDefined(typeof(ContentType), contentType))
                        {
                            request.ContentType = (ContentType)Enum.Parse(typeof(ContentType), contentType, true);
                        }
                    }
                }
                else
                {
                    request = new Request();
                }
                response.Request = request;
                response.Data = data;
                Context.Send(serializeResponse(response));
            }
            finally//Make sure we clean up the request object regardless of whether the response gets sent or not.
            {
                Requests.TryRemove(request.Id, out request);
            }
        }

        private string serializeResponse(Response response)
        {
            //TODO handle multiple input/output types
            response.Time = Common.Functions.ConvertToUnixTimestamp(DateTime.Now);
            String responseString = string.Empty;
            switch (response.Request.ContentType)
            {
                case ContentType.XML:
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(response.GetType());
                    using (StringWriter writer = new StringWriter())
                    {
                        x.Serialize(writer, response);
                        responseString = writer.ToString();
                    }
                    break;
                case ContentType.JSON:
                default:
                    responseString = JsonConvert.SerializeObject(response, Formatting.None, _serializerSettings);
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
                switch (ContentType)
                {
                    case ContentType.XML:
                        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requests.GetType());
                        using (StringReader reader = new StringReader(Context.DataFrame.ToString()))
                        {
                            requests = x.Deserialize(reader) as Request[];
                        }
                        break;
                    case ContentType.JSON:
                    default:
                        requests = JsonConvert.DeserializeObject<Request[]>(Context.DataFrame.ToString());
                        break;
                }
                
            }
            catch (Exception)
            {
                //Ignore, we don't care why the request is malformed.
                //TODO:: Logging?
            }
            return requests;
        }
    }
}
