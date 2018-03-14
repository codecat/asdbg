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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabCallstack = new System.Windows.Forms.TabPage();
			this.gridCallstack = new System.Windows.Forms.DataGridView();
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tabLocals = new System.Windows.Forms.TabPage();
			this.tabBreakpoints = new System.Windows.Forms.TabPage();
			this.gridBreakpoints = new System.Windows.Forms.DataGridView();
			this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.statusStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridLocals)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabCallstack.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridCallstack)).BeginInit();
			this.tabLocals.SuspendLayout();
			this.tabBreakpoints.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridBreakpoints)).BeginInit();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 552);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(887, 22);
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
			this.editor.Size = new System.Drawing.Size(679, 362);
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
			this.toolStrip1.Size = new System.Drawing.Size(887, 25);
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
			this.buttonStepOver.Click += new System.EventHandler(this.buttonStepOver_Click);
			// 
			// buttonStepOut
			// 
			this.buttonStepOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonStepOut.Image = ((System.Drawing.Image)(resources.GetObject("buttonStepOut.Image")));
			this.buttonStepOut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonStepOut.Name = "buttonStepOut";
			this.buttonStepOut.Size = new System.Drawing.Size(23, 22);
			this.buttonStepOut.Text = "Step out";
			this.buttonStepOut.Click += new System.EventHandler(this.buttonStepOut_Click);
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
			this.listFiles.Size = new System.Drawing.Size(178, 362);
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
			this.gridLocals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridLocals.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.TypeColumn,
            this.ValueColumn});
			this.gridLocals.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridLocals.Location = new System.Drawing.Point(3, 3);
			this.gridLocals.MultiSelect = false;
			this.gridLocals.Name = "gridLocals";
			this.gridLocals.Size = new System.Drawing.Size(849, 121);
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
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabCallstack);
			this.tabControl1.Controls.Add(this.tabLocals);
			this.tabControl1.Controls.Add(this.tabBreakpoints);
			this.tabControl1.Location = new System.Drawing.Point(12, 396);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(863, 153);
			this.tabControl1.TabIndex = 7;
			// 
			// tabCallstack
			// 
			this.tabCallstack.Controls.Add(this.gridCallstack);
			this.tabCallstack.Location = new System.Drawing.Point(4, 22);
			this.tabCallstack.Name = "tabCallstack";
			this.tabCallstack.Padding = new System.Windows.Forms.Padding(3);
			this.tabCallstack.Size = new System.Drawing.Size(855, 127);
			this.tabCallstack.TabIndex = 1;
			this.tabCallstack.Text = "Callstack";
			this.tabCallstack.UseVisualStyleBackColor = true;
			// 
			// gridCallstack
			// 
			this.gridCallstack.AllowUserToAddRows = false;
			this.gridCallstack.AllowUserToDeleteRows = false;
			this.gridCallstack.AllowUserToResizeRows = false;
			this.gridCallstack.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridCallstack.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
			this.gridCallstack.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridCallstack.Location = new System.Drawing.Point(3, 3);
			this.gridCallstack.MultiSelect = false;
			this.gridCallstack.Name = "gridCallstack";
			this.gridCallstack.ReadOnly = true;
			this.gridCallstack.Size = new System.Drawing.Size(849, 121);
			this.gridCallstack.TabIndex = 7;
			// 
			// dataGridViewTextBoxColumn1
			// 
			this.dataGridViewTextBoxColumn1.HeaderText = "Declaration";
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.ReadOnly = true;
			this.dataGridViewTextBoxColumn1.Width = 350;
			// 
			// dataGridViewTextBoxColumn2
			// 
			this.dataGridViewTextBoxColumn2.HeaderText = "Filename";
			this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			this.dataGridViewTextBoxColumn2.ReadOnly = true;
			this.dataGridViewTextBoxColumn2.Width = 350;
			// 
			// dataGridViewTextBoxColumn3
			// 
			this.dataGridViewTextBoxColumn3.HeaderText = "Line";
			this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
			this.dataGridViewTextBoxColumn3.ReadOnly = true;
			this.dataGridViewTextBoxColumn3.Width = 80;
			// 
			// tabLocals
			// 
			this.tabLocals.Controls.Add(this.gridLocals);
			this.tabLocals.Location = new System.Drawing.Point(4, 22);
			this.tabLocals.Name = "tabLocals";
			this.tabLocals.Padding = new System.Windows.Forms.Padding(3);
			this.tabLocals.Size = new System.Drawing.Size(855, 127);
			this.tabLocals.TabIndex = 0;
			this.tabLocals.Text = "Locals";
			this.tabLocals.UseVisualStyleBackColor = true;
			// 
			// tabBreakpoints
			// 
			this.tabBreakpoints.Controls.Add(this.gridBreakpoints);
			this.tabBreakpoints.Location = new System.Drawing.Point(4, 22);
			this.tabBreakpoints.Name = "tabBreakpoints";
			this.tabBreakpoints.Padding = new System.Windows.Forms.Padding(3);
			this.tabBreakpoints.Size = new System.Drawing.Size(855, 127);
			this.tabBreakpoints.TabIndex = 2;
			this.tabBreakpoints.Text = "Breakpoints";
			this.tabBreakpoints.UseVisualStyleBackColor = true;
			// 
			// gridBreakpoints
			// 
			this.gridBreakpoints.AllowUserToAddRows = false;
			this.gridBreakpoints.AllowUserToDeleteRows = false;
			this.gridBreakpoints.AllowUserToResizeRows = false;
			this.gridBreakpoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridBreakpoints.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
			this.gridBreakpoints.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridBreakpoints.Location = new System.Drawing.Point(3, 3);
			this.gridBreakpoints.MultiSelect = false;
			this.gridBreakpoints.Name = "gridBreakpoints";
			this.gridBreakpoints.ReadOnly = true;
			this.gridBreakpoints.Size = new System.Drawing.Size(849, 121);
			this.gridBreakpoints.TabIndex = 8;
			this.gridBreakpoints.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridBreakpoints_CellDoubleClick);
			// 
			// dataGridViewTextBoxColumn5
			// 
			this.dataGridViewTextBoxColumn5.HeaderText = "Filename";
			this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
			this.dataGridViewTextBoxColumn5.ReadOnly = true;
			this.dataGridViewTextBoxColumn5.Width = 550;
			// 
			// dataGridViewTextBoxColumn6
			// 
			this.dataGridViewTextBoxColumn6.HeaderText = "Line";
			this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
			this.dataGridViewTextBoxColumn6.ReadOnly = true;
			this.dataGridViewTextBoxColumn6.Width = 80;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(887, 574);
			this.Controls.Add(this.tabControl1);
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
			this.tabControl1.ResumeLayout(false);
			this.tabCallstack.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridCallstack)).EndInit();
			this.tabLocals.ResumeLayout(false);
			this.tabBreakpoints.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridBreakpoints)).EndInit();
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
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabLocals;
		private System.Windows.Forms.TabPage tabCallstack;
		private System.Windows.Forms.DataGridView gridCallstack;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
		private System.Windows.Forms.TabPage tabBreakpoints;
		private System.Windows.Forms.DataGridView gridBreakpoints;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
	}
}

