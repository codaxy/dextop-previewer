using System;
using System.Collections.Generic;
using System.Text;
using Codaxy.Dextop.Forms;
using Codaxy.Dextop.Data;

namespace Codaxy.Dextop.Previewer.Tests
{
	[DextopForm]
    [DextopGrid]
	class Login
	{
		[DextopFormField(allowBlank = false, anchor = "0")]
        [DextopGridColumn]
		public String Username { get; set; }

		[DextopFormField(inputType = "password", anchor = "0")]
        [DextopGridColumn]
		public String Password { get; set; }
	}
}
