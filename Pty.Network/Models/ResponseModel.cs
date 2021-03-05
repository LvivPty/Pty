using Pty.Network.Core;

namespace Pty.Network.Models
{
    public class ResponseModel
    {
        public Command Command { get; set; }
        public StatusCode StatusCode { get; set; }
        public string Data { get; set; }
    }
}
