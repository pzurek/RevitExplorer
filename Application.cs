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
        bool documentLoaded;

        public Result OnStartup(UIControlledApplication application)
        {
            documentLoaded = false;

            try
            {
                uiCtrlApp = application;
                app = uiCtrlApp.ControlledApplication;
                dialog = new Dialog();
                DataGridView elementGridView = dialog.elementGridView;
                setupElementGridView(elementGridView);
                Button applyButton = dialog.applyButton;
                Button revertButton = dialog.revertButton;
                Button closeButton = dialog.closeButton;
                Label activeViewLabel = dialog.activeViewLabel;

                #region Filter definitions
                ElementStructuralTypeFilter structuralTypeFilter = new ElementStructuralTypeFilter(Autodesk.Revit.DB.Structure.StructuralType.NonStructural, true);
                #endregion

                app.DocumentOpened += ((object o, DocumentOpenedEventArgs e) =>
                {
                    UpdateDocument(e.Document);
                    documentLoaded = true;
                    activeViewLabel.Text = e.Document.ActiveView.ViewName;
                    populateElementGridView(elementGridView, structuralElements, activeView);
                    dialog.ShowDialog();
                });

                app.DocumentClosed += ((object o, DocumentClosedEventArgs e) =>
                {
                    documentLoaded = false;
                });

                uiCtrlApp.ViewActivated += ((object o, ViewActivatedEventArgs e) =>
                {
                    if (documentLoaded)
                    {
                        UpdateActiveView(e);
                        activeViewLabel.Text = e.CurrentActiveView.ViewName;
                        populateElementGridView(elementGridView, structuralElements, activeView);
                        dialog.ShowDialog();
                    }
                });

                closeButton.Click += ((object o, EventArgs e) => dialog.Close());

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

        private void populateElementGridView(DataGridView gridView, IList<Element> elements, View view)
        {
            if (gridView.Rows.Count > 0)
                gridView.Rows.Clear();

            foreach (Element element in elements)
            {
                ElementType elementType = activeDoc.get_Element(element.GetTypeId()) as ElementType;
                Bitmap image = elementType.GetPreviewImage(new Size(100, 100));
                object[] newRow = {element.Id, element.Name, !element.IsHidden(activeView)};
                gridView.Rows.Add(newRow);
            }
        }

        private static void setupElementGridView(DataGridView gridView)
        {
            var textCell = new DataGridViewTextBoxCell();
            var checkCell = new DataGridViewCheckBoxCell();
            var checkCellStyle = new DataGridViewCellStyle();
            checkCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var elementIdColumn = new DataGridViewColumn(textCell);

            var elementNameColumn = new DataGridViewColumn(textCell);
            elementNameColumn.HeaderText = "Element Name";

            var elementVisibilityColumn = new DataGridViewColumn(checkCell);
            elementVisibilityColumn.HeaderText = "Visible";

            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            gridView.RowHeadersVisible = false;

            gridView.Columns.Add(elementIdColumn);
            gridView.Columns.Add(elementNameColumn);
            gridView.Columns.Add(elementVisibilityColumn);

            gridView.Columns[0].Visible = false;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
