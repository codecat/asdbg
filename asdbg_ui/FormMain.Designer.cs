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
			this.listLocals = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.listFiles = new System.Windows.Forms.TreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.statusStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
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
			// listLocals
			// 
			this.listLocals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listLocals.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2});
			this.listLocals.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listLocals.FullRowSelect = true;
			this.listLocals.Location = new System.Drawing.Point(12, 376);
			this.listLocals.Name = "listLocals";
			this.listLocals.Size = new System.Drawing.Size(835, 153);
			this.listLocals.TabIndex = 4;
			this.listLocals.UseCompatibleStateImageBehavior = false;
			this.listLocals.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 150;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Type";
			this.columnHeader3.Width = 150;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Value";
			this.columnHeader2.Width = 390;
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
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(859, 554);
			this.Controls.Add(this.listFiles);
			this.Controls.Add(this.listLocals);
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
		private System.Windows.Forms.ListView listLocals;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ToolStripButton buttonStepOut;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.TreeView listFiles;
		private System.Windows.Forms.ImageList imageList1;
	}
}

