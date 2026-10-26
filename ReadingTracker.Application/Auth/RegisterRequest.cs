using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingTracker.Application.Auth
{
    public record RegisterRequest(string Email, string Password, string DisplayName);
}
