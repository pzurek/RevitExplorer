#region Imported Namespaces

//.NET common used namespaces
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

//Revit.NET common used namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;

using RevitExplorer;

using View = Autodesk.Revit.DB.View;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace RevitExplorer
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class Application : IExternalApplication
    {
        UIControlledApplication uiCtrlApp;
        ControlledApplication app;
        Document activeDoc;
        View activeView;
        FilteredElementCollector collector;
        IList<Element> structuralElements;
        Dialog dialog;

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                uiCtrlApp = application;
                app = uiCtrlApp.ControlledApplication;
                dialog = new Dialog();
                DataGridView elementGridView = dialog.elementGridView;

                #region Filter definitions
                ElementStructuralTypeFilter structuralTypeFilter = new ElementStructuralTypeFilter(Autodesk.Revit.DB.Structure.StructuralType.NonStructural, true);
                #endregion

                app.DocumentOpened += ((object o, DocumentOpenedEventArgs e) => {
                    UpdateDocument(e.Document);
                    setupElementGridView(elementGridView, structuralElements);
                    dialog.ShowDialog();
                });

                uiCtrlApp.ViewActivated += ((object o, ViewActivatedEventArgs e) =>
                {
                    UpdateActiveView(e);
                });

            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private void UpdateActiveView(ViewActivatedEventArgs e)
        {
            if (e.Status == EventStatus.Succeeded)
                activeView = e.CurrentActiveView;
        }

        private void UpdateDocument(Document document)
        {
            activeDoc = document;
            activeView = document.ActiveView;
            collector = new FilteredElementCollector(activeDoc);
            structuralElements = collector.WherePasses(new ElementStructuralTypeFilter(Autodesk.Revit.DB.Structure.StructuralType.NonStructural, true)).ToElements();
        }

        private void setupElementGridView(DataGridView gridView, IList<Element> elements)
        {
            var textCell = new DataGridViewTextBoxCell();
            var imageCell = new DataGridViewImageCell();
            var imageCellStyle = new DataGridViewCellStyle();
            imageCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            var elementNameColumn = new DataGridViewColumn(textCell);
            var elementImageColumn = new DataGridViewColumn(imageCell);

            gridView.Columns.Add(elementNameColumn);
            gridView.Columns.Add(elementImageColumn);

            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            gridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            if (gridView.Rows.Count > 0)
                gridView.Rows.Clear();

            foreach (Element element in elements)
            {
                ElementType elementType = activeDoc.get_Element(element.GetTypeId()) as ElementType;
                //Bitmap image = elementType.GetPreviewImage(new Size(100, 100));

                object[] newRow = {
                                      element.Name//,
                                      //image
                                  };
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
