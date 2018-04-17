using System;
using System.Collections.Generic;

namespace WFTDC
{
    class DefaultConfig
    {
        public C.Configuration Config;

        public DefaultConfig()
        {
            C.User User = new C.User
            {
                Account = new C.Account
                {
                    Cookie = string.Empty,
                    GetCookieFrom = C.Account.GetCookieFromEnum.Chrome,
                    Enabled = false,
                    SetStatus = string.Empty
                },
                Id = Guid.NewGuid().ToString(),
                Platform = Platform.Pc,
                Region = Region.En,
                UserStates = new List<Status>
                {
                    Status.Ingame,
                    Status.Online
                }
            };

            Config = new C.Configuration
            {
                User = User,
                Items = new List<C.Item>()
            };
        }
    }
}
