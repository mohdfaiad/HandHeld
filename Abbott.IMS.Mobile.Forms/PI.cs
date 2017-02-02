#region Copyright
//============================================================================================
=====================================
//  Copyright 2010-2013, Abbott Laboratories
//  (http://www.abbott.com)
//  All Rights Reserved.
//============================================================================================
=====================================
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using Symbol.Barcode;
using Symbol.RFID3;
using Abbott.IMS.Mobile.STARLiMS;
using Abbott.IMS.Mobile.Helpers;
using Abbott.IMS.Mobile.Codec;
using Abbott.IMS.Mobile.Models;
using Abbott.IMS.Mobile.Controls;
using Abbott.IMS.Mobile.Class;
#endregion

namespace Abbott.IMS.Mobile.Forms
{
    /// <summary>
    /// The PhysicalInventory class handles the view and view controller capabilities
    /// associated with the Physical inventory process/task.
    /// </summary>
    public partial class PhysicalInventory :BaseForm  
    {
        #region Fields
        private bool taskActive;
        private bool taskComplete;
        private int lastRFIDTagTimeStamp;
        private int lastUIUpdateTimeStamp;
        private int sessionId;
        private readonly Object syncObject;
        private System.Threading.Timer displayTimer;
        private LocationDto inventoryLocation;
        private Dictionary<String, ItemDto> expectedItems;
        private Dictionary<String, ItemDto> foundItems;
        private Dictionary<String, ItemDto> missingItems;
        private Dictionary<String, ItemDto> unexpectedItems;
        private Dictionary<String, Object> hexItems;
        private delegate void OnDisplayRefreshHandler(Object state);
        #endregion

        #region Methods
        /// <summary>
        /// Creates an instance of this Form
        /// </summary>
        public PhysicalInventory()
        {
            if (Log.IsInfoEnabled) Log.Info("Mobile: Physical Inventory");

            InitializeComponent();

            // initialize
            this.Text = Resources.Strings.PhysicalInventory;
            this.lblTitle.Text = Resources.Strings.PhysicalInventorySummary;
            this.lblScanMode.Text = ReaderHelper.GetScanMode(TaskType.PhysicalInventory);
            this.lblServerStatus.Text = ServerInterface.ServerStatus;
            this.lblServerStatus.BackColor = ServerInterface.ServerColorStatus;
            this.lblLocation.Text = Resources.Strings.Location;
            this.cmdSettings.Text = Resources.MenusAndButtons.ScanMode;
            this.cmdToolbar1.Click += new EventHandler(cmdToolbar1_Click);
            this.cmdToolbar2.Click += new EventHandler(cmdToolbar2_Click);
            this.cmdSettings.Click += new EventHandler(cmdScanMode_Click);
            this.cmdToolbar1.ButtonUpImage = Resources.Images.toolbar_up;
            this.cmdToolbar1.ButtonDownImage = Resources.Images.toolbar_down;
            this.cmdToolbar1.ButtonFocusImage = Resources.Images.toolbar_focus;
            this.cmdToolbar2.ButtonUpImage = Resources.Images.toolbar_up;
            this.cmdToolbar2.ButtonDownImage = Resources.Images.toolbar_down;
            this.cmdToolbar2.ButtonFocusImage = Resources.Images.toolbar_focus;
            this.cmdSettings.ButtonUpImage = Resources.Images.toolbar_up;
            this.cmdSettings.ButtonDownImage = Resources.Images.toolbar_down;
            this.cmdSettings.ButtonFocusImage = Resources.Images.toolbar_focus;
            this.lblMissingItemsCount.TrailingEllipsis = false;
            this.lblUnexpectedItemsCount.TrailingEllipsis = false;
            this.lblExpectedItemsCount.TrailingEllipsis = false;
            this.KeyPreview = true;
            this.KeyPress += new KeyPressEventHandler(PhysicalInventory_KeyPress);
            this.Closed += new EventHandler(PhysicalInventory_Closed);
            ServerInterface.OnServerStatusChange += new 
ServerInterface.OnServerStatusChangeHandler(ServerInterface_OnServerStatusChange);
            expectedItems = new Dictionary<String, ItemDto>();
            foundItems = new Dictionary<String, ItemDto>();
            missingItems = new Dictionary<String, ItemDto>();
            unexpectedItems = new Dictionary<String, ItemDto>();
            hexItems = new Dictionary<String, Object>();
            syncObject = new Object();
            displayTimer = new System.Threading.Timer(new TimerCallback(DisplayRefreshThread), 
null, Timeout.Infinite, Timeout.Infinite);

            // expected items
            this.lblExpectedItems.Text = Resources.Strings.Expected;
            this.cmdExpectedItems.ButtonUpImage = Resources.Images.button_up;
            this.cmdExpectedItems.ButtonDownImage = Resources.Images.button_down;
            this.cmdExpectedItems.ButtonFocusImage = Resources.Images.button_focus;
            this.cmdExpectedItems.Text = Resources.MenusAndButtons.View;
            this.cmdExpectedItems.Click += new EventHandler(cmdExpectedItems_Click);

            // missing items
            this.lblMissingItems.Text = Resources.Strings.Missing;
            this.cmdMissingItems.ButtonUpImage = Resources.Images.button_up;
            this.cmdMissingItems.ButtonDownImage = Resources.Images.button_down;
            this.cmdMissingItems.ButtonFocusImage = Resources.Images.button_focus;
            this.cmdMissingItems.Text = Resources.MenusAndButtons.View;
            this.cmdMissingItems.Click += new EventHandler(cmdMissingItems_Click);

            // unexpected items
            this.lblUnexpectedItems.Text = Resources.Strings.Unexpected;
            this.cmdUnexpectedItems.ButtonUpImage = Resources.Images.button_up;
            this.cmdUnexpectedItems.ButtonDownImage = Resources.Images.button_down;
            this.cmdUnexpectedItems.ButtonFocusImage = Resources.Images.button_focus;
            this.cmdUnexpectedItems.Text = Resources.MenusAndButtons.View;
            this.cmdUnexpectedItems.Click += new EventHandler(cmdUnexpectedItems_Click);

            // reset task
            ResetTask();

            RegistryHelper.SetOrCreateRegistryValue(RegistryConstants.RegistryLocation, 
RegistryConstants.LastUserTimeInteraction, DateTime.Now);

        }

        /// <summary>
        /// Handles changes in the IMS Server communication status
        /// </summary>
        private void ServerInterface_OnServerStatusChange()
        {
            if (this.InvokeRequired) 
                this.Invoke(new ServerInterface.OnServerStatusChangeHandler
(ServerInterface_OnServerStatusChange), null);
            else
            {
                this.lblServerStatus.Text = ServerInterface.ServerStatus;
                this.lblServerStatus.BackColor = ServerInterface.ServerColorStatus;
            }
        }

        /// <summary>
        /// Handles key presses to toggle the Scan mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhysicalInventory_KeyPress(object sender, KeyPressEventArgs e)
        {
            // toggle scan mode
            if (e.KeyChar == Constants.ScanModeToggleKey)
            {
                ReaderHelper.ToggleScanMode(TaskType.PhysicalInventory);
                this.lblScanMode.Text = ReaderHelper.GetScanMode(TaskType.PhysicalInventory);
                if (taskActive) 
                    ReaderHelper.SetReaderConfiguration(TaskType.PhysicalInventory);
            }

            RegistryHelper.SetOrCreateRegistryValue(RegistryConstants.RegistryLocation, 
RegistryConstants.LastUserTimeInteraction, DateTime.Now);
        }

        /// <summary>
        /// Closes the Physical Inventory task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhysicalInventory_Closed(object sender, EventArgs e)
        {
            // remove event handlers

            // Before de-attaching the event, check what reader mode is on
            if (RFIDReaderInterface.Enabled)
                RFIDReaderInterface.OnTagsRead -= new RFIDReaderInterface.OnTagsReadHandler
(RFIDReaderInterface_OnTagsRead);
            else if (BarcodeReaderInterface.Enabled)
                BarcodeReaderInterface.OnTagsRead -= new 
BarcodeReaderInterface.OnTagsReadHandler(BarcodeReaderInterface_OnTagsRead);

            // Disable readers
            ReaderHelper.DisableReaders();

            // Nullify of the timer
            if (displayTimer != null)
            {
                displayTimer.Change(Timeout.Infinite, Timeout.Infinite);
                displayTimer = null;
            }
        }

        /// <summary>
        /// Handles the View Unexpected Items command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdUnexpectedItems_Click(object sender, EventArgs e)
        {
            RegistryHelper.SetOrCreateRegistryValue(RegistryConstants.RegistryLocation, 
RegistryConstants.LastUserTimeInteraction, DateTime.Now);
            ItemList form = new ItemList(Resources.Strings.UnexpectedItems, true, false, ref 
unexpectedItems);
            ShowDialogForm(form);
        }

        /// <summary>
        /// Handles the View Missing Items command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdMissingItems_Click(object sender, EventArgs e)
        {
            RegistryHelper.SetOrCreateRegistryValue(RegistryConstants.RegistryLocation, 
RegistryConstants.LastUserTimeInteraction, DateTime.Now);
            ItemList form = new ItemList(Resources.Strings.MissingItem, false, false, ref 
missingItems);
            ShowDialogForm(form);
        }

        /// <summary>
        /// Handles the View Expected Items command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdExpectedItems_Click(object sender, EventArgs e)
        {
            RegistryHelper.SetOrCreateRegistryValue(RegistryConstants.RegistryLocation, 
RegistryConstants.LastUserTimeInteraction, DateTime.Now);
            ItemList form = new ItemList(Resources.Strings.ExpectedItem, false, false, ref 
foundItems);
            ShowDialogForm(form);
        }

        /// <summary>
        /// Handles the task processing steps including:
        ///     - Start Physical Inventory process
        ///     - Complete Physical Inventory process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdToolbar1_Click(object sender, EventArgs e)
        {
            RegistryHelper.SetOrCreateRegistryValue(RegistryConstants.RegistryLocation, 
RegistryConstants.LastUserTimeInteraction, DateTime.Now);
            // start and complete actions
            if (this.cmdToolbar1.Text == Resources.MenusAndButtons.Start)
            {
                // start physical inventory
                SelectLocation form = new SelectLocation(TaskType.PhysicalInventory);
                FormsHelper.AddOpenForm(form);   
                DialogResult result = form.ShowDialog();

                if (result == DialogResult.Cancel && AutoLogOff)
                {
                    FormsHelper.CloseOpenForm(this);
                    this.Close();
                }

                if (result == DialogResult.OK && form.SelectedLocation != null)
                {
                    // get items for selected location
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        StartPhysicalInventoryRequestDto startPhysicalInventoryRequest = new 
StartPhysicalInventoryRequestDto();
                        startPhysicalInventoryRequest.LocationId = form.SelectedLocation.Id;

                        if (Log.IsInfoEnabled) Log.Info("IMS: StartPhysicalInventory begin");
                        ServerInterface.LogRequest(startPhysicalInventoryRequest);
                        StartPhysicalInventoryReplyDto startPhysicalInventoryReply = 
ServerInterface.Service.StartPhysicalInventory(startPhysicalInventoryRequest);
                        ServerInterface.LogReply(startPhysicalInventoryReply);
                        if (Log.IsInfoEnabled) Log.Info("IMS: StartPhysicalInventory end");
                        startPhysicalInventoryReply.ThrowError();

                        if (startPhysicalInventoryReply.Items != null)
                        {
                            foreach (ItemDto i in startPhysicalInventoryReply.Items)
                            {
                                if (!StringHelper.IsNullOrEmpty(i.URI))
                                {
                                    if (!expectedItems.ContainsKey(i.URI))
                                    {
                                        expectedItems.Add(i.URI, i);
                                        missingItems.Add(i.URI, i);
                                    }
                                }
                            }
                        }
                        this.cmdToolbar1.Text = Resources.MenusAndButtons.Complete;
                        this.cmdToolbar2.Text = Resources.MenusAndButtons.Cancel;
                        this.lblSelectedLocation.Text = form.SelectedLocation.Name;
                        ReaderHelper.SetReaderConfiguration(TaskType.PhysicalInventory);

                        // Before attaching the event, check what reader mode is on
                        if (RFIDReaderInterface.Enabled)
                            RFIDReaderInterface.OnTagsRead += new 
RFIDReaderInterface.OnTagsReadHandler(RFIDReaderInterface_OnTagsRead);
                        else if (BarcodeReaderInterface.Enabled)
                            BarcodeReaderInterface.OnTagsRead += new 
BarcodeReaderInterface.OnTagsReadHandler(BarcodeReaderInterface_OnTagsRead);

                        sessionId = startPhysicalInventoryReply.SessionId;
                        inventoryLocation = form.SelectedLocation;
                        taskActive = true;
                        displayTimer.Change(0, Constants.UIRefreshInterval);
                        Cursor.Current = Cursors.Default;
                        UpdateDisplay();
                    }
                    catch (IMSException iex)
                    {
                        Log.Error(iex);
                        Cursor.Current = Cursors.Default;
                        MessageBoxDialog.Show(iex.Message, String.Empty, 
MessageBoxButtons1.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                        ResetTask();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        Cursor.Current = Cursors.Default;
                        ServerInterface.SetServerDown();
                        MessageBoxDialog.Show(Resources.ErrorMessages.ServerError, 
String.Empty, MessageBoxButtons1.OK, MessageBoxIcon.Exclamation, 
MessageBoxDefaultButton.Button1);
                    }
                }
            }
            else if (this.cmdToolbar1.Text == Resources.MenusAndButtons.Complete)
            {
                // complete physical inventory
                DialogResult result = MessageBoxDialog.Show
(Resources.ErrorMessages.ConfirmComplete, String.Empty, MessageBoxButtons1.YesNo, 
MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    CompletePhysicalInventoryRequestDto completePhysicalInventoryRequest = new 
CompletePhysicalInventoryRequestDto();
                    completePhysicalInventoryRequest.SessionId = sessionId;
                    List<ItemDto> itemList = new List<ItemDto>();

                    // add found items to list
                    foreach (ItemDto i in foundItems.Values)
                    {
                        itemList.Add(i);
                    }

                    // add unexpected items to list
                    foreach (ItemDto i in unexpectedItems.Values)
                    {
                        itemList.Add(i);
                    }
                    completePhysicalInventoryRequest.Items = itemList.ToArray();

                    try
                    {
                        // attempt to send message to server
                        Cursor.Current = Cursors.WaitCursor;

                        if (Log.IsInfoEnabled) Log.Info("IMS: CompletePhysicalInventory begin");
                        ServerInterface.LogRequest(completePhysicalInventoryRequest);
                        CompletePhysicalInventoryReplyDto completePhysicalInventoryReply = 
ServerInterface.Service.CompletePhysicalInventory(completePhysicalInventoryRequest);
                        ServerInterface.LogReply(completePhysicalInventoryReply);
                        if (Log.IsInfoEnabled) Log.Info("IMS: CompletePhysicalInventory end");
                        completePhysicalInventoryReply.ThrowError();

                        Cursor.Current = Cursors.Default;
                    }
                    catch (IMSException iex)
                    {
                        Log.Error(iex);
                        Cursor.Current = Cursors.Default;
                        MessageBoxDialog.Show(iex.Message, String.Empty, 
MessageBoxButtons1.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        Cursor.Current = Cursors.Default;
                        ServerInterface.SetServerDown();
                        ServerInterface.AddServerMessage(completePhysicalInventoryRequest);
                        MessageBoxDialog.Show(Resources.ErrorMessages.ServerError, 
String.Empty, MessageBoxButtons1.OK, MessageBoxIcon.Exclamation, 
MessageBoxDefaultButton.Button1);
                    }
                    finally
                    {
                        ResetTask();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the task processing steps including:
        ///     - Navigate to Main menu
        ///     - Cancel Physical Inventory process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdToolbar2_Click(object sender, EventArgs e)
        {
            RegistryHelper.SetOrCreateRegistryValue(RegistryConstants.RegistryLocation, 
RegistryConstants.LastUserTimeInteraction, DateTime.Now);
            // main menu and cancel actions
            if (this.cmdToolbar2.Text == Resources.MenusAndButtons.Main)
            {
                // return to the main menu
                ResetTask();
                FormsHelper.CloseOpenForm(this);
                this.Close();
            }
            else if (this.cmdToolbar2.Text == Resources.MenusAndButtons.Cancel)
            {
                // cancel the physical inventory
                DialogResult result = MessageBoxDialog.Show
(Resources.ErrorMessages.ConfirmCancel, String.Empty, MessageBoxButtons1.YesNo, 
MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    CancelPhysicalInventoryRequestDto cancelPhysicalInventoryRequest = new 
CancelPhysicalInventoryRequestDto();
                    cancelPhysicalInventoryRequest.SessionId = sessionId;

                    try
                    {
                        // attempt to send message to server
                        Cursor.Current = Cursors.WaitCursor;

                        if (Log.IsInfoEnabled) Log.Info("IMS: CancelPhysicalInventory begin");
                        ServerInterface.LogRequest(cancelPhysicalInventoryRequest);
                        CancelPhysicalInventoryReplyDto cancelPhysicalInventoryReply = 
ServerInterface.Service.CancelPhysicalInventory(cancelPhysicalInventoryRequest);
                        ServerInterface.LogReply(cancelPhysicalInventoryReply);
                        if (Log.IsInfoEnabled) Log.Info("IMS: CancelPhysicalInventory end");
                        cancelPhysicalInventoryReply.ThrowError();

                        Cursor.Current = Cursors.Default;
                    }
                    catch (IMSException iex)
                    {
                        Log.Error(iex);
                        Cursor.Current = Cursors.Default;
                        MessageBoxDialog.Show(iex.Message, String.Empty, 
MessageBoxButtons1.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        Cursor.Current = Cursors.Default;
                        ServerInterface.SetServerDown();
                        ServerInterface.AddServerMessage(cancelPhysicalInventoryRequest);
                    }
                    finally
                    {
                        ResetTask();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Scan Mode command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdScanMode_Click(object sender, EventArgs e)
        {
            RegistryHelper.SetOrCreateRegistryValue(RegistryConstants.RegistryLocation, 
RegistryConstants.LastUserTimeInteraction, DateTime.Now);
            Settings form;

            // show scan mode
            form = new Settings(TaskType.PhysicalInventory);

            ShowDialogForm(form);
        }

        /// <summary>
        /// Handles Barcode read events
        /// </summary>
        /// <param name="d"></param>
        private void BarcodeReaderInterface_OnTagsRead(ReaderData d)
        {
            if (!StringHelper.IsNullOrEmpty(d.Text))
            {
                if (Log.IsDebugEnabled) Log.Debug(String.Format("Barcode Read - Raw: {0}", 
d.Text));
                string hexTagId = StringHelper.GetHexStringFromBarcodeScan(d.Text);
                if (StringHelper.IsNullOrEmpty(hexTagId))
                    MessageBoxDialog.Show(Resources.ErrorMessages.UnrecognizedBarcodeError + 
"(IMS6)", String.Empty, MessageBoxButtons1.OK, MessageBoxIcon.Exclamation, 
MessageBoxDefaultButton.Button1);
                else
                    ProcessTag(hexTagId);
            }

            RegistryHelper.SetOrCreateRegistryValue(RegistryConstants.RegistryLocation, 
RegistryConstants.LastUserTimeInteraction, DateTime.Now);
        }

        /// <summary>
        /// Handles RFID read events
        /// </summary>
        /// <param name="e"></param>
        private void RFIDReaderInterface_OnTagsRead(Events.ReadEventArgs e)
        {
            RegistryHelper.SetOrCreateRegistryValue(RegistryConstants.RegistryLocation, 
RegistryConstants.LastUserTimeInteraction, DateTime.Now);
            ProcessTag(e.ReadEventData.TagData.TagID);
        }

        /// <summary>
        /// Processes GS1 tag data 
        /// </summary>
        /// <param name="hexTagId">String of Hex digits from GS1 Tag encding</param>
        private void ProcessTag(String hexTagId)
        {
            // Avoid processing tags at the hex level as soon as possible.
            if (!hexItems.ContainsKey(hexTagId))
                hexItems[hexTagId] = null;
            else
                return;

            String uri;
            ItemDto unexpectedItem;

            // Determine if we can convert the read to a GS1 Pure Identity URI string 
otherwise fall back to the EPC Tag URI format
            TagEncoding enc = TagEncoding.ParseHex(hexTagId);
            if (enc.PureIdentity != null)
                uri = enc.PureIdentity.ToURI();
            else
                uri = enc.ToURI();

            // Filter tag URIs based on the application configuration.
            if (StringHelper.IsFiltered(TaskType.PhysicalInventory, uri))
            {
                if (Log.IsDebugEnabled) Log.Debug(String.Format("Filtered Tag Read - Hex: {0} 
| URI: {1}", hexTagId, uri));
                return;
            }

            // Special case logging for Tag data analysis
            if (Log.IsDebugEnabled)
                Log.Debug(String.Format("Tag Read - Hex: {0} | URI: {1}", hexTagId, uri));

            // Ignore RAW / SSCC tags in the Physical inventory process.
            if (uri.StartsWith(RFIDTagConstants.SGTINPrefix) || uri.StartsWith
(RFIDTagConstants.SGTIN96Prefix))
            {
                bool newItem = false;

                try
                {
                    lock (syncObject)
                    {
                        if ((expectedItems.ContainsKey(uri)) && (!foundItems.ContainsKey
(uri)))
                        {
                            // this is an expected item; add item to found list and remove 
item from missing item list
                            foundItems.Add(uri, expectedItems[uri]);
                            if (missingItems.ContainsKey(uri))
                                missingItems.Remove(uri);
                            newItem = true;
                        }
                        else if ((!expectedItems.ContainsKey(uri)) && (!
unexpectedItems.ContainsKey(uri)))
                        {
                            // add item to the unexpected list
                            unexpectedItem = new ItemDto();
                            unexpectedItem.URI = uri;
                            unexpectedItems.Add(uri, unexpectedItem);
                            newItem = true;
                        }
                    }

                    // Beep outside the locking context
                    if (newItem)
                    {
                        ReaderHelper.Beep();

                        // Request update on the UI thread
                        lastRFIDTagTimeStamp = Environment.TickCount;
                    }
                }
                finally
                {
                    // Nullify the object(s)
                    unexpectedItem = null;
                }
            }
        }

        /// <summary>
        /// Display the specified Form instance while disabling read events
        /// </summary>
        /// <param name="form">Form instance</param>
        /// <returns>DialogResult result value</returns>
        private DialogResult ShowDialogForm(BaseForm form)
        {
            DialogResult result;

            // disable readers and detach events
            if (taskActive)
            {

                // Before de-attaching the event, check what reader mode is on
                if (RFIDReaderInterface.Enabled)
                    RFIDReaderInterface.OnTagsRead -= new 
RFIDReaderInterface.OnTagsReadHandler(RFIDReaderInterface_OnTagsRead);
                else if (BarcodeReaderInterface.Enabled)
                    BarcodeReaderInterface.OnTagsRead -= new 
BarcodeReaderInterface.OnTagsReadHandler(BarcodeReaderInterface_OnTagsRead);

                // Disable readers
                ReaderHelper.DisableReaders();
            }
            FormsHelper.AddOpenForm(form);   
            // show popup form
            result = form.ShowDialog();

            if (result == DialogResult.Cancel && AutoLogOff)
            {
                FormsHelper.CloseOpenForm(this);
                this.Close();
            }

            this.lblScanMode.Text = ReaderHelper.GetScanMode(TaskType.PhysicalInventory);
            UpdateDisplay();
            this.cmdToolbar1.Focus();

            // enable readers if the task is active and reattach event handlers
            if (taskActive)
            {
                ReaderHelper.SetReaderConfiguration(TaskType.PhysicalInventory);

                // Before attaching the event, check what reader mode is on
                if (RFIDReaderInterface.Enabled)
                    RFIDReaderInterface.OnTagsRead += new 
RFIDReaderInterface.OnTagsReadHandler(RFIDReaderInterface_OnTagsRead);
                else if (BarcodeReaderInterface.Enabled)
                    BarcodeReaderInterface.OnTagsRead += new 
BarcodeReaderInterface.OnTagsReadHandler(BarcodeReaderInterface_OnTagsRead);
            }
            return result;
        }

        /// <summary>
        /// Handler for thread timer callback to refresh UI
        /// </summary>
        /// <param name="state"></param>
        private void DisplayRefreshThread(Object state)
        {
            try
            {
                // Update only if needed.
                if (this.lastUIUpdateTimeStamp <= (this.lastRFIDTagTimeStamp + 
Constants.UIRefreshInterval))
                {
                    // this method handles frequent updating of the UI when one of the 
scanners is active
                    if (this.InvokeRequired)
                        this.Invoke(new OnDisplayRefreshHandler(DisplayRefreshThread), new 
Object[] { state });
                    else
                    {
                        lastUIUpdateTimeStamp = Environment.TickCount;
                        UpdateDisplay();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        /// Updates the UI with the process task information
        /// </summary>
        private void UpdateDisplay()
        {
            int foundItems;
            int expectedItems;
            int missingItems;
            int unexpectedItems;

            // update display    
            lock (syncObject)
            {
                foundItems = this.foundItems.Count;
                expectedItems = this.expectedItems.Count;
                missingItems = this.missingItems.Count;
                unexpectedItems = this.unexpectedItems.Count;
            }

            // expected and missing item labels
            if (expectedItems > 0)
            {
                // update missing and expected items
                this.lblMissingItemsCount.Text = missingItems.ToString();
                this.lblExpectedItemsCount.Text = foundItems.ToString();

                // check if task is complete
                if (missingItems == 0)
                {
                    if (!taskComplete)
                    {
                        // task is complete
                        taskComplete = true;
                        SoundHelper.PlaySound(SoundHelper.Complete);
                    }
                }
                else
                {
                    // task is not complete
                    taskComplete = false;
                }
            }
            else
            {
                this.lblExpectedItemsCount.Text = string.Empty  ;
                this.lblMissingItemsCount.Text = string.Empty  ;
            }

            // unexpected item labels
            if (unexpectedItems > 0) 
                this.lblUnexpectedItemsCount.Text = unexpectedItems.ToString();
            else 
                this.lblUnexpectedItemsCount.Text = string.Empty  ;

            // found item viewing
            if (foundItems > 0 && !this.cmdExpectedItems.Enabled) 
                this.cmdExpectedItems.Enabled = true;
            else if (foundItems == 0 && this.cmdExpectedItems.Enabled)
                this.cmdExpectedItems.Enabled = false;

            // missing item viewing
            if (missingItems > 0 && !this.cmdMissingItems.Enabled) 
                this.cmdMissingItems.Enabled = true;
            else if (missingItems == 0 && this.cmdMissingItems.Enabled)
                this.cmdMissingItems.Enabled = false;

            // unexpected item viewing
            if (unexpectedItems > 0 && !this.cmdUnexpectedItems.Enabled) 
                this.cmdUnexpectedItems.Enabled = true;
            else if (unexpectedItems == 0 && this.cmdUnexpectedItems.Enabled)
                this.cmdUnexpectedItems.Enabled = false;
        }

        /// <summary>
        /// Reset task by clearing counters and resetting labels and menu options
        /// </summary>
        private void ResetTask()
        {
            this.cmdToolbar1.Text = Resources.MenusAndButtons.Start;
            this.cmdToolbar1.Focus();
            this.cmdToolbar2.Text = Resources.MenusAndButtons.Main;
            this.lblSelectedLocation.Text = string.Empty;
            inventoryLocation = null;
            taskActive = false;
            taskComplete = false;
            sessionId = -1;
            expectedItems.Clear();
            foundItems.Clear();
            missingItems.Clear();
            unexpectedItems.Clear();
            hexItems.Clear();
            this.lastRFIDTagTimeStamp = -1;
            this.lastUIUpdateTimeStamp = 0;
            displayTimer.Change(Timeout.Infinite, Timeout.Infinite);

            // Before de-attaching the event, check what reader mode is on
            if (RFIDReaderInterface.Enabled)
                RFIDReaderInterface.OnTagsRead -= new RFIDReaderInterface.OnTagsReadHandler
(RFIDReaderInterface_OnTagsRead);
            else if (BarcodeReaderInterface.Enabled)
                BarcodeReaderInterface.OnTagsRead -= new 
BarcodeReaderInterface.OnTagsReadHandler(BarcodeReaderInterface_OnTagsRead);

            // Disable readers
            ReaderHelper.DisableReaders();

            UpdateDisplay();
        }
        #endregion
    }
}