#region Imported Namespaces

//.NET common used namespaces
using System;
using System.Diagnostics;
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
        IEnumerable<Element> allElements;
        IEnumerable<Element> structuralElements;
        Dialog dialog;
        bool documentLoaded;
        Process process;
        IntPtr handle;
        RevitWindowHandle revitWindowHandle;

        public Result OnStartup(UIControlledApplication application)
        {
            documentLoaded = false;

            try
            {
                process = Process.GetCurrentProcess();
                handle = process.MainWindowHandle;
                revitWindowHandle = new RevitWindowHandle(handle);

                uiCtrlApp = application;
                app = uiCtrlApp.ControlledApplication;

                app.DocumentOpened += ((object o, DocumentOpenedEventArgs e) =>
                {
                    UpdateDocument(e.Document);
                    documentLoaded = true;
                    ShowDialog();
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
                        ShowDialog();
                    }
                });
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private void SetupDialog()
        {
            dialog = new Dialog();
            dialog.StartPosition = FormStartPosition.CenterScreen;
            dialog.revertButton.Enabled = false;
            dialog.applyButton.Enabled = false;

            dialog.closeButton.Click += ((object o, EventArgs e) => dialog.Close());

            dialog.applyButton.Click += ((object o, EventArgs e) =>
            {
                applyChanges(dialog.elementGridView);
                dialog.revertButton.Enabled = false;
                dialog.applyButton.Enabled = false;
            });

            dialog.revertButton.Click += ((object o, EventArgs e) =>
            {
                setupElementGridView(dialog.elementGridView, structuralElements);
                dialog.revertButton.Enabled = false;
                dialog.applyButton.Enabled = false;
            });

            //Enabling Apply and Revert buttons when cell value changes
            //It should actually track the value and go back to disabled if there is no change
            dialog.elementGridView.CellContentClick += ((object o, DataGridViewCellEventArgs e) =>
            {
                if (e.ColumnIndex != 2)
                    return;

                dialog.applyButton.Enabled = true;
                dialog.revertButton.Enabled = true;
            });
        }

        private void ShowDialog()
        {
            if (dialog == null || dialog.IsDisposed)
                SetupDialog();

            dialog.activeViewLabel.Text = activeView.ViewName;
            setupElementGridView(dialog.elementGridView, structuralElements);

            //I only want to enable the dialog if the active view is a plan
            //a section or a 3D view
            if (activeView is ViewPlan ||
                activeView is ViewSection ||
                activeView is View3D)
                EnableDialog();
            else
                DisableDialog();

            if (dialog.Visible)
                return;

            dialog.Show(revitWindowHandle);
        }

        //Disabling all the children of the dialog but not the dialog itself
        //I want to disable all elements of the dialog to make it unusable
        //but keep the dialog itself movable
        private void DisableDialog()
        {
            foreach (System.Windows.Forms.Control control in dialog.Controls)
                control.Enabled = false;
        }
        
        //Opposite to the one above
        private void EnableDialog()
        {
            foreach (System.Windows.Forms.Control control in dialog.Controls)
                control.Enabled = true;
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
            if (gridView.ColumnCount > 0)
                gridView.Columns.Clear();

            var textCell = new DataGridViewTextBoxCell();
            var checkCell = new DataGridViewCheckBoxCell();
            var checkCellStyle = new DataGridViewCellStyle();
            checkCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var elementIdColumn = new DataGridViewColumn(textCell);

            var elementNameColumn = new DataGridViewColumn(textCell);
            elementNameColumn.HeaderText = "Element Name";
            elementNameColumn.ReadOnly = true;

            var elementVisibilityColumn = new DataGridViewColumn(checkCell);
            elementVisibilityColumn.HeaderText = "Visible";

            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            gridView.RowHeadersVisible = false;

            gridView.Columns.Add(elementIdColumn);
            gridView.Columns.Add(elementNameColumn);
            gridView.Columns.Add(elementVisibilityColumn);

            gridView.Columns[0].Visible = false;

            if (gridView.Rows.Count > 0)
                gridView.Rows.Clear();

            foreach (Element element in elements)
            {
                ElementType elementType = activeDoc.get_Element(element.GetTypeId()) as ElementType;
                object[] newRow = { element.Id, element.Name, !element.IsHidden(activeView) };
                gridView.Rows.Add(newRow);
            }
        }

        void applyChanges(DataGridView gridView)
        {
            foreach (DataGridViewRow row in gridView.Rows)
            {
                var element = activeDoc.get_Element(row.Cells[0].Value as ElementId);
                ElementSet elementSet = new ElementSet();
                elementSet.Insert(element);

                Transaction transaction = new Transaction(activeDoc);
                transaction.Start(string.Format("Applying visibility of element {0}", element.Id.ToString()));

                if ((bool)row.Cells[2].Value == true)
                    activeView.Unhide(elementSet);
                else
                    if (element.CanBeHidden(activeView))
                        activeView.Hide(elementSet);

                transaction.Commit();
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
