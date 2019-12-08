using System;
using System.Collections.Generic;
using System.Text;

namespace NuKeeper.Gitlab
{
    public static class GitLabHelpers
    {
#pragma warning disable CA1034 // Nested types should not be visible
        public static class AccessLevel
#pragma warning restore CA1034 // Nested types should not be visible
        {
            public static bool IsOwner(long      accessLevel) => accessLevel >= 50;
            public static bool IsMaintainer(long accessLevel) => accessLevel >= 40;
            public static bool IsDeveloper(long  accessLevel) => accessLevel >= 30;
            public static bool IsReporter(long   accessLevel) => accessLevel >= 20;
            public static bool IsGuest(long      accessLevel) => accessLevel >= 10;
        }
    }
}
