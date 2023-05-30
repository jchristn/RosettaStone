using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosettaStone.Core
{
    public class ResultSet
    {
        #region Public-Members

        public string Key { get; set; } = null;
        public string Left { get; set; } = null;
        public string Right { get; set; } = null;
        public VendorMetadata Vendor { get; set; } = null;
        public List<VendorMetadata> Vendors { get; set; } = null;
        public CodecMetadata Codec { get; set; } = null;
        public List<CodecMetadata> Codecs { get; set; } = null;

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        public ResultSet()
        {

        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
