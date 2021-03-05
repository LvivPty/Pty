using System;
using System.Collections.Generic;
using System.Text;

namespace Pty.Network.Models.Commands
{
    public class FileModel:BaseModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
