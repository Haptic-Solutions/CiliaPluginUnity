using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.CiliaPlugin.Scripts
{
    [Serializable]
    class GroupIDs
    {

        public List<int> GroupID;
        public object Message;
        public GroupIDs()
        {
            GroupID = new List<int>();
        }
    }
}
