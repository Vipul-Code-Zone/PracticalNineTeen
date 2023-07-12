using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalNineteen.Models.ViewModels
{
    public class UserManagerRespose
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public DateTime? ExpireDate { get; set; }

        public string? Email { get; set; }
    }
}
