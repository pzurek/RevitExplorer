#region Imported Namespaces

//.NET common used namespaces
using System;
using System.Windows.Forms;
using System.Collections.Generic;

//Revit.NET common used namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion

namespace ElementExplorer
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class Application : IExternalApplication
    {
        /// <summary>
        /// Implement this method to implement the external application which should be called when 
        /// Revit starts before a file or default template is actually loaded.
        /// </summary>
        /// <param name="application">An object that is passed to the external application which contains the controlled application.</param>
        /// <returns>Return the status of the external application. A result of Succeeded means that the external application successfully started. Cancelled can be used to signify that the user cancelled the external operation at some point. If false is returned then Revit should inform the user that the external application failed to load and the release the internal reference.</returns>
        public Result OnStartup(UIControlledApplication application)
        {
            //TODO: Add your code here


            //Must return some code
            return Result.Succeeded;
        }

        /// <summary>
        /// Implement this method to implement the external application which should be called when 
        /// Revit is about to exit. Any documents must have been closed before this method is called.
        /// </summary>
        /// <param name="application">An object that is passed to the external application which contains the controlled application.</param>
        /// <returns>Return the status of the external application. A result of Succeeded means that the external application successfully shutdown. Cancelled can be used to signify that the user cancelled the external operation at some point. If false is returned then the Revit user should be warned of the failure of the external application to shut down correctly.</returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            //TODO: Add your code here


            //Must return some code
            return Result.Succeeded;
        }
    }
}
