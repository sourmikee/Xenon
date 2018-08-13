﻿#region

using System.ComponentModel;
using System.Threading.Tasks;
using Discord.Commands;

#endregion

namespace Discord.Addons.Interactive
{
    public class EnsureFromUserCriterion : ICriterion<IMessage>
    {
        private readonly ulong _id;

        public EnsureFromUserCriterion(IUser user)
        {
            _id = user.Id;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public EnsureFromUserCriterion(ulong id)
        {
            _id = id;
        }

        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, IMessage parameter)
        {
            var ok = _id == parameter.Author.Id;
            return Task.FromResult(ok);
        }
    }
}