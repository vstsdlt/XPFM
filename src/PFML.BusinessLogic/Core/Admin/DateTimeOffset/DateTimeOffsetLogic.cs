using System;
using FACTS.Framework.Service;
using FACTS.Framework.Utility;

namespace PFML.BusinessLogic.Core.Admin.DateTimeOffset
{

    /// <summary>Methods to support Core/Admin/DateTimeOffset module</summary>
    public static class DateTimeOffsetLogic
    {

        /// <summary>Set offset on service tier</summary>
        /// <param name="name">Name of user (null for global)</param>
        /// <param name="offset">Offset to apply to user/global Date/Time operations</param>
        [OperationMethod]
        public static void SetOffset(string name, double offset)
        {
            DateTimeUtil.SetOffset(name, offset);
        }

    }

}
