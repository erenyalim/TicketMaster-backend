using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Domain.Users;

namespace TicketMaster.Application.Services;
    public interface IJwtProvider
    {

    public Task<string> CreateTokenAsync(AppUser user, CancellationToken cancellationToken = default);
    }

