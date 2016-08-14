using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace eshopUtilities
{
    public class BLException:BaseException, ISerializable
    {
        public BLException()
            : base()
        {

        }

        public BLException(string message)
            : base(message)
        {

        }

        public BLException(string message, System.Exception inner)
            : base(message, inner)
        {

        }

        protected BLException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
