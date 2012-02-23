using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using Codaxy.Dextop;
using Codaxy.Common;

namespace Codaxy.Dextop.Previewer.Engine
{
	public class DynamicCompiler
	{
		public String BinPath { get; set; }

		public String Compile(String code)
		{            
			CSharpCodeProvider provider = new CSharpCodeProvider();		
			CompilerParameters cp = new CompilerParameters
			{
				GenerateInMemory = true
			};
			cp.ReferencedAssemblies.Add("System.dll");
			cp.ReferencedAssemblies.Add("System.Data.dll");
			cp.ReferencedAssemblies.Add(Path.Combine(BinPath, "Codaxy.Dextop.dll"));

			var rc = RewriteCode(code);

			var results = provider.CompileAssemblyFromSource(cp, rc);
			foreach (CompilerError error in results.Errors)
				throw new Exception(error.ErrorText);           
            

			var assembly = results.CompiledAssembly;

            StringBuilder res = new StringBuilder();
            
            var app = new PreviewerApplication();            
            using (var ss = new MemoryStream())
            {                
                var columnProcessor = new Data.DextopGridHeaderPreprocessor();
                columnProcessor.ProcessAssemblies(app, new[] { assembly }, ss);
                res.AppendLine(Encoding.UTF8.GetString(ss.ToArray()));
            }

            using (var ss = new MemoryStream())
            {
                var formProcessor = new Forms.DextopFormPreprocessor();
                formProcessor.ProcessAssemblies(app, new[] { assembly }, ss);
                res.AppendLine(Encoding.UTF8.GetString(ss.ToArray()));               
            }

            return res.ToString();            
		}

		public String RewriteCode(String code)
		{
			StringBuilder res = new StringBuilder();
			res.AppendLine("using System;");
			res.AppendLine("using Codaxy.Dextop;");
			res.AppendLine("using Codaxy.Dextop.Forms;");
			res.AppendLine("using Codaxy.Dextop.Data;");

			res.AppendLine();
			res.AppendLine("namespace Codaxy.Dextop.Previewer {");

			int start = 0;
			do
			{
				var grid = code.IndexOf("[DextopGrid", start);
				var form = code.IndexOf("[DextopForm", start);
				if (grid != -1 && form != -1)
					start = Math.Min(grid, form);
				else if (form != -1)
					start = form;
				else if (grid != -1)
					start = grid;
				else
					break;                

				int pos = code.IndexOf('{', start); //class {
                if (pos == -1)
                    break;

                var colonIndex = code.IndexOf(':', start);
                if (colonIndex > start && colonIndex < pos)
                {
                    code = code.Remove(colonIndex, pos - colonIndex);
                    pos = colonIndex;
                }
				
				int braces = 1;
				while (braces > 0 && ++pos < code.Length)
				{
					switch (code[pos])
					{
						case '{': braces++; break;
						case '}': braces--; break;

					}
				}
				if (braces == 0)
				{
					pos++;
					res.AppendLine(code.Substring(start, pos - start));
				}
				else
					break;
				start = pos;
			} while (start < code.Length);

			res.AppendLine("}");
			return res.ToString();
		}
	}
}
