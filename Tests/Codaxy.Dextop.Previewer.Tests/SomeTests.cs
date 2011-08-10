using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using Codaxy.Dextop.Previewer.Engine;
using System.IO;
using Codaxy.Dextop;

namespace Codaxy.Dextop.Previewer.Tests
{
	[TestFixture]
	class SomeTests
	{
		[Test]
		public void Test()
		{
			var code = File.ReadAllText("TestCode.cs");
			var dynamicCompiler = new DynamicCompiler
			{
				BinPath = Environment.CurrentDirectory
			};
			var res = dynamicCompiler.Compile(code);            
		}
	}
}
