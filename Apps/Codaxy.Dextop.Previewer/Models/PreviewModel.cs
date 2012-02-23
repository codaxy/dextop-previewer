using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codaxy.Dextop.Previewer.Models
{
    public class PreviewModel
    {
        public HtmlString InlineJsCode { get; set; }

		public string FilePath { get; set; }

        public bool IsUpload { get; set; }

        public int FormWidth { get; set; }
	}	
}