using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Constants;

namespace Router.Classes
{
    class Request
    {
        public Guid Id = Guid.NewGuid();
        private String _path = "";
        private String[] _pathArray = new String[0];

        public String Data = "";

        public Client From = null;
        public Server To = null;

        public ContentType? OutputType = null;
        public ContentType? InputType = null;

        public String Path
        {
            get
            { return _path; }
            set
            {
                _path = value;
                _pathArray = _path.Split('/');
            }
        }

        public String[] PathArray
        {
            get
            { return _pathArray; }
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
