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
using Common;

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
            if (requests == null)
            {
                client.Error(null, Message.REQUEST_MALFORMED);
            }
            else
            if(requests.Length > 0)
            {
                foreach (Request request in requests)
                {
                    request.From = client;

                    if (tryForward(request))
                    {
                        //Only requests that are forwarded need to be tracked.
                        client.Requests.TryAdd(request.Id, request);
                    }
                    else
                    {
                        client.Error(request, Message.SERVER_UNAVAILABLE);
                    }
                }
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
                Client client = context.Data as Client;
                _clients.TryRemove(client.Id, out client);
            }
        }

        private Server tryFindServer(String path)
        {
            Server server = null;
            if (_servers.TryGetValue(path, out server))
            {
                //TODO:: find and connect to server for path, implement wait and retry if necessary
            }
            else
            {
                //TODO:: Try to find server for path in database(maybe it hasn't propagated to here yet?). Connect and add to list if necessary.
                //TODO:: Make sure to add a special lock here(by request path) to prevent the same server from being added/connected to twice.
            }

            return server;
        }

        private Boolean tryForward(Request request)
        {
            String path = string.Empty;
            if (request.PathArray.Length > 0)
                path = request.PathArray[0];

            Server server = tryFindServer(path);
            if (server != null)
            {
                if(!server.Forward(request))
                {
                    //TODO:: implement retries on failed messages(limit number of retries) 
                }
                return true;//return true because we don't want to send an error in this case.
            }
            return false;
        }
    }
}
