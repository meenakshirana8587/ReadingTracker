using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingTracker.Application.Auth
{
    public record LoginRequest([Required][EmailAddress] string Email,
        [Required] string Password);
}
