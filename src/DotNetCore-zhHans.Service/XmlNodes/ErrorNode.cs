using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Base;

namespace DotNetCoreZhHans.Service.XmlNodes
{
    class ErrorNode : NodeBase
    {
        public ErrorNode(IndexProvider indexProvider
            , ITransmitData transmits
            , XmlNode xmlNode
            , int index
            , NodeBase parent) : base(indexProvider, transmits, xmlNode, index, parent)
        {

        }
    }
}
