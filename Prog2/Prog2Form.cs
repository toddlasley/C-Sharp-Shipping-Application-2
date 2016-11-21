//Todd Lasley
//CIS 200-10
//Program 3
//Program uses serialization to open and save file of addresses
//Used in collusion with Dr. Wright's Program 2 solution as shown blow.

// Program 2
// CIS 200
// Summer 2015
// Due: 6/11/2015
// By: Andrew L. Wright

// File: Prog2Form.cs
// This class creates the main GUI for Program 2. It provides a
// File menu with About and Exit items, an Insert menu with Address and
// Letter items, and a Report menu with List Addresses and List Parcels
// items.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace Prog2
{
    public partial class Prog2Form : Form
    {
        private List<Address> addressList; // The list of addresses
        private List<Parcel> parcelList;   // The list of parcels

        // object for serializing RecordSerializables in binary format
        private BinaryFormatter formatter = new BinaryFormatter();
        private FileStream output; // stream for writing to a file

        // Precondition:  None
        // Postcondition: The form's GUI is prepared for display. A few test addresses are
        //                added to the list of addresses
        public Prog2Form()
        {
            InitializeComponent();
        }

        // Precondition:  File, About menu item activated
        // Postcondition: Information about author displayed in dialog box
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(String.Format("Program 2{0}By: Andrew L. Wright{0}" +
                "CIS 200{0}Summer 2015", Environment.NewLine), "About Program 2");
        }

        // Precondition:  File, Exit menu item activated
        // Postcondition: The application is exited
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Precondition:  Insert, Address menu item activated
        // Postcondition: The Address dialog box is displayed. If data entered
        //                are OK, an Address is created and added to the list
        //                of addresses
        private void addressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddressForm addressForm = new AddressForm(); // The address dialog box form
            DialogResult result = addressForm.ShowDialog(); // Show form as dialog and store result

            if (result == DialogResult.OK) // Only add if OK
            {
                try
                {
                    Address newAddress = new Address(addressForm.AddressName, addressForm.Address1,
                        addressForm.Address2, addressForm.City, addressForm.State,
                        int.Parse(addressForm.ZipText)); // Use form's properties to create address
                    addressList.Add(newAddress);
                }
                catch (FormatException) // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Address Validation!", "Validation Error");
                }
            }

            addressForm.Dispose(); // Best practice for dialog boxes
        }

        // Precondition:  Report, List Addresses menu item activated
        // Postcondition: The list of addresses is displayed in the addressResultsTxt
        //                text box
        private void listAddressesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder(); // Holds text as report being built
                                                        // StringBuilder more efficient than String

            result.Append("Addresses:");
            result.Append(Environment.NewLine); // Remember, \n doesn't always work in GUIs
            result.Append(Environment.NewLine);

            foreach (Address a in addressList)
            {
                result.Append(a.ToString());
                result.Append(Environment.NewLine);
                result.Append(Environment.NewLine);
            }

            reportTxt.Text = result.ToString();

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }

        // Precondition:  Insert, Letter menu item activated
        // Postcondition: The Letter dialog box is displayed. If data entered
        //                are OK, a Letter is created and added to the list
        //                of parcels
        private void letterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LetterForm letterForm; // The letter dialog box form
            DialogResult result;   // The result of showing form as dialog

            if (addressList.Count < LetterForm.MIN_ADDRESSES) // Make sure we have enough addresses
            {
                MessageBox.Show("Need " + LetterForm.MIN_ADDRESSES + " addresses to create letter!",
                    "Addresses Error");
                return;
            }

            letterForm = new LetterForm(addressList); // Send list of addresses
            result = letterForm.ShowDialog();

            if (result == DialogResult.OK) // Only add if OK
            {
                try
                {
                    // For this to work, LetterForm's combo boxes need to be in same
                    // order as addressList
                    Letter newLetter = new Letter(addressList[letterForm.OriginAddressIndex],
                        addressList[letterForm.DestinationAddressIndex],
                        decimal.Parse(letterForm.FixedCostText)); // Letter to be inserted
                    parcelList.Add(newLetter);
                }
                catch (FormatException) // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Letter Validation!", "Validation Error");
                }
            }

            letterForm.Dispose(); // Best practice for dialog boxes
        }

        // Precondition:  Report, List Parcels menu item activated
        // Postcondition: The list of parcels is displayed in the parcelResultsTxt
        //                text box
        private void listParcelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder(); // Holds text as report being built
                                                        // StringBuilder more efficient than String
            decimal totalCost = 0;                      // Running total of parcel shipping costs

            result.Append("Parcels:");
            result.Append(Environment.NewLine); // Remember, \n doesn't always work in GUIs
            result.Append(Environment.NewLine);

            foreach (Parcel p in parcelList)
            {
                result.Append(p.ToString());
                result.Append(Environment.NewLine);
                result.Append(Environment.NewLine);
                totalCost += p.CalcCost();
            }

            result.Append("------------------------------");
            result.Append(Environment.NewLine);
            result.Append(String.Format("Total Cost: {0:C}", totalCost));

            reportTxt.Text = result.ToString();

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }

        //event handler for File > Save Addresses
        //serializes the AddressList
        private void saveAddressesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result; //variable used to store response from the save dialog box
            string filename; //stores filename

            //following try-catch blocks were taken from Figure 17.9 in textbook
            using (SaveFileDialog saveBox = new SaveFileDialog())
            {
                saveBox.CheckFileExists = false; //enables user to create file

                result = saveBox.ShowDialog(); //get result from dialog box
                filename = saveBox.FileName; //get the filename
            }
            try
            {
                //open file
                output = new FileStream(filename, FileMode.Create, FileAccess.Write);

                //use the formatter to serialize addressList and write it to the file
                formatter.Serialize(output, addressList);
            }
            catch(IOException)
            {
                MessageBox.Show("Error opening file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //close the file
                output.Close();
            }
        }

        //event handler for opening a file
        //File > Open Addresses
        private void openAddressesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result; //variable used to store response from the save dialog box
            string filename; //stores filename
            
            
            using (OpenFileDialog openBox = new OpenFileDialog())
            {
                openBox.CheckFileExists = false; //enables user to create file

                result = openBox.ShowDialog(); //get result from dialog box
                filename = openBox.FileName; //get the filename
            }

            try
            {
                //open file
                output = new FileStream(filename, FileMode.Open, FileAccess.Read);

                //use the formatter to deserialize addressList and read it
                //you have to downcast it to a List of type Address
                addressList = (List<Address>)formatter.Deserialize(output);
            }
            catch(IOException)
            {
                MessageBox.Show("Error opening file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //close the file
                output.Close();
            }      
        }

        //event handler for Edit > Address
        private void addressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //create new EditAddress form
            EditAddress editForm = new EditAddress(addressList);
            DialogResult editResult;

            editResult = editForm.ShowDialog(); //open editForm as modal

            //user has chosed an address to edit and has clicked "Edit"
            if (editResult == DialogResult.OK)
            {
                DialogResult addrResult;
                int index = editForm.Edit; //int for storing index of selected address
                Address a = addressList[index]; //this is the chosen address to edit

                AddressForm addressForm = new AddressForm(); //opens new AddressForm
                addressForm.AddressName = a.Name.ToString(); //set the name in the text box
                addressForm.Address1 = a.Address1.ToString(); //set the address 1 in the text box
                addressForm.Address2 = a.Address2.ToString(); //set the address 2 in the text box
                addressForm.City = a.City.ToString(); //set the city in the text box
                addressForm.State = a.State.ToString(); //set the state in the text box
                addressForm.ZipText = a.Zip.ToString(); //set the zip code in the text box

                addrResult = addressForm.ShowDialog(); //open edit form with populated values

                //user has clicked ok to edit the address
                if (addrResult == DialogResult.OK)
                {
                    //now we do the reverse of what we just did previously by actually EDITING the
                    //values for the given address, not replacing the address with a new version
                    a.Name = addressForm.AddressName;
                    a.Address1 = addressForm.Address1;
                    a.Address2 = addressForm.Address2;
                    a.City = addressForm.City;
                    a.State = addressForm.State;
                    a.Zip = int.Parse(addressForm.ZipText);

                }
            }
        }
    }
}