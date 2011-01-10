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
    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Commands : IExternalCommand
    {
        /// <summary>
        /// The one and only method required by the IExternalCommand interface,
        /// the main entry point for every external command.
        /// </summary>
        /// <param name="commandData">Input argument providing access to the Revit application and its documents and their properties.</param>
        /// <param name="message">Return argument to display a message to the user in case of error if Result is not Succeeded.</param>
        /// <param name="elements">Return argument to highlight elements on the graphics screen if Result is not Succeeded.</param>
        /// <returns>Cancelled, Failed or Succeeded Result code.</returns>
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            //TODO: Add your code here


            //Must return some code
            return Result.Succeeded;
        }
    }
}
