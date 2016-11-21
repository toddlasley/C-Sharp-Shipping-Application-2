//Todd Lasley
//CIS 200-10
//6/22/15
//File used to define requirements and specifications for an EditAddress form.
//Enables the user to specify which address to edit.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Prog2
{
    public partial class EditAddress : Form
    {
        //backingfield for list of addresses
        private List<Address> addressList;
        
        //constructor for creating EditAddress Forms
        public EditAddress(List<Address> addresses)
        {
            InitializeComponent();
            addressList = addresses;
        }

        //property for editComboBox
        public int Edit
        {
            //get accessor
            get
            {
                return editComboBox.SelectedIndex;
            }
            //set accessor
            set
            {
                editComboBox.SelectedIndex = value;
            }
        }
        
        //load event for EditAddress
        //populates editComboBox
        private void EditAddress_Load(object sender, EventArgs e)
        {
            foreach (Address a in addressList)
            {
                editComboBox.Items.Add(a.Name);
            }
        }

        //event handler for edit button
        private void editButton_Click(object sender, EventArgs e)
        {
            if (ValidateChildren())
                this.DialogResult = DialogResult.OK;
        }

        //mouseDown event handler for cancel button
        private void cancelButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Was it a left-click?
                this.DialogResult = DialogResult.Cancel;
        }
    }
}
