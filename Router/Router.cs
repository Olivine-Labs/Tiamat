using System;
using System.Collections.Concurrent;
using System.Net;
using Alchemy.Server;
using Alchemy.Server.Classes;
using Common;
using Router.Classes;

namespace Router
{
    public class Router
    {
        private readonly ConcurrentDictionary<Guid, Client> _clients = new ConcurrentDictionary<Guid, Client>();
        private readonly ConcurrentDictionary<String, Server> _servers = new ConcurrentDictionary<String, Server>();
        private WSServer _alchemy;

        public void Start()
        {
            if (_alchemy == null)
            {
                _alchemy = new WSServer(81, IPAddress.Any)
                {
                    DefaultOnReceive = OnReceive,
                    DefaultOnSend = OnSend,
                    DefaultOnConnect = OnConnect,
                    DefaultOnConnected = OnConnected,
                    DefaultOnDisconnect = OnDisconnect,
                    TimeOut = new TimeSpan(0, 5, 0)
                };
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

        private void OnReceive(UserContext context)
        {
            //TODO :: Logging?
            var client = context.Data as Client;
            if (client != null)
            {
                Request[] requests = client.GetRequests();
                if (requests != null)
                {
                    if (requests.Length > 0)
                    {
                        foreach (Request request in requests)
                        {
                            request.From = client;

                            if (TryForward(request))
                            {
                                //Only requests that are forwarded need to be tracked.
                                client.Requests.TryAdd(request.Id, request);
                            }
                            else
                            {
                                client.Error(request, Message.ServerUnavailable);
                            }
                        }
                    }
                }
                else
                {
                    client.Error(null, Message.RequestMalformed);
                }
            }
            else
            {
                context.Send(String.Empty, true); //Close
            }
        }

        private void OnSend(UserContext context)
        {
            //TODO :: Logging?
        }

        private void OnConnect(UserContext context)
        {
            //TODO :: Logging?
        }

        private void OnConnected(UserContext context)
        {
            //TODO :: Logging?
            var client = new Client
            {
                Id = Guid.NewGuid(),
                Context = context
            };
            context.Data = client;
            _clients.TryAdd(client.Id, client);
        }

        private void OnDisconnect(UserContext context)
        {
            //TODO :: Logging?
            if (context.Data != null)
            {
                var client = context.Data as Client;
                if (client != null)
                {
                    _clients.TryRemove(client.Id, out client);
                }
            }
        }

        private Server TryFindServer(String path)
        {
            Server server;
            if (_servers.TryGetValue(path, out server))
            {
                //TODO:: find and connect to server for path, implement wait and retry if necessary
            }
                // ReSharper disable RedundantIfElseBlock
            else
            {
                //TODO:: Try to find server for path in database(maybe it hasn't propagated to here yet?). Connect and add to list if necessary.
                //TODO:: Make sure to add a special lock here(by request path) to prevent the same server from being added/connected to twice.
            }
            // ReSharper restore RedundantIfElseBlock

            return server;
        }

        private Boolean TryForward(Request request)
        {
            String path = string.Empty;
            if (request.PathArray.Length > 0)
            {
                path = request.PathArray[0];
            }

            request.To = TryFindServer(path);
            if (request.To != null)
            {
                if (!request.To.Forward(request))
                {
                    //TODO:: implement retries on failed messages(limit number of retries) 
                }
                return true; //return true because we don't want to send an error in this case.
            }
            return false;
        }
    }
}