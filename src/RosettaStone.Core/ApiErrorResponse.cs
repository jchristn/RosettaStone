using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosettaStone.Core
{
    public class ApiErrorResponse
    {
        #region Public-Members

        public int StatusCode { get; set; } = 200;
        public string Message { get; set; } = null;
        public string Context { get; set; } = null;
        public Exception Exception { get; set; } = null;

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        public ApiErrorResponse()
        {

        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
