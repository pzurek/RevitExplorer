namespace RevitExplorer
{
    partial class Dialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.elementGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // elementGridView
            // 
            this.elementGridView.AllowUserToAddRows = false;
            this.elementGridView.AllowUserToDeleteRows = false;
            this.elementGridView.AllowUserToOrderColumns = true;
            this.elementGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.elementGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementGridView.Location = new System.Drawing.Point(0, 0);
            this.elementGridView.Name = "elementGridView";
            this.elementGridView.ReadOnly = true;
            this.elementGridView.Size = new System.Drawing.Size(284, 262);
            this.elementGridView.TabIndex = 0;
            // 
            // Dialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.elementGridView);
            this.Name = "Dialog";
            this.Text = "Dialog";
            ((System.ComponentModel.ISupportInitialize)(this.elementGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView elementGridView;
    }
}