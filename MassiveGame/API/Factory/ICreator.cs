using MassiveGame.API.Factory.ObjectArguments;
using MassiveGame.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.API.Factory
{
    public interface ICreator
    {
        IVisible CreateInstance(Arguments a);
    }
}
