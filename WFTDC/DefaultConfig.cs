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
                    GetMessages = false,
                    SetStatus = false
                },
                Id = Guid.NewGuid().ToString(),
                Platform = Payloads.Platform.Pc,
                Region = Payloads.Region.En,
                UserStates = new List<Payloads.Status>
                {
                    Payloads.Status.Ingame,
                    Payloads.Status.Online
                }
            };

            Config = new C.Configuration
            {
                User = User,
                Items = new List<C.Item>(),
                Application = new C.Application
                {
                    StartWithWindows = true,
                    Watcher = true
                }
            };
        }
    }
}
