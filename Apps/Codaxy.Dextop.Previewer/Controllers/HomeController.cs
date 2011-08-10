using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codaxy.Dextop.Previewer.Engine;
using Codaxy.Dextop.Previewer.Models;
using System.IO;

namespace Codaxy.Dextop.Previewer.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/



		public ActionResult Index(PreviewModel data)
        {
			var model = new Models.PreviewModel();

            String src = null;

			if (!String.IsNullOrEmpty(data.FilePath))
                src = System.IO.File.ReadAllText(data.FilePath);
            else if (Request.Files.Count == 1)
            {
                var sr = new StreamReader(Request.Files[0].InputStream);
                src = sr.ReadToEnd();
                data.FilePath = Request.Files[0].FileName;
                model.IsUpload = true;
            }

            if (src != null)
            {
                var dc = new DynamicCompiler
                {
                    BinPath = HttpRuntime.BinDirectory
                };
                model.FilePath = data.FilePath;
                var code = dc.Compile(src);
                model.InlineJsCode = new HtmlString(code);
            }
        

            return View(model);
        }
    }
}
