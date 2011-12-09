using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemy.Server;
using Alchemy.Server.Classes;
using Router.Classes;
using System.Net;
using Newtonsoft.Json;
using Constants;

namespace Router
{
    public class Router
    {
        private Alchemy.Server.WSServer _alchemy = null;
        private ConcurrentDictionary<Guid, Client> _clients = new ConcurrentDictionary<Guid, Client>();
        private ConcurrentDictionary<String, Server> _servers = new ConcurrentDictionary<String, Server>();
        
        public void Start()
        {
            if (_alchemy == null)
            {
                _alchemy = new Alchemy.Server.WSServer(81, IPAddress.Any);
                _alchemy.DefaultOnReceive = new OnEventDelegate(onReceive);
                _alchemy.DefaultOnSend = new OnEventDelegate(onSend);
                _alchemy.DefaultOnConnect = new OnEventDelegate(onConnect);
                _alchemy.DefaultOnConnected = new OnEventDelegate(onConnected);
                _alchemy.DefaultOnDisconnect = new OnEventDelegate(onDisconnect);
                _alchemy.TimeOut = new TimeSpan(0, 5, 0);
                _alchemy.Start();
            }
        }

        public void Stop()
        {
            if (_alchemy != null)
            {
                _alchemy.Stop();
                _alchemy = null;
            }
        }

        private void onReceive(UserContext context)
        {
            //TODO :: Logging?
            Client client = context.Data as Client;
            Request[] requests = client.GetRequests();

            if (requests.Length > 0)
            {
                foreach (Request request in requests)
                {
                    request.From = client;

                    if (request.InputType != null)
                        client.InputType = (ContentType)request.InputType;

                    //This tells us if we need to try to forward this request or not.
                    if (String.IsNullOrEmpty(request.Path))
                    {
                        if (tryForward(request))
                        {
                            //Only requests that are forwarded need to be tracked.
                            client.Requests.TryAdd(request.Id, request);
                        }
                    }
                }
            }
            else
            {
                client.Error(null, Message.REQUEST_MALFORMED);
            }
        }

        private void onSend(UserContext context)
        {
            //TODO :: Logging?
        }

        private void onConnect(UserContext context)
        {
            //TODO :: Logging?
        }

        private void onConnected(UserContext context)
        {
            //TODO :: Logging?
            Client client = new Client();
            client.Id = Guid.NewGuid();
            client.Context = context;
            context.Data = client;
            _clients.TryAdd(client.Id, client);
        }

        private void onDisconnect(UserContext context)
        {
            //TODO :: Logging?
            if (context.Data != null)
            {
                Client client = (Client)context.Data;
                _clients.TryRemove(client.Id, out client);
            }
        }

        private Server tryFindServer(String path)
        {
            Server server = null;
            if (!_servers.TryGetValue(path, out server))
            {
                //TODO:: find and connect to server for path, implement wait and retry if necessary
            }
            return server;
        }

        private bool tryForward(Request request)
        {
            String path = string.Empty;
            if (request.PathArray.Length > 0)
                path = request.PathArray[0];

            Server server = tryFindServer(path);

            //TODO:: implement retries on failed messages(limit number of retries) in both cases here.
            if (server != null)
            {
                server.Forward(request);
                return true;
            }
            else
            {
                request.Error(Message.SERVER_UNAVAILABLE);
                return false;
            }
        }
    }
}
