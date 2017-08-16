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
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.buttonStep = new System.Windows.Forms.Button();
			this.editor = new ScintillaNET.Scintilla();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 341);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(572, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// labelStatus
			// 
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(40, 17);
			this.labelStatus.Text = "Wait...";
			// 
			// buttonStep
			// 
			this.buttonStep.Location = new System.Drawing.Point(12, 12);
			this.buttonStep.Name = "buttonStep";
			this.buttonStep.Size = new System.Drawing.Size(75, 23);
			this.buttonStep.TabIndex = 1;
			this.buttonStep.Text = "Step";
			this.buttonStep.UseVisualStyleBackColor = true;
			this.buttonStep.Click += new System.EventHandler(this.buttonStep_Click);
			// 
			// editor
			// 
			this.editor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.editor.Location = new System.Drawing.Point(12, 41);
			this.editor.Name = "editor";
			this.editor.Size = new System.Drawing.Size(548, 297);
			this.editor.TabIndex = 2;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(572, 363);
			this.Controls.Add(this.editor);
			this.Controls.Add(this.buttonStep);
			this.Controls.Add(this.statusStrip1);
			this.Name = "FormMain";
			this.Text = "AngelScript Debugger";
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel labelStatus;
		private System.Windows.Forms.Button buttonStep;
		private ScintillaNET.Scintilla editor;
	}
}

