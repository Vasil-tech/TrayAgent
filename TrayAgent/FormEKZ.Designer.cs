namespace TrayAgent
{
   public  partial class FormEKZ
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEKZ));
			this.cmbxDB = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtbINV = new System.Windows.Forms.TextBox();
			this.txtbBOOK = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.txtMSG = new System.Windows.Forms.TextBox();

			this.timerekz = new System.Windows.Forms.Timer();
			//this.timerekz.Interval = CNST.CLIENT_TIME_LIEVE * 60000;
			this.timerekz.Tick += FormEKZ_Timer;
			
			this.SuspendLayout();
			// 
			// cmbxDB
			// 
			this.cmbxDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cmbxDB.FormattingEnabled = true;
			this.cmbxDB.Location = new System.Drawing.Point(103, 12);
			this.cmbxDB.Name = "cmbxDB";
			this.cmbxDB.Size = new System.Drawing.Size(193, 28);
			this.cmbxDB.TabIndex = 0;
			this.cmbxDB.SelectedIndexChanged +=new System.EventHandler(cbmxDB_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(6, 15);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label1.Size = new System.Drawing.Size(91, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "База данных";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label2.Location = new System.Drawing.Point(6, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(222, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Инвентарный номер, RFID-метка";
			// 
			// txtbINV
			// 
			this.txtbINV.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.txtbINV.Location = new System.Drawing.Point(9, 62);
			this.txtbINV.Name = "txtbINV";
			this.txtbINV.Size = new System.Drawing.Size(287, 26);
			this.txtbINV.TabIndex = 3;
			
			//this.txtbINV.TextChanged += this.txtbINV_AfterUpdate;
			this.txtbINV.KeyDown += this.txtbINV_KeyDown;    
			// 
			// txtbBOOK
			// 
			this.txtbBOOK.BackColor = System.Drawing.SystemColors.Control;
			this.txtbBOOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.txtbBOOK.Location = new System.Drawing.Point(9, 104);
			this.txtbBOOK.Multiline = true;
			this.txtbBOOK.Name = "txtbBOOK";
			this.txtbBOOK.Size = new System.Drawing.Size(410, 125);
			this.txtbBOOK.TabIndex = 4;
			// 
			// btnOK
			// 
			this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.btnOK.Location = new System.Drawing.Point(343, 62);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 26);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnOK.Hide();
			// 
			// txtMSG
			// 
			this.txtMSG.BackColor = System.Drawing.SystemColors.Control;
			this.txtMSG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtMSG.Enabled = true;
			
			this.txtMSG.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.txtMSG.Location = new System.Drawing.Point(9, 232);
			this.txtMSG.Name = "txtMSG";
			this.txtMSG.Size = new System.Drawing.Size(410, 42);
			this.txtMSG.TabIndex = 6;
			// 
			// FormEKZ
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(440, 261);
			this.Controls.Add(this.txtMSG);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtbBOOK);
			this.Controls.Add(this.txtbINV);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cmbxDB);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEKZ";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Отметка о поступлении";
			this.Load += new System.EventHandler(this.FormEKZ_Load);
			this.FormClosing += this.FormEKZ_FormClosing;
		   
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cmbxDB;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtbINV;
		private System.Windows.Forms.TextBox txtbBOOK;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox txtMSG;
	}
}