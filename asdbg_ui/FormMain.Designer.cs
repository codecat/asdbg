namespace asdbg_ui
{
	partial class FormMain
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
			if (disposing && (components != null)) {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.editor = new ScintillaNET.Scintilla();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.buttonResume = new System.Windows.Forms.ToolStripButton();
			this.buttonPause = new System.Windows.Forms.ToolStripButton();
			this.buttonStep = new System.Windows.Forms.ToolStripButton();
			this.buttonStepOver = new System.Windows.Forms.ToolStripButton();
			this.buttonStepOut = new System.Windows.Forms.ToolStripButton();
			this.listFiles = new System.Windows.Forms.TreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.gridLocals = new System.Windows.Forms.DataGridView();
			this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.statusStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridLocals)).BeginInit();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 532);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(859, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// labelStatus
			// 
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(40, 17);
			this.labelStatus.Text = "Wait...";
			// 
			// editor
			// 
			this.editor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.editor.Location = new System.Drawing.Point(196, 28);
			this.editor.Name = "editor";
			this.editor.Size = new System.Drawing.Size(651, 342);
			this.editor.TabIndex = 2;
			this.editor.MarginClick += new System.EventHandler<ScintillaNET.MarginClickEventArgs>(this.editor_MarginClick);
			this.editor.StyleNeeded += new System.EventHandler<ScintillaNET.StyleNeededEventArgs>(this.editor_StyleNeeded);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonResume,
            this.buttonPause,
            this.buttonStep,
            this.buttonStepOver,
            this.buttonStepOut});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(859, 25);
			this.toolStrip1.TabIndex = 3;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// buttonResume
			// 
			this.buttonResume.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonResume.Image = ((System.Drawing.Image)(resources.GetObject("buttonResume.Image")));
			this.buttonResume.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonResume.Name = "buttonResume";
			this.buttonResume.Size = new System.Drawing.Size(23, 22);
			this.buttonResume.Text = "Resume";
			this.buttonResume.Click += new System.EventHandler(this.buttonResume_Click);
			// 
			// buttonPause
			// 
			this.buttonPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonPause.Image = ((System.Drawing.Image)(resources.GetObject("buttonPause.Image")));
			this.buttonPause.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonPause.Name = "buttonPause";
			this.buttonPause.Size = new System.Drawing.Size(23, 22);
			this.buttonPause.Text = "Pause";
			this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
			// 
			// buttonStep
			// 
			this.buttonStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonStep.Image = ((System.Drawing.Image)(resources.GetObject("buttonStep.Image")));
			this.buttonStep.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonStep.Name = "buttonStep";
			this.buttonStep.Size = new System.Drawing.Size(23, 22);
			this.buttonStep.Text = "Step into";
			this.buttonStep.Click += new System.EventHandler(this.buttonStep_Click);
			// 
			// buttonStepOver
			// 
			this.buttonStepOver.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonStepOver.Image = ((System.Drawing.Image)(resources.GetObject("buttonStepOver.Image")));
			this.buttonStepOver.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonStepOver.Name = "buttonStepOver";
			this.buttonStepOver.Size = new System.Drawing.Size(23, 22);
			this.buttonStepOver.Text = "Step over";
			// 
			// buttonStepOut
			// 
			this.buttonStepOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonStepOut.Image = ((System.Drawing.Image)(resources.GetObject("buttonStepOut.Image")));
			this.buttonStepOut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonStepOut.Name = "buttonStepOut";
			this.buttonStepOut.Size = new System.Drawing.Size(23, 22);
			this.buttonStepOut.Text = "Step out";
			// 
			// listFiles
			// 
			this.listFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listFiles.FullRowSelect = true;
			this.listFiles.ImageIndex = 0;
			this.listFiles.ImageList = this.imageList1;
			this.listFiles.Location = new System.Drawing.Point(12, 28);
			this.listFiles.Name = "listFiles";
			this.listFiles.SelectedImageIndex = 0;
			this.listFiles.ShowLines = false;
			this.listFiles.Size = new System.Drawing.Size(178, 342);
			this.listFiles.TabIndex = 5;
			this.listFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.listFiles_AfterSelect);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "Folder");
			this.imageList1.Images.SetKeyName(1, "Script");
			// 
			// gridLocals
			// 
			this.gridLocals.AllowUserToAddRows = false;
			this.gridLocals.AllowUserToDeleteRows = false;
			this.gridLocals.AllowUserToResizeRows = false;
			this.gridLocals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridLocals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridLocals.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.TypeColumn,
            this.ValueColumn});
			this.gridLocals.Location = new System.Drawing.Point(12, 376);
			this.gridLocals.MultiSelect = false;
			this.gridLocals.Name = "gridLocals";
			this.gridLocals.Size = new System.Drawing.Size(835, 153);
			this.gridLocals.TabIndex = 6;
			this.gridLocals.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridLocals_CellEndEdit);
			// 
			// NameColumn
			// 
			this.NameColumn.HeaderText = "Name";
			this.NameColumn.Name = "NameColumn";
			this.NameColumn.ReadOnly = true;
			this.NameColumn.Width = 150;
			// 
			// TypeColumn
			// 
			this.TypeColumn.HeaderText = "Type";
			this.TypeColumn.Name = "TypeColumn";
			this.TypeColumn.ReadOnly = true;
			this.TypeColumn.Width = 150;
			// 
			// ValueColumn
			// 
			this.ValueColumn.HeaderText = "Value";
			this.ValueColumn.Name = "ValueColumn";
			this.ValueColumn.Width = 460;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(859, 554);
			this.Controls.Add(this.gridLocals);
			this.Controls.Add(this.listFiles);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.editor);
			this.Controls.Add(this.statusStrip1);
			this.Name = "FormMain";
			this.Text = "AngelScript Debugger";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridLocals)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel labelStatus;
		private ScintillaNET.Scintilla editor;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton buttonStep;
		private System.Windows.Forms.ToolStripButton buttonResume;
		private System.Windows.Forms.ToolStripButton buttonStepOver;
		private System.Windows.Forms.ToolStripButton buttonPause;
		private System.Windows.Forms.ToolStripButton buttonStepOut;
		private System.Windows.Forms.TreeView listFiles;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.DataGridView gridLocals;
		private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn TypeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn ValueColumn;
	}
}

