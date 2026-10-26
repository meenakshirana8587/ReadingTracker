using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingTracker.Application.Auth
{
    public record LoginRequest([property: Required][property: EmailAddress] string Email,
        [property: Required] string Password);
}
