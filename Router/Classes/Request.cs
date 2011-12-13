using System;
using Common;

namespace Router.Classes
{
    internal class Request
    {
        public ContentType? ContentType;
        public String Data = "";

        public Client From;
        public Guid Id = Guid.NewGuid();
        public Server To;
        private String _path = "";
        private String[] _pathArray = new String[0];

        public String Path
        {
            get { return _path; }
            set
            {
                _path = value;
                _pathArray = _path.Split('/');
            }
        }

        public String[] PathArray
        {
            get { return _pathArray; }
            set
            {
                _pathArray = value;
                _path = String.Join("/", _pathArray);
            }
        }

        public void Error(Message message)
        {
            From.Error(this, message);
        }

        public void Respond(object data)
        {
            From.Respond(this, data);
        }
    }
}