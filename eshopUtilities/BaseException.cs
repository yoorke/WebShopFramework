using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace eshopUtilities
{
    public class BaseException:System.Exception, ISerializable
    {
        public BaseException()
            : base()
        {

        }

        public BaseException(string message)
            : base(message)
        {

        }

        public BaseException(string message, System.Exception inner)
            : base(message, inner)
        {

        }

        protected BaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
