using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Controlling
{
    public interface IController
    {
        void Start(FunctionContext ctx);

        void Stop(FunctionContext ctx);

        void Enter(FunctionContext ctx);

        void Exit(FunctionContext ctx);

        void Update(FunctionContext ctx);

        void DoAction(FunctionContext ctx);
    }
}
