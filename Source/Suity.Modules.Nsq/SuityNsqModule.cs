using Suity.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suity.Modules.Nsq
{
    public class SuityNsqModule : Module
    {
        public SuityNsqModule() : base(ModuleBindingNames.MessageQueue, "Nsq implementation for [MessageQueue]")
        {
        }

        protected override IModuleBinding Bind(ModuleConfig input)
        {
            return new SuityNsqBinding(input);
        }
    }
}
