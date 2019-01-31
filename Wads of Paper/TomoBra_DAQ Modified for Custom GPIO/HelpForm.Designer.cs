namespace HelpForm
{
    partial class HelpFormWindow
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
            this.components = new System.ComponentModel.Container();
            this.lb_topic = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmb_topic = new System.Windows.Forms.ComboBox();
            this.rtbx_description = new System.Windows.Forms.RichTextBox();
            this.bn_New = new System.Windows.Forms.Button();
            this.bn_Edit = new System.Windows.Forms.Button();
            this.bn_Delete = new System.Windows.Forms.Button();
            this.tbx_title = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lb_topic
            // 
            this.lb_topic.AutoSize = true;
            this.lb_topic.Location = new System.Drawing.Point(12, 13);
            this.lb_topic.Name = "lb_topic";
            this.lb_topic.Size = new System.Drawing.Size(74, 13);
            this.lb_topic.TabIndex = 1;
            this.lb_topic.Text = "Current Topic:";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // cmb_topic
            // 
            this.cmb_topic.FormattingEnabled = true;
            this.cmb_topic.Location = new System.Drawing.Point(15, 29);
            this.cmb_topic.Name = "cmb_topic";
            this.cmb_topic.Size = new System.Drawing.Size(121, 21);
            this.cmb_topic.TabIndex = 3;
            this.cmb_topic.Text = "Select a topic...";
            this.cmb_topic.SelectionChangeCommitted += new System.EventHandler(this.cmb_topic_SelectionChangeCommitted);
            // 
            // rtbx_description
            // 
            this.rtbx_description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbx_description.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rtbx_description.Location = new System.Drawing.Point(15, 65);
            this.rtbx_description.Name = "rtbx_description";
            this.rtbx_description.ReadOnly = true;
            this.rtbx_description.Size = new System.Drawing.Size(489, 268);
            this.rtbx_description.TabIndex = 5;
            this.rtbx_description.Text = "";
            // 
            // bn_New
            // 
            this.bn_New.Location = new System.Drawing.Point(164, 29);
            this.bn_New.Name = "bn_New";
            this.bn_New.Size = new System.Drawing.Size(86, 21);
            this.bn_New.TabIndex = 6;
            this.bn_New.Text = "New";
            this.bn_New.UseVisualStyleBackColor = true;
            this.bn_New.Click += new System.EventHandler(this.bn_New_Click);
            // 
            // bn_Edit
            // 
            this.bn_Edit.Location = new System.Drawing.Point(270, 29);
            this.bn_Edit.Name = "bn_Edit";
            this.bn_Edit.Size = new System.Drawing.Size(86, 21);
            this.bn_Edit.TabIndex = 7;
            this.bn_Edit.Text = "Edit";
            this.bn_Edit.UseVisualStyleBackColor = true;
            this.bn_Edit.Click += new System.EventHandler(this.bn_Edit_Click);
            // 
            // bn_Delete
            // 
            this.bn_Delete.Location = new System.Drawing.Point(376, 29);
            this.bn_Delete.Name = "bn_Delete";
            this.bn_Delete.Size = new System.Drawing.Size(86, 21);
            this.bn_Delete.TabIndex = 8;
            this.bn_Delete.Tag = "0";
            this.bn_Delete.Text = "Delete";
            this.bn_Delete.UseVisualStyleBackColor = true;
            this.bn_Delete.Click += new System.EventHandler(this.bn_Delete_Click);
            // 
            // tbx_title
            // 
            this.tbx_title.Location = new System.Drawing.Point(15, 30);
            this.tbx_title.Name = "tbx_title";
            this.tbx_title.Size = new System.Drawing.Size(105, 20);
            this.tbx_title.TabIndex = 9;
            this.tbx_title.Visible = false;
            // 
            // HelpFormWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 345);
            this.Controls.Add(this.bn_Delete);
            this.Controls.Add(this.bn_Edit);
            this.Controls.Add(this.bn_New);
            this.Controls.Add(this.rtbx_description);
            this.Controls.Add(this.lb_topic);
            this.Controls.Add(this.cmb_topic);
            this.Controls.Add(this.tbx_title);
            this.Name = "HelpFormWindow";
            this.Text = "Help Menu";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_topic;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ComboBox cmb_topic;
        private System.Windows.Forms.RichTextBox rtbx_description;
        private System.Windows.Forms.Button bn_New;
        private System.Windows.Forms.Button bn_Edit;
        private System.Windows.Forms.Button bn_Delete;
        private System.Windows.Forms.TextBox tbx_title;
    }
}

