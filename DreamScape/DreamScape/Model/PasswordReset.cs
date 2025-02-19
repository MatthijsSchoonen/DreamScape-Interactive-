using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Model
{
    class PasswordReset
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Code {get; set;}
    }
}
