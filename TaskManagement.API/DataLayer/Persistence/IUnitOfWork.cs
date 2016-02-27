using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Persistence
{
    public interface IUnitOfWork
    {
        void Save();
    }
}
