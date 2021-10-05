using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ttt.Models.DTOs;

namespace ttt.Models.Repositories
{
    public interface IRepository
    {
        List<Test> GetList();
        void Create(Test obj);
    }
}
