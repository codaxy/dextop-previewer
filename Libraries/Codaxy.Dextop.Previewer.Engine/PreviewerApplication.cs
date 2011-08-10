using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codaxy.Dextop.Previewer.Engine
{
    public class PreviewerApplication : DextopApplication
    {
        public PreviewerApplication()
        {
           
        }

        protected override void RegisterModules()
        {
            
        }

        public override string MapNamespace(string ns)
        {
            return "Previewer";
        }
    }
}
