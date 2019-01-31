using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HelpForm
{
    /// <summary>
    /// Author:         Nicolas Vendeville
    /// Created:        August 19 2013
    /// Last Updated:   August 30 2013
    /// Summary:        Help window for the TomoBra Acquisition App.
    ///                 Reads / writes help files from program path // bin // Debug // Help
    /// </summary>
    public partial class HelpFormWindow : Form
    {
        String help_path = Application.StartupPath.ToString() + @"/Help";
        bool saving = false; // boolean that indicates whether a file is currently being edited

        public HelpFormWindow()
        {
            InitializeComponent();
            InitializeSelection();
        }

        /// <summary>
        /// Populates combobox selection list with files in the "Help" folder
        /// </summary>
        private void InitializeSelection()
        {
            StringBuilder sb = new StringBuilder();
            String name;
            foreach (string txtName in Directory.GetFiles(help_path, "*.txt"))
            {
                name = txtName.Replace(help_path, "");
                name = name.Replace(".txt", "");
                name = name.Replace(@"\", "");
                cmb_topic.Items.Add(name);
            }
            cmb_topic.SelectedIndex = 0;
            StreamReader reader = new StreamReader(help_path + @"\" + cmb_topic.SelectedItem.ToString() + ".txt");
            rtbx_description.Text = reader.ReadToEnd();
            reader.Close();
        }

        private void cmb_topic_SelectionChangeCommitted(object sender, EventArgs e)
        {
            rtbx_description.Clear();

            if (!File.Exists(help_path + @"\" + cmb_topic.SelectedItem.ToString() + ".txt"))
                return;

            StreamReader reader = new StreamReader(help_path + @"\" + cmb_topic.SelectedItem.ToString() + ".txt");
            rtbx_description.Text = reader.ReadToEnd();
            reader.Close();
        }

        private void bn_Edit_Click(object sender, EventArgs e)
        {
            if (!saving) // Edit
            {
                // Set interface for saving
                saving = true;

                cmb_topic.Visible = false;
                tbx_title.Visible = true;
                lb_topic.Text = "Title Name:";
                tbx_title.Text = cmb_topic.SelectedItem.ToString();

                // Buttons
                bn_Edit.Text = "Save Changes";
                bn_New.Enabled = false;
                bn_Delete.Text = "Cancel";
                bn_Delete.Tag = "1";

                // Allow writing
                rtbx_description.ReadOnly = false;


            }

            else // Save
            {
                // Save edit to text file
                String fileName = help_path + @"\" + tbx_title.Text + ".txt";
                System.IO.File.WriteAllText(fileName, rtbx_description.Text);

                // Reset interface
                saving = false;

                cmb_topic.Visible = true;
                tbx_title.Visible = false;
                lb_topic.Text = "Current Topic:";

                // Buttons
                bn_New.Enabled = true;
                bn_Delete.Text = "Delete";
                bn_Delete.Tag = "0";
                bn_Edit.Text = "Edit";

                // Allow reading
                cmb_topic.Items.Clear();
                InitializeSelection();
                cmb_topic.SelectedItem = tbx_title.Text;
                rtbx_description.ReadOnly = true;

                if (!File.Exists(help_path + @"\" + cmb_topic.SelectedItem.ToString() + ".txt"))
                    return;

                StreamReader reader = new StreamReader(help_path + @"\" + cmb_topic.SelectedItem.ToString() + ".txt");
                rtbx_description.Text = reader.ReadToEnd();
                reader.Close();
            }
        }

        private void bn_New_Click(object sender, EventArgs e)
        {
            if (!saving)
            {
                // Set interface for saving
                saving = true; 

                cmb_topic.Visible = false;
                tbx_title.Visible = true;
                lb_topic.Text = "Title Name:";
                tbx_title.Text = "Write title here...";

                // Buttons
                bn_New.Text = "Save";
                bn_Delete.Text = "Cancel";
                bn_Delete.Tag = "1";
                bn_Edit.Enabled = false;

                // Allow writing
                rtbx_description.Clear();
                rtbx_description.Text = "Write description here...";
                rtbx_description.ReadOnly = false;

            }
            else
            {
                // Save text file
                String fileName = help_path + @"\" + tbx_title.Text + ".txt";
                System.IO.File.WriteAllText(fileName, rtbx_description.Text);

                // Reset interface
                saving = false;

                cmb_topic.Visible = true;
                tbx_title.Visible = false;
                lb_topic.Text = "Current Topic:";

                // Buttons
                bn_New.Text = "New";
                bn_Delete.Text = "Delete";
                bn_Delete.Tag = "0";
                bn_Edit.Enabled = true;

                // Allow reading
                cmb_topic.Items.Clear();
                InitializeSelection();
                cmb_topic.SelectedItem = tbx_title.Text;
                rtbx_description.ReadOnly = true;

                if (!File.Exists(help_path + @"\" + cmb_topic.SelectedItem.ToString() + ".txt"))
                    return;

                StreamReader reader = new StreamReader(help_path + @"\" + cmb_topic.SelectedItem.ToString() + ".txt");
                rtbx_description.Text = reader.ReadToEnd();
                reader.Close();

            }


        }

        private void bn_Delete_Click(object sender, EventArgs e)
        {
            if (bn_Delete.Tag == "1") // Cancel
            {
                saving = false;

                // Reset buttons
                bn_Delete.Text = "Delete";
                bn_Delete.Tag = "0";

                bn_New.Text = "New";
                bn_New.Enabled = true;

                bn_Edit.Text = "Edit";
                bn_Edit.Enabled = true;

                // Reset textbox / combobox
                lb_topic.Text = "Current Topic:";
                rtbx_description.ReadOnly = true;

                cmb_topic.Visible = true;
                cmb_topic.Items.Clear();
                InitializeSelection();
            }

            else // Delete
            {
                DialogResult dr = MessageBox.Show("Delete " + cmb_topic.SelectedItem +"?", "Delete", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    try
                    {
                        System.IO.File.Delete(help_path + @"/" + cmb_topic.SelectedItem + ".txt");
                    }
                    catch (System.IO.IOException error)
                    {
                        MessageBox.Show("Cannot delete file");
                        return;
                    }
                }
                else
                    return;

                // reset cmb selection
                cmb_topic.Items.Clear();
                InitializeSelection();
            }
        }

    }
}
