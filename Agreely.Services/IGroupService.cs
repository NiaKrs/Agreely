using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agreely.Services
{
    public interface IGroupService
    {
        int CreateGroup(string Name, string? Description, int UserId);

    }
}
