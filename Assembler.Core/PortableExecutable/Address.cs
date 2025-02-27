using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Core.PortableExecutable;


public class Address
{
    public uint VirtualAddress { get; set; }
    public uint RelativeVirtualAddress {  get; set; }

}