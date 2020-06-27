namespace AWSRedisUtility
{
    partial class Form1
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
            this.lstResult = new System.Windows.Forms.ListBox();
            this.btnClusters = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnGet = new System.Windows.Forms.Button();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupKeyValue = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnGetAllKeys = new System.Windows.Forms.Button();
            this.btnGetAll = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtPhrase = new System.Windows.Forms.TextBox();
            this.cmbUsers = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupPOCO = new System.Windows.Forms.GroupBox();
            this.chkIsPOCO = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnFlushALL = new System.Windows.Forms.Button();
            this.chkFlushALL = new System.Windows.Forms.CheckBox();
            this.groupKeyValue.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupPOCO.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstResult
            // 
            this.lstResult.FormattingEnabled = true;
            this.lstResult.Location = new System.Drawing.Point(153, 12);
            this.lstResult.Name = "lstResult";
            this.lstResult.Size = new System.Drawing.Size(310, 563);
            this.lstResult.TabIndex = 1;
            // 
            // btnClusters
            // 
            this.btnClusters.Location = new System.Drawing.Point(10, 14);
            this.btnClusters.Name = "btnClusters";
            this.btnClusters.Size = new System.Drawing.Size(117, 23);
            this.btnClusters.TabIndex = 2;
            this.btnClusters.Text = "Clusters List";
            this.btnClusters.UseVisualStyleBackColor = true;
            this.btnClusters.Click += new System.EventHandler(this.btnClusters_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(9, 17);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(120, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add Item";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(9, 46);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(120, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove Item";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(9, 75);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(120, 23);
            this.btnGet.TabIndex = 2;
            this.btnGet.Text = "Get Item";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(9, 27);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(120, 20);
            this.txtKey.TabIndex = 3;
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(9, 67);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(120, 20);
            this.txtValue.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "key";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "value";
            // 
            // groupKeyValue
            // 
            this.groupKeyValue.Controls.Add(this.label1);
            this.groupKeyValue.Controls.Add(this.label2);
            this.groupKeyValue.Controls.Add(this.txtValue);
            this.groupKeyValue.Controls.Add(this.txtKey);
            this.groupKeyValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupKeyValue.Location = new System.Drawing.Point(10, 209);
            this.groupKeyValue.Name = "groupKeyValue";
            this.groupKeyValue.Size = new System.Drawing.Size(137, 97);
            this.groupKeyValue.TabIndex = 5;
            this.groupKeyValue.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnGetAllKeys);
            this.groupBox2.Controls.Add(this.btnGetAll);
            this.groupBox2.Controls.Add(this.btnClusters);
            this.groupBox2.Location = new System.Drawing.Point(10, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(137, 104);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            // 
            // btnGetAllKeys
            // 
            this.btnGetAllKeys.Location = new System.Drawing.Point(10, 71);
            this.btnGetAllKeys.Name = "btnGetAllKeys";
            this.btnGetAllKeys.Size = new System.Drawing.Size(117, 23);
            this.btnGetAllKeys.TabIndex = 3;
            this.btnGetAllKeys.Text = "Get All Keys";
            this.btnGetAllKeys.UseVisualStyleBackColor = true;
            this.btnGetAllKeys.Click += new System.EventHandler(this.btnGetAllKeys_Click);
            // 
            // btnGetAll
            // 
            this.btnGetAll.Location = new System.Drawing.Point(10, 42);
            this.btnGetAll.Name = "btnGetAll";
            this.btnGetAll.Size = new System.Drawing.Size(117, 23);
            this.btnGetAll.TabIndex = 2;
            this.btnGetAll.Text = "Get All Items";
            this.btnGetAll.UseVisualStyleBackColor = true;
            this.btnGetAll.Click += new System.EventHandler(this.btnGetAll_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.btnSearch);
            this.groupBox3.Controls.Add(this.txtPhrase);
            this.groupBox3.Location = new System.Drawing.Point(10, 483);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(137, 88);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "phrase";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(9, 57);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 23);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtPhrase
            // 
            this.txtPhrase.Location = new System.Drawing.Point(9, 31);
            this.txtPhrase.Name = "txtPhrase";
            this.txtPhrase.Size = new System.Drawing.Size(120, 20);
            this.txtPhrase.TabIndex = 5;
            // 
            // cmbUsers
            // 
            this.cmbUsers.FormattingEnabled = true;
            this.cmbUsers.Location = new System.Drawing.Point(8, 14);
            this.cmbUsers.Name = "cmbUsers";
            this.cmbUsers.Size = new System.Drawing.Size(120, 21);
            this.cmbUsers.TabIndex = 5;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnAdd);
            this.groupBox4.Controls.Add(this.btnRemove);
            this.groupBox4.Controls.Add(this.btnGet);
            this.groupBox4.Location = new System.Drawing.Point(10, 373);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(137, 109);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            // 
            // groupPOCO
            // 
            this.groupPOCO.Controls.Add(this.cmbUsers);
            this.groupPOCO.Enabled = false;
            this.groupPOCO.Location = new System.Drawing.Point(10, 308);
            this.groupPOCO.Name = "groupPOCO";
            this.groupPOCO.Size = new System.Drawing.Size(137, 44);
            this.groupPOCO.TabIndex = 9;
            this.groupPOCO.TabStop = false;
            // 
            // chkIsPOCO
            // 
            this.chkIsPOCO.AutoSize = true;
            this.chkIsPOCO.Location = new System.Drawing.Point(19, 357);
            this.chkIsPOCO.Name = "chkIsPOCO";
            this.chkIsPOCO.Size = new System.Drawing.Size(56, 17);
            this.chkIsPOCO.TabIndex = 10;
            this.chkIsPOCO.Text = "POCO";
            this.chkIsPOCO.UseVisualStyleBackColor = true;
            this.chkIsPOCO.CheckedChanged += new System.EventHandler(this.chkIsPOCO_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkFlushALL);
            this.groupBox1.Controls.Add(this.btnFlushALL);
            this.groupBox1.Location = new System.Drawing.Point(10, 121);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(137, 68);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // btnFlushALL
            // 
            this.btnFlushALL.Location = new System.Drawing.Point(9, 15);
            this.btnFlushALL.Name = "btnFlushALL";
            this.btnFlushALL.Size = new System.Drawing.Size(120, 23);
            this.btnFlushALL.TabIndex = 5;
            this.btnFlushALL.Text = "Flush ALL";
            this.btnFlushALL.UseVisualStyleBackColor = true;
            this.btnFlushALL.Click += new System.EventHandler(this.btnFlushALL_Click);
            // 
            // chkFlushALL
            // 
            this.chkFlushALL.AutoSize = true;
            this.chkFlushALL.Location = new System.Drawing.Point(10, 45);
            this.chkFlushALL.Name = "chkFlushALL";
            this.chkFlushALL.Size = new System.Drawing.Size(61, 17);
            this.chkFlushALL.TabIndex = 6;
            this.chkFlushALL.Text = "Confirm";
            this.chkFlushALL.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 585);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkIsPOCO);
            this.Controls.Add(this.groupPOCO);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupKeyValue);
            this.Controls.Add(this.lstResult);
            this.Name = "Form1";
            this.Text = "AWS Redis Utility";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.groupKeyValue.ResumeLayout(false);
            this.groupKeyValue.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupPOCO.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstResult;
        private System.Windows.Forms.Button btnClusters;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupKeyValue;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnGetAll;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPhrase;
        private System.Windows.Forms.ComboBox cmbUsers;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupPOCO;
        private System.Windows.Forms.CheckBox chkIsPOCO;
        private System.Windows.Forms.Button btnGetAllKeys;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnFlushALL;
        private System.Windows.Forms.CheckBox chkFlushALL;
    }
}

