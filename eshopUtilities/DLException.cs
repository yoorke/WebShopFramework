using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace eshopUtilities
{
    public class DLException:BaseException, ISerializable
    {
        public DLException()
            : base()
        {

        }

        public DLException(string message)
            : base(message)
        {

        }

        public DLException(string message, System.Exception inner)
            : base(message, inner)
        {

        }

        protected DLException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
