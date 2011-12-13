using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml.Serialization;
using Alchemy.Server.Classes;
using Common;
using Newtonsoft.Json;

namespace Router.Classes
{
    internal class Client
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling =
                NullValueHandling.Ignore,
            DefaultValueHandling =
                DefaultValueHandling.Ignore
        };

        public ContentType ContentType = ContentType.Json;
        public UserContext Context;

        public Guid Id = Guid.Empty;
        public ConcurrentDictionary<Guid, Request> Requests = new ConcurrentDictionary<Guid, Request>();

        public void Error(Request request, Message message)
        {
            var response = new Response {Error = message};
            Respond(request, null, response);
        }

        public void Respond(Request request, object data, Response response = null)
        {
            if (response == null)
            {
                response = new Response();
            }
            if (request == null)
            {
                request = new Request();
            }

            try
            {
                response.Path = request.Path;
                if (request.ContentType == null)
                {
                    String contentType = request.From.Context.Header["Content-Type"];
                    if (Enum.IsDefined(typeof (ContentType), contentType))
                    {
                        request.ContentType = (ContentType) Enum.Parse(typeof (ContentType), contentType, true);
                    }
                }

                response.Request = request;
                response.Data = data;
                Context.Send(SerializeResponse(response));
            }
            finally //Make sure we clean up the request object regardless of whether the response gets sent or not.
            {
                Requests.TryRemove(request.Id, out request);
            }
        }

        private string SerializeResponse(Response response)
        {
            //TODO handle multiple input/output types
            response.Time = Functions.ConvertToUnixTimestamp(DateTime.Now);
            String responseString;
            switch (response.Request.ContentType)
            {
                case ContentType.Xml:
                    var x = new XmlSerializer(response.GetType());
                    using (var writer = new StringWriter())
                    {
                        x.Serialize(writer, response);
                        responseString = writer.ToString();
                    }
                    break;
                    //case ContentType.Json:
                default:
                    responseString = JsonConvert.SerializeObject(response, Formatting.None, SerializerSettings);
                    break;
            }
            return responseString;
        }

        public Request[] GetRequests()
        {
            //TODO handle multiple input/output types
            Request[] requests;

            switch (ContentType)
            {
                case ContentType.Xml:
                    var x = new XmlSerializer(typeof (Request[]));
                    using (var reader = new StringReader(Context.DataFrame.ToString()))
                    {
                        requests = x.Deserialize(reader) as Request[];
                    }
                    break;
                    //case ContentType.Json:
                default:
                    requests = JsonConvert.DeserializeObject<Request[]>(Context.DataFrame.ToString());
                    break;
            }

            //Ignore, we don't care why the request is malformed.
            //TODO:: Logging?

            return requests;
        }
    }
}